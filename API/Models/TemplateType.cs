using System;
using GraphQL.Types;

namespace API.Models
{
    public class TemplateType : ObjectGraphType<Template>
    {
        internal static IServiceProvider ServiceProvider;

        public TemplateType()
        {
            Field(_ => _.Id);
            Field(_ => _.Name);
            Field(_ => _.Description);
            Field<StringGraphType>("document",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "fields", DefaultValue = ""}),
                resolve: context => "Thats how we get the id: " + context.Source.Id);
        }
    }
}