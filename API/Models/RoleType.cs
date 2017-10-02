using GraphQL.Types;

namespace API.Models
{
    public class RoleType : ObjectGraphType<Role>
    {
        public RoleType()
        {
            Name = "role";

            Field(_ => _.Id);
            Field(_ => _.Name);
        }
    }
}