using System;
using System.Linq;
using API.Contexts;
using API.Models;
using API.Services;
using GraphQL;
using GraphQL.Types;
using JWT;
using JWT.Serializers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace API.Queries
{
    public class RootQuery : ObjectGraphType
    {

        public RootQuery(IServiceProvider serviceProvider, IConfiguration configuration, AuthService authService)
        {

            // .Net Core DI does not work in GraphQL yet
            QueryType._serviceProvider = serviceProvider;

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