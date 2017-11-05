using System.Linq;
using GraphQL.Types;

namespace API.Models
{
    public class UserType : ObjectGraphType<User>
    {
        public UserType()
        {
            Name = "user";

            Field(_ => _.Id);
            Field(_ => _.Name);
            Field(_ => _.Email);
            Field<ListGraphType<TemplateType>>("templates");
            Field<TemplateType>("template",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id"}),
                resolve: context =>
                {
                    int id = context.GetArgument<int>("id");
                    return context.Source.Templates.SingleOrDefault(_ => _.Id == id);
                });
            Field("role", _ => _.Role.Name);
        }
    }
}