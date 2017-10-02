using System;
using API.Models;
using API.Services;
using GraphQL.Language.AST;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace API.Queries
{
    public class AuthType : ObjectGraphType
    {
        internal static IServiceProvider ServiceProvider { get; set; }

        public AuthType()
        {
            AuthService authService = ServiceProvider.GetService<AuthService>();

            Field<StringGraphType>("token", resolve: _ => authService.Token);
            Field<UserType>("user",
                resolve: context => authService.User);
        }
    }
}