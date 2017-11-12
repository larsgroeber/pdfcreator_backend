using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using API.Models;
using GraphQL;

namespace API.Utils.TemplateParser
{
    public class TemplateParser
    {
        public List<TemplateField> GetInputFields(string template)
        {
            List<TemplateField> result = new List<TemplateField>();

            foreach (FieldType fieldType in new[] {FieldType.Placeholder, FieldType.Variable})
            {
                result.AddRange(GetFieldsByType(template, fieldType));
            }

            return result;
        }

        public string ReplacePlaceholderFields(string template, List<TemplateField> templateFields)
        {
            string resultTemplate = template;
            foreach (TemplateField templateField in GetFieldsByType(template, FieldType.Placeholder))
            {
                TemplateField replacementField = templateFields.Find(_ => _.Content == templateField.Content);

                if (replacementField?.Replacement != null)
                {
                    resultTemplate = ReplaceField(resultTemplate, replacementField, FieldType.Placeholder);
                }
            }
            return resultTemplate;
        }

        public string ReplaceField(string template, TemplateField replacementField, FieldType type)
        {
            Delimiter delimiter = GetDelimiter(type);
            return Regex.Replace(
                template,
                $"{delimiter.Left} *{replacementField.Content} *(; *.*)? *{delimiter.Right}",
                replacementField.Replacement);
        }

        public List<TemplateField> GetFieldsByType(string template, FieldType type)
        {
            Delimiter delimiter = GetDelimiter(type);

            List<TemplateField> result = new List<TemplateField>();

            foreach (Match match in GetMatches(template, delimiter))
            {
                Console.WriteLine(match.Value);
                string field = Regex.Replace(match.Value, $"{delimiter.Left}|{delimiter.Right}", "");
                CheckForDisallowedCharacters(field);
                result.Add(ParseTemplateField(field, type));
            }
            return result;
        }

        private TemplateField ParseTemplateField(string field, FieldType type)
        {
            List<string> strings = new List<string>(field.Split(";").Map(s => s.Trim()));
            TemplateField templateField = new TemplateField();

            if (type != FieldType.Expression && strings[0].Contains(" "))
            {
                throw new Exception($"Template content cannot contain spaces but '{strings[0]}' does!");
            }

            if (strings.Count > 1)
            {
                templateField.Comment = strings[1].Trim();
            }
            templateField.Content = strings[0].Trim();
            return templateField;
        }

        private Delimiter GetDelimiter(FieldType type)
        {
            Delimiter del = new Delimiter();

            switch (type)
            {
                case FieldType.Placeholder:
                    del.Left = Delimiters.PlaceholderLeft;
                    del.Right = Delimiters.PlaceholderRight;
                    break;
                case FieldType.Expression:
                    del.Left = Delimiters.ExpressionLeft;
                    del.Right = Delimiters.ExpressionRight;
                    break;
                case FieldType.Variable:
                    del.Left = Delimiters.VariableLeft;
                    del.Right = Delimiters.VariableRight;
                    break;
                case FieldType.Comment:
                    del.Left = Delimiters.CommentLeft;
                    del.Right = Delimiters.CommentRight;
                    break;
                default:
                    throw new Exception($"Field type {type.ToString()} is not supported!");
            }
            return del;
        }

        private MatchCollection GetMatches(string template, Delimiter delimiter)
        {
            return Regex.Matches(template, $"{delimiter.Left}(.*?){delimiter.Right}");
        }

        public string DeleteCommentsAndVariables(string template)
        {
            foreach (FieldType fieldType in new[] {FieldType.Comment, FieldType.Variable})
            {
                Delimiter delimiter = GetDelimiter(fieldType);
                template = Regex.Replace(template, $"{delimiter.Left}.*{delimiter.Right}", String.Empty);
            }
            return template;
        }

        public string EscapeTemplateFields(string template)
        {
            string pkgFancyvrb = "\\usepackage{fancyvrb}";
            if (!Regex.Match(template, "\\" + pkgFancyvrb).Success)
            {
                Match usePackageMatch = Regex.Match(template, "usepackage{.*}");
                if (usePackageMatch.Success)
                {
                    template = Regex.Replace(template, usePackageMatch.Value,
                        string.Format("{0}\n{1}\n", usePackageMatch.Value, pkgFancyvrb));
                    Console.WriteLine("Added package: " + template);
                }
                else
                {
                    throw new Exception("Please add a '\\usepackage{fancyvrb}' to your document. " +
                                        "The template fields are excaped by '\\Verb|...|'.");
                }
            }

            foreach (FieldType fieldType in new[] {FieldType.Placeholder, FieldType.Expression})
            {
                Delimiter delimiter = GetDelimiter(fieldType);
                template = Regex.Replace(template, $"{delimiter.Left}.*{delimiter.Right}", m =>
                    String.Format("\\Verb|{0}|", m.Value));
            }
            return template;
        }

        /// <summary>
        /// Checks for disallowed characters and throws an exception if it encounters one.
        /// </summary>
        /// <param name="template">The template text</param>
        /// <exception cref="Exception">If a disallowed character is found</exception>
        public static void CheckForDisallowedCharacters(string template)
        {
            foreach (string s in new[] {"|"})
            {
                if (Regex.Match(template, s).Length > 0)
                {
                    throw new Exception($"The character '{s}' is not allowed inside a templatefield or replacement!");
                }
            }
        }
    }
}