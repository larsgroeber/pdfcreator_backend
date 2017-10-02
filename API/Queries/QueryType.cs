using System;
using System.Linq;
using API.Contexts;
using API.Services;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace API.Models
{
    public class QueryType : ObjectGraphType
    {
        internal static IServiceProvider _serviceProvider { get; set; }
        private readonly AuthService _auth;

        public QueryType()
        {
            Name = "query";
            _auth = _serviceProvider.GetService<AuthService>();

            Field<UserType>("user",
                arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "id" }),
                resolve: context =>
                {
                    int id = context.GetArgument<int>("id");
                    if (_auth.User.Id == id || _auth.CheckAuthentication("admin"))
                    {
                        return _serviceProvider.GetService<PDFCreatorContext>().Users
                            .Include(_ => _.Role).FirstOrDefault(_ => _.Id == id);
                    }
                    throw new UnauthorizedAccessException("Role " + "admin" + " required!");
                });

        }
    }
}