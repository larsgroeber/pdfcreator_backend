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
        internal static IServiceProvider _serviceProvider { get; set; }

        public AuthType()
        {
            AuthService authService = _serviceProvider.GetService<AuthService>();

            Field<StringGraphType>("token", resolve: _ => authService.Token);
            Field<UserType>("user",
                resolve: context => authService.User);
        }
    }
}