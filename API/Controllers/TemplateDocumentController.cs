using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("/api/v1/document")]
    public class TemplateDocumentController : Controller
    {
        private readonly AuthService _authService;
        private readonly TemplateService _templateService;
        public TemplateDocumentController(AuthService authService, TemplateService templateService)
        {
            _authService = authService;
            _templateService = templateService;
        }

        [HttpGet("{token}")]
        public IActionResult Download(string templateToken)
        {
            Template template = _templateService.GetTemplateByToken(templateToken);

            return File(new FileStream(template.Path, FileMode.Open), "application/zip");
        }

        [HttpPost("{id}")]
        public IActionResult Index(List<IFormFile> file, int id)
        {
            Console.WriteLine(file.Count);
            if (file.Count != 1)
            {
                return StatusCode(400);
            }

            string authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader != "")
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                // check token and authorization
                try
                {
                    _authService.CheckJwt(token);
                    _templateService.UploadTemplate(file[0], id);
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e);
                    return StatusCode(401);
                }

                return new EmptyResult();
            }
            return StatusCode(401);
        }
    }
}