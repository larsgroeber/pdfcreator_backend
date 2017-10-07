using System;
using System.Linq;
using API.Contexts;
using API.Models;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class TemplateService
    {
        private readonly PDFCreatorContext _context;
        private readonly AuthService _authService;

        public TemplateService(PDFCreatorContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public object GetTemplate(ResolveFieldContext<object> context)
        {
            int id = context.GetArgument<int>("id");
            try
            {
                Template template = GetTemplateById(id);
                CheckAuthenticationForTemplate(template);

                if (template != null)
                {
                    return template;
                }
                throw new Exception($"Could not find Template with id '{id}'!");
            }
            catch (Exception e)
            {
                HandleError(context, e);
                return null;
            }
        }

        private Template GetTemplateById(int id)
        {
            return _context.Templates.SingleOrDefault(_ => _.Id == id);
        }

        public Template UpdateTemplate(ResolveFieldContext<object> context)
        {
            int id = context.GetArgument<int>("id");
            string name = context.GetArgument<string>("name");
            string description = context.GetArgument<string>("description");

            try
            {
                Template template = GetTemplateById(id);

                if (template != null)
                {
                    CheckAuthenticationForTemplate(template.Id);
                    template.Name = name != "" ? name : template.Name;
                    template.Description = description != "" ? description : template.Description;
                    _context.SaveChanges();
                    return template;
                }
                throw new Exception($"Could not find Template with id '{id}'!");
            }
            catch (Exception e)
            {
                HandleError(context, e);
                return null;
            }
        }

        public SuccessType RemoveTemplate(ResolveFieldContext<object> context)
        {
            int id = context.GetArgument<int>("id");
            try
            {
                CheckAuthenticationForTemplate(id);



                //Template template = new Template {Id = id};
                //_context.Templates.Attach(template);
                Template template = _context.Templates.SingleOrDefault(_ => _.Id == id);
                _context.Remove(template);
                _context.SaveChanges();
                return new SuccessType();
            }
            catch (Exception e)
            {
                HandleError(context, e);
                return null;
            }
        }

        public Template AddTemplate(ResolveFieldContext<object> context)
        {
            string name = context.GetArgument<string>("name");
            string description = context.GetArgument<string>("description");
            int idUser = context.GetArgument<int>("idUser");

            try
            {
                Template newTemplate = new Template
                {
                    Name = name,
                    Description = description,
                    Path = "Does not exist!"
                };


                if (idUser > 0)
                {
                    _authService.CheckAuthentication(idUser);
                    User user = _context.Users.Include(_ => _.Templates).SingleOrDefault(_ => _.Id == idUser);
                    user.Templates.Add(newTemplate);
                }
                else
                {
                    _context.Templates.Add(newTemplate);
                }

                _context.SaveChanges();
                return newTemplate;
            }
            catch (Exception e)
            {
                HandleError(context, e);
                return null;
            }
        }

        private void CheckAuthenticationForTemplate(int id)
        {
            User user = GetUserWhoOwnsTemplate(id);

            if (user != null)
            {
                _authService.CheckAuthentication(user.Id);
            }
            else
            {
                Console.WriteLine($"Template of id '{id}' has no user!");
                _authService.CheckAuthentication(-1);
            }
        }

        private void CheckAuthenticationForTemplate(Template template)
        {
            User user = GetUserWhoOwnsTemplate(template.Id);

            if (user != null)
            {
                _authService.CheckAuthentication(user.Id);
            }
            else
            {
                Console.WriteLine($"Template of id '{template.Id}' has no user!");
                _authService.CheckAuthentication(-1);
            }
        }

        private void HandleError(ResolveFieldContext<object> context, Exception e)
        {
            GraphQlErrorService.AttachError(context, e);
        }

        private User GetUserWhoOwnsTemplate(int id)
        {
            return _context.Users.Include(_ => _.Templates)
                .SingleOrDefault(_ => _.Templates.Any(t => t.Id == id));
        }
    }
}