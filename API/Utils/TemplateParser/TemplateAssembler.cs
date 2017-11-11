using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using API.Models;
using API.Services;

namespace API.Utils.TemplateParser
{
    public class TemplateAssembler
    {
        private readonly string _template;

        public TemplateAssembler(string template)
        {
            _template = template;
        }

        public string Assemble(List<TemplateField> inputFields)
        {
            TemplateParser templateParser = new TemplateParser();
            ExpressionParser expressionParser = new ExpressionParser();

            var expressionFields = templateParser.GetFieldsByType(_template, FieldType.Expression);
            string finalTemplate = _template;

            foreach (TemplateField expressionField in expressionFields)
            {
                try
                {
                    expressionField.Replacement = expressionParser.Parse(expressionField.Content, inputFields);
                    if (expressionField.Replacement != expressionField.Content)
                    {
                        expressionField.Content = EscapeRegex(expressionField.Content);
                        finalTemplate = templateParser.ReplaceField(finalTemplate, expressionField, FieldType.Expression);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    GraphQlErrorService.Add(e);
                }
            }

            finalTemplate = templateParser.ReplacePlaceholderFields(finalTemplate, inputFields);
            finalTemplate = templateParser.DeleteCommentsAndVariables(finalTemplate);
            finalTemplate = templateParser.EscapeTemplateFields(finalTemplate);

            return finalTemplate;
        }

        public static string EscapeRegex(string str)
        {
            return Regex.Escape(str);
        }
    }
}