using GraphQL.Types;

namespace API.Models
{
    public class SuccessType : ObjectGraphType
    {
        public SuccessType()
        {
            Field<StringGraphType>("ok", resolve: _ => "ok");
        }
    }
}