using System;
using System.Linq;
using API.Contexts;
using API.Models;
using API.Queries;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserService
    {
        private readonly PDFCreatorContext _context;
        private readonly AuthService _authService;

        public UserService(PDFCreatorContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public User GetUser(ResolveFieldContext<object> context)
        {
            int id = context.GetArgument<int>("id");

            try
            {
                _authService.CheckAuthentication(id);

                return GetUserById(id);
            }
            catch (Exception e)
            {
                HandleError(context, e);
                return null;
            }
        }

        private User GetUserById(int id)
        {
            return _context.Users.Include(_ => _.Role).Include(_ => _.Templates)
                .SingleOrDefault(_ => _.Id == id);
        }

        public object GetActiveUser(ResolveFieldContext<object> context)
        {
            int id = _authService.User.Id;

            try
            {
                return GetUserById(id);
            }
            catch (Exception e)
            {
                HandleError(context, e);
                return null;
            }
        }

        public User AddTemplateToUser(ResolveFieldContext<object> context)
        {
            int idTemplate = context.GetArgument<int>("idTemplate");
            int idUser = context.GetArgument<int>("idUser");

            try
            {
                _authService.CheckAuthentication(idUser);

                User user = _context.Users.Include(_ => _.Templates).SingleOrDefault(_ => _.Id == idUser);
                Template template = _context.Templates.SingleOrDefault(_ => _.Id == idTemplate);

                // TODO
                if (_context.Users.Include(_ => _.Templates).Count(u => u.Templates.Any(t => t.Id == idTemplate)) != 0)
                {
                    throw new Exception($"There is already a user who owns Template with id '{idTemplate}'!");
                }

                if (user != null)
                {
                    user.Templates.Add(template);
                    _context.SaveChanges();
                }
                return user;
            }
            catch (Exception e)
            {
                HandleError(context, e);
                return null;
            }
        }

        public User RemoveTemplateFromUser(ResolveFieldContext<object> context)
        {
            int idTemplate = context.GetArgument<int>("idTemplate");
            int idUser = context.GetArgument<int>("idUser");

            try
            {
                _authService.CheckAuthentication(idUser);

                User user = _context.Users.Include(_ => _.Templates).SingleOrDefault(_ => _.Id == idUser);
                Template template = _context.Templates.SingleOrDefault(_ => _.Id == idTemplate);

                if (user != null)
                {
                    user.Templates.Remove(template);
                    _context.SaveChanges();
                }
                return user;
            }
            catch (Exception e)
            {
                HandleError(context, e);
                return null;
            }
        }

        public User UpdateUser(ResolveFieldContext<object> context)
        {
            int id = context.GetArgument<int>("id");
            string username = context.GetArgument<string>("username");
            string password = context.GetArgument<string>("password");
            string role = context.GetArgument<string>("role");

            try
            {
                _authService.CheckAuthentication(id);
                User user = _context.Users.Include(_ => _.Role).SingleOrDefault(_ => _.Id == id);
                if (user != null)
                {
                    user.Name = username != "" ? username : user.Name;
                    user.Password = password != "" ? AuthService.HashPassword(password) : user.Password;
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

        public SuccessType RemoveUser(ResolveFieldContext<object> context)
        {
            int id = context.GetArgument<int>("id");
            try
            {
                _authService.CheckAuthentication(id);

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

        public AuthType Authorize(ResolveFieldContext<object> context)
        {
            string username = context.GetArgument<string>("username");
            string password = context.GetArgument<string>("password");

            try
            {
                _authService.Authorize(username, password);
            }
            catch (UnauthorizedAccessException e)
            {
                HandleError(context, e);
                return null;
            }

            return new AuthType();
        }


        public User AddUser(ResolveFieldContext<object> context)
        {
            string username = context.GetArgument<string>("username");
            string password = context.GetArgument<string>("password");

            try
            {
                User newUser = new User
                {
                    Name = username,
                    Password = AuthService.HashPassword(password),
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
            Console.WriteLine(e.ToString());
            GraphQlErrorService.AttachError(context, e);
        }
    }
}