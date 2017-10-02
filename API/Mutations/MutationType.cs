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
        private readonly AuthService _auth;
        private readonly PDFCreatorContext _context;

        public MutationType()
        {
            AuthType._serviceProvider = ServiceProvider;
            _auth = ServiceProvider.GetService<AuthService>();
            _context = ServiceProvider.GetService<PDFCreatorContext>();

            Field<AuthType>("authorize",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> {Name = "username"},
                    new QueryArgument<StringGraphType> {Name = "password"}),
                resolve: Authorize);

            Field<UserType>("addUser",
                arguments: new QueryArguments(
                    new QueryArgument<StringGraphType> {Name = "username"},
                    new QueryArgument<StringGraphType> {Name = "password"}),
                resolve: AddUser);

            Field<UserType>("updateUser",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> {Name = "id"},
                    new QueryArgument<StringGraphType> {Name = "username", DefaultValue = ""},
                    new QueryArgument<StringGraphType> {Name = "password", DefaultValue = ""},
                    new QueryArgument<StringGraphType> {Name = "role", DefaultValue = ""}),
                resolve: UpdateUser);

            Field<SuccessType>("removeUser",
                arguments: new QueryArguments(
                    new QueryArgument<IntGraphType> {Name = "id"}),
                resolve: RemoveUser);
        }

        private User UpdateUser(ResolveFieldContext<object> context)
        {
            int id = context.GetArgument<int>("id");
            string username = context.GetArgument<string>("username");
            string password = context.GetArgument<string>("password");
            string role = context.GetArgument<string>("role");

            try
            {
                User user = _context.Users.Include(_ => _.Role).SingleOrDefault(_ => _.Id == id);
                if (user != null)
                {
                    user.Name = username != "" ? username : user.Name;
                    user.Password = password != "" ? password : user.Password;
                    user.Role = role != "" ? _context.Roles.Single(_ => _.Name == role) : user.Role;
                    _context.SaveChanges();
                    return user;
                }
                throw new Exception($"Could not find user with id '{id}'!");
            }
            catch (Exception e)
            {
                HandleError(context, e);
                return null;
            }
        }

        private SuccessType RemoveUser(ResolveFieldContext<object> context)
        {
            int id = context.GetArgument<int>("id");
            try
            {
                if (_auth.User.Id != id && !_auth.CheckAuthentication("admin"))
                {
                    throw new UnauthorizedAccessException();
                }

                User user = new User {Id = id};
                _context.Users.Attach(user);
                _context.Users.Remove(user);
                _context.SaveChanges();
                return new SuccessType();
            }
            catch (Exception e)
            {
                HandleError(context, e);
                return null;
            }
        }

        private AuthType Authorize(ResolveFieldContext<object> context)
        {
            string username = context.GetArgument<string>("username");
            string password = context.GetArgument<string>("password");

            try
            {
                _auth.Authorize(username, password);
            }
            catch (UnauthorizedAccessException e)
            {
                HandleError(context, e);
                return null;
            }

            return new AuthType();
        }

        private User AddUser(ResolveFieldContext<object> context)
        {
            string username = context.GetArgument<string>("username");
            string password = context.GetArgument<string>("password");

            try
            {
                User newUser = new User
                {
                    Name = username,
                    Password = password,
                    Role = _context.Roles.First(_ => _.Name == "user")
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();
                return newUser;
            }
            catch (Exception e)
            {
                HandleError(context, e);
                return null;
            }
        }



        private void HandleError(ResolveFieldContext<object> context, Exception e)
        {
            GraphQlErrorService.AttachError(context, e);
        }
    }
}