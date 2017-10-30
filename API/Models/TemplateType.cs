using System;
using API.Services;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace API.Models
{
    public class TemplateType : ObjectGraphType<Template>
    {
        internal static IServiceProvider ServiceProvider;
        private Document _document;
        private TemplateService _templateService;


        public TemplateType()
        {
            _templateService = ServiceProvider.GetService<TemplateService>();

            Field(_ => _.Id);
            Field(_ => _.Name);
            Field(_ => _.Description);
            Field<StringGraphType>("document",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "fields", DefaultValue = ""},
                    new QueryArgument<StringGraphType> { Name = "multiple", DefaultValue = false}),
                resolve: ctx => GetDocument(ctx).DataUri);
            Field<StringGraphType>("fields",
                resolve: ctx => GetDocument(ctx).TemplateFields);
        }

        private Document GetDocument(ResolveFieldContext<Template> ctx)
        {
            if (_document.Template == null)
            {
                _document = _templateService.Compile(ctx);
            }
            return _document;
        }
    }
}