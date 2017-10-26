using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using API.Models;
using GraphQL;

namespace API.TemplateParser
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

            foreach (var templateField in result)
            {
                Console.WriteLine(templateField.ToString());
            }
            return result;
        }

        public string ReplaceFields(string template, List<TemplateField> templateFields)
        {
            string resultTemplate = template;
            foreach (TemplateField templateField in GetFieldsByType(resultTemplate, FieldType.Placeholder))
            {
                Console.WriteLine(templateField.ToString());
                TemplateField replacementField = templateFields.Find(_ => _.Content == templateField.Content);
                Console.WriteLine(replacementField.ToString());
                if (replacementField?.Replacement != null)
                {
                    Console.WriteLine(replacementField.ToString());
                    Delimiter delimiter = GetDelimiter(FieldType.Placeholder);
                    resultTemplate = Regex.Replace(
                        resultTemplate,
                        $"{delimiter.Left} *{templateField.Content} *(; *.*)? *{delimiter.Right}",
                        replacementField.Replacement);
                }
            }
            return resultTemplate;
        }

        private List<TemplateField> GetFieldsByType(string template, FieldType type)
        {
            Delimiter delimiter = GetDelimiter(type);

            List<TemplateField> result = new List<TemplateField>();

            foreach (Match match in GetMatches(template, delimiter))
            {
                string field = Regex.Replace(match.Value, $"{delimiter.Left}|{delimiter.Right}", "");
                result.Add(ParseTemplateField(field));
            }
            return result;
        }

        private TemplateField ParseTemplateField(string field)
        {
            List<string> strings = new List<string>(field.Split(";").Map(s => s.Trim()));
            TemplateField templateField = new TemplateField();

            if (strings[0].Contains(" "))
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
                default:
                    throw new Exception($"Field type {type.ToString()} is not supported!");
            }
            return del;
        }

        private MatchCollection GetMatches(string template, Delimiter delimiter)
        {
            return Regex.Matches(template, $"{delimiter.Left}(.*?){delimiter.Right}");
        }
    }
}