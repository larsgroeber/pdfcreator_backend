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
            Field<ListGraphType<TemplateType>>("templates");
            Field("role", _ => _.Role.Name);
        }
    }
}