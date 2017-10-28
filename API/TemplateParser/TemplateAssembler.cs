using System.Collections.Generic;
using API.Models;

namespace API.TemplateParser
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
                expressionField.Replacement = expressionParser.Parse(expressionField.Content, inputFields);
                finalTemplate = templateParser.ReplaceField(finalTemplate, expressionField, FieldType.Expression);
            }

            finalTemplate = templateParser.ReplacePlaceholderFields(finalTemplate, inputFields);
            finalTemplate = templateParser.DeleteCommentsAndVariables(finalTemplate);

            return finalTemplate;
        }
    }
}