using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using API.Models;
using DynamicExpresso;

namespace API.Utils.TemplateParser
{
    public class ExpressionParser
    {
        private readonly string varDelLeft = "<";
        private readonly string varDelRight = ">";

        public string Parse(string expressionFieldContent, List<TemplateField> inputFields)
        {
            string expression = ReplaceVariables(expressionFieldContent, inputFields);

            if (String.IsNullOrEmpty(expression))
            {
                return expressionFieldContent;
            }

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
            foreach (Match match in Regex.Matches(expressionFieldContent, $"{varDelLeft}(.*?){varDelRight}"))
            {
                string variable = Regex.Replace(match.Value, $"{varDelLeft}|{varDelRight}", "").Trim();
                TemplateField replacement = inputFields.Find(_ => _.Content == variable);
                if (String.IsNullOrEmpty(replacement?.Replacement))
                {
                    return null;
                    // throw new Exception($"Variable '{variable}' in expression '{expressionFieldContent}' has no replacement!");
                }
                expression = Regex.Replace(expression, $"{varDelLeft} *{variable} *{varDelRight}", replacement.Replacement);
            }
            return expression;
        }
    }
}