using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using API.Models;
using GraphQL;

namespace API.Services
{
    public struct Delimiters
    {
        public static readonly string PlaceholderLeft = "<<";
        public static readonly string PlaceholderRight = ">>";
        public static readonly string ExpressionLeft = "<=";
        public static readonly string ExpressionRight = "=>";
        public static readonly string VariableLeft = "<\\$";
        public static readonly string VariableRight = "\\$>";
    }

    public struct Delimiter
    {
        public string Left;
        public string Right;
    }

    public enum FieldType
    {
        Placeholder,
        Expression,
        Comment,
        Variable
    }

    public class TemplateParseService
    {
        public List<TemplateField> GetFields(string template)
        {
            List<TemplateField> result = new List<TemplateField>();


            foreach (FieldType fieldType in new[] {FieldType.Placeholder, FieldType.Variable})
            {
                result.AddRange(GetFieldsByType(template, fieldType).Map(s => new TemplateField {Name = s}));
            }

            foreach (var templateField in result)
            {
                Console.WriteLine(templateField.ToString());
            }
            return new List<TemplateField>();
        }

        private List<string> GetFieldsByType(string template, FieldType type)
        {
            Delimiter delimiter = GetDelimiter(type);

            List<string> result = new List<string>();

            foreach (Match match in GetMatches(template, delimiter))
            {
                result.Add(Regex.Replace(match.Value, $"{delimiter.Left}|{delimiter.Right}", "").Trim());
            }
            return result;
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