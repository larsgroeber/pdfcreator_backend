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
                    GraphQlErrorService.Context = context;
                    string token = context.GetArgument<string>("token");

                    try
                    {
                        if (token != "")
                        {
                            authService.CheckJwt(token);
                        }
                        return new QueryType();
                    }
                    catch (Exception e)
                    {
                        GraphQlErrorService.AttachError(context, e);
                        return null;
                    }
                });
        }
    }
}