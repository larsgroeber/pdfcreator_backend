using System;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public struct PasswordRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public struct ResetPasswordRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }

    [Route("/api/v1/password")]
    public class PasswordRevertController : Controller
    {
        private readonly UserService _userService;

        public PasswordRevertController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public IActionResult SendToken([FromBody] PasswordRequest request)
        {
            string name = request.Name;
            string email = request.Email;

            string token = _userService.GetUserToken(name, email);

            if (String.IsNullOrEmpty(token))
            {
                return new EmptyResult();
            }

            EmailService.SendMail(email, "Passwort zurücksetzen",
                "Hi,\n\njemand (hoffentlich Du) hat ein neues Passwort für Dich angefordert.\n" +
                $"Dies ist der Token:\n{token}\n\nDein PDFCreator Backend");

            return new EmptyResult();
        }

        [HttpPost("reset")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            string token = request.Token;
            string newPassword = request.NewPassword;
            string email = request.Email;
            string name = request.Name;

            if (!_userService.ResetPassword(name, email, token, newPassword))
            {
                return StatusCode(500);
            }

            EmailService.SendMail(email, "Passwort zurücksetzen",
                "Hi,\n\njemand (hoffentlich Du) hat Dein Passwort zurückgesetzt.\n" +
                $"\n\nDein PDFCreator Backend");

            return new EmptyResult();
        }
    }
}