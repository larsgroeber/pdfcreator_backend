using System;
using API.Models;
using API.Services;
using GraphQL;
using GraphQL.Types;

namespace API.Mutations
{
    public class RootMutation : ObjectGraphType
    {
        public RootMutation(AuthService authService)
        {
            Name = "mutation";

            Field<MutationType>("mutation",
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
                        return new MutationType();
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