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

        public TemplateType()
        {
            var templateService = ServiceProvider.GetService<TemplateService>();

            Field(_ => _.Id);
            Field(_ => _.Name);
            Field(_ => _.Description);
            Field<StringGraphType>("document",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "fields", DefaultValue = ""}),
                resolve: ctx => templateService.Compile(ctx).DataUri);
            // TODO no second Compiling
            Field<StringGraphType>("fields",
                resolve: ctx => templateService.Compile(ctx).TemplateFields);
        }
    }
}