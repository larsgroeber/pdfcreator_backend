using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using API.Models;
using DynamicExpresso;

namespace API.TemplateParser
{
    public class ExpressionParser
    {
        private readonly string varDelLeft = "<";
        private readonly string varDelRight = ">";

        public string Parse(string expressionFieldContent, List<TemplateField> inputFields)
        {
            string expression = ReplaceVariables(expressionFieldContent, inputFields);

            var engine = new Interpreter();
            try
            {
                return engine.Eval(expression).ToString();
            }
            catch (Exception e)
            {
                throw new Exception($"Error in statement '{expression}': {e.Message}");
            }
        }

        private string ReplaceVariables(string expressionFieldContent, List<TemplateField> inputFields)
        {
            string expression = expressionFieldContent;
            foreach (Match match in Regex.Matches(expressionFieldContent, $"{varDelLeft}.*{varDelRight}"))
            {
                string variable = Regex.Replace(match.Value, $"{varDelLeft}|{varDelRight}", "").Trim();
                string replacement = inputFields.Find(_ => _.Content == variable)?.Replacement;
                if (replacement == null)
                {
                    throw new Exception($"Variable '{variable}' in expression '{expressionFieldContent}' has no replacement!");
                }

                expression = Regex.Replace(expression, $"{varDelLeft} *{variable} *{varDelRight}", replacement);
            }
            return expression;
        }
    }
}