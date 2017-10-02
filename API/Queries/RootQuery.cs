using System;
using API.Models;
using API.Services;
using GraphQL;
using GraphQL.Types;

namespace API.Queries
{
    public class RootQuery : ObjectGraphType
    {

        public RootQuery(AuthService authService)
        {
            Field<QueryType>("query",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> {Name = "token", DefaultValue = ""}),
                resolve: context =>
                {
                    string token = context.GetArgument<string>("token");

                    if (token != "")
                    {
                        try
                        {
                            authService.CheckJwt(token);
                        }
                        catch (UnauthorizedAccessException e)
                        {
                            context.Errors.Add(new ExecutionError(e.ToString()));
                        }
                    }

                    return new QueryType();
                });
        }
    }
}