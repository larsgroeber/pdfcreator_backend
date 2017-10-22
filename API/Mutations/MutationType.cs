using System;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using API.Contexts;
using API.Models;
using API.Queries;
using API.Services;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace API.Mutations
{
    public class MutationType : ObjectGraphType
    {
        internal static IServiceProvider ServiceProvider { get; set; }

        public MutationType()
        {
            AuthType.ServiceProvider = ServiceProvider;
            UserService userService = ServiceProvider.GetService<UserService>();
            TemplateService templateService = ServiceProvider.GetService<TemplateService>();

            Field<AuthType>("authorize",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> {Name = "username"},
                    new QueryArgument<StringGraphType> {Name = "password"}),
                resolve: userService.Authorize);



            Field<UserType>("addUser",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> {Name = "username"},
                    new QueryArgument<StringGraphType> {Name = "email"},
                    new QueryArgument<StringGraphType> {Name = "password"}),
                resolve: userService.AddUser);

            Field<UserType>("updateUser",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> {Name = "id"},
                    new QueryArgument<StringGraphType> {Name = "username", DefaultValue = ""},
                    new QueryArgument<StringGraphType> {Name = "email", DefaultValue = ""},
                    new QueryArgument<StringGraphType> {Name = "password", DefaultValue = ""},
                    new QueryArgument<StringGraphType> {Name = "role", DefaultValue = ""}),
                resolve: userService.UpdateUser);

            Field<SuccessType>("removeUser",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> {Name = "id"}),
                resolve: userService.RemoveUser);



            Field<UserType>("addTemplateToUser",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> {Name = "idTemplate"},
                    new QueryArgument<IntGraphType> {Name = "idUser"}),
                resolve: userService.AddTemplateToUser);

            Field<UserType>("removeTemplateFromUser",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> {Name = "idTemplate"},
                    new QueryArgument<IntGraphType> {Name = "idUser"}),
                resolve: userService.RemoveTemplateFromUser);



            Field<TemplateType>("addTemplate",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> {Name = "name"},
                    new QueryArgument<StringGraphType> {Name = "description", DefaultValue = ""},
                    new QueryArgument<IntGraphType> {Name = "idUser", DefaultValue = 0}),
                resolve: templateService.AddTemplate);

            Field<TemplateType>("updateTemplate",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> {Name = "id"},
                    new QueryArgument<StringGraphType> {Name = "name", DefaultValue = ""},
                    new QueryArgument<StringGraphType> {Name = "description", DefaultValue = ""}),
                resolve: templateService.UpdateTemplate);

            Field<SuccessType>("removeTemplate",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> {Name = "id"}),
                resolve: templateService.RemoveTemplate);
        }


    }
}