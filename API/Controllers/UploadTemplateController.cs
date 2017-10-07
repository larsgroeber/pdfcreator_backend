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
        public UploadTemplateController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("{id}")]
        public IActionResult Index(List<IFormFile> template, int id)
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader != "")
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                Console.WriteLine(token);
                try
                {
                    _authService.CheckJwt(token);
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e);
                    return StatusCode(401);
                }
                Console.WriteLine(template.Count);
                Console.WriteLine(template[0].FileName);
                Console.WriteLine(id);
                return new EmptyResult();
            }
            else
            {
                return StatusCode(401);
            }
        }
    }
}