using System;
using System.Collections.Generic;
using System.Net;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("/api/v1/uploadTemplate")]
    public class UploadTemplateController : Controller
    {
        private readonly AuthService _authService;
        private readonly TemplateService _templateService;
        public UploadTemplateController(AuthService authService, TemplateService templateService)
        {
            _authService = authService;
            _templateService = templateService;
        }

        [HttpPost("{id}")]
        public IActionResult Index(List<IFormFile> template, int id)
        {
            if (template.Count != 1)
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
                    _templateService.UploadTemplate(template[0], id);
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