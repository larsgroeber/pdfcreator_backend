using System;
using API.Services;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace API.Models
{
    public class TemplateType : ObjectGraphType<Template>
    {
        internal static IServiceProvider ServiceProvider;
        private readonly TemplateService _templateService;

        public TemplateType()
        {
            _templateService = ServiceProvider.GetService<TemplateService>();

            Field(_ => _.Id);
            Field(_ => _.Name);
            Field(_ => _.Description);
            Field<StringGraphType>("document",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "fields", DefaultValue = ""}),
                resolve: _templateService.Compile);
        }
    }
}