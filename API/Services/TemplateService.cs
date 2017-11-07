using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using API.Contexts;
using API.Models;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Services
{
    public class TemplateService
    {
        private readonly PDFCreatorContext _context;
        private readonly AuthService _authService;
        private readonly DocumentService _documentService;

        public TemplateService(PDFCreatorContext context, AuthService authService, DocumentService documentService)
        {
            _context = context;
            _authService = authService;
            _documentService = documentService;
        }

        public Template GetTemplate(ResolveFieldContext<object> context)
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
                HandleError(e);
                return null;
            }
        }

        public Template GetTemplateById(int id)
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
                    template.Description = description;
                    _context.SaveChanges();
                    return template;
                }
                throw new Exception($"Could not find Template with id '{id}'!");
            }
            catch (Exception e)
            {
                HandleError(e);
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
                HandleError(e);
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
                    Path = ""
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
                HandleError(e);
                return null;
            }
        }

        public void UploadTemplate(IFormFile file, int templateId)
        {
            CheckAuthenticationForTemplate(templateId);

            Template template = GetTemplateById(templateId);

            if (template != null)
            {
                string oldPath = template.Path;

                string newPath = _documentService.SaveTemplate(file, templateId);
                template.Path = newPath;
                template.DownloadToken = AuthService.GenerateRandomToken();
                _context.SaveChanges();

                if (oldPath != "" && oldPath != newPath)
                {
                    _documentService.DeleteTemplate(oldPath);
                }
                return;
            }
            throw new Exception($"Could not find template with id '{templateId}'!");
        }

        public Document Compile(ResolveFieldContext<Template> context)
        {
            int id = context.Source.Id;
            string templateFieldsEncoded = context.GetArgument<string>("fields");

            if (string.IsNullOrEmpty(templateFieldsEncoded))
            {
                templateFieldsEncoded = "[]";
            }

            try
            {
                CheckAuthenticationForTemplate(id);

                Template template = GetTemplateById(id);

                string templateFieldsJson = Encoding.UTF8.GetString(Convert.FromBase64String(templateFieldsEncoded));

                List<List<TemplateField>> templateFields = JsonConvert.DeserializeObject<List<List<TemplateField>>>(templateFieldsJson);

                foreach (List<TemplateField> templateField in templateFields)
                {
                    foreach (TemplateField field in templateField)
                    {
                        Console.WriteLine(field.ToString());
                    }
                }

                if (templateFields?.Count > 1)
                {
                    return _documentService.CompileTemplate(template, templateFields);
                }
                if (templateFields?.Count == 1)
                {
                    return _documentService.CompileTemplate(template, templateFields[0]);
                }
                return _documentService.CompileTemplate(template, new List<TemplateField>());
            }
            catch (Exception e)
            {
                HandleError(e);
                return new Document();
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
                _authService.CheckAuthentication();
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
                _authService.CheckAuthentication();
            }
        }

        private void HandleError(Exception e)
        {
            Console.WriteLine(e.ToString());
            GraphQlErrorService.Add(e);
        }

        private User GetUserWhoOwnsTemplate(int id)
        {
            return _context.Users.Include(_ => _.Templates)
                .SingleOrDefault(_ => _.Templates.Any(t => t.Id == id));
        }


        public Template GetTemplateByToken(string templateToken)
        {
            Template template = _context.Templates.SingleOrDefault(_ => _.DownloadToken == templateToken);
            return template;
        }
    }
}