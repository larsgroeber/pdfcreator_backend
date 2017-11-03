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
                return new JsonResult(new
                {
                    result = "None"
                });
            }

//            EmailService.SendMail(email, "Passwort zurücksetzen",
//                "Hi,\n\njemand (hoffentlich Du) hat ein neues Passwort für Dich angefordert.\n" +
//                $"Dies ist der Token:\n{token}\n\nDein PDFCreator Backend");

            return new JsonResult(new
            {
                result = "OK"
            });
        }

        [HttpPost("reset")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            string token = request.Token;
            string newPassword = request.NewPassword;

            string result = _userService.ResetPassword(token, newPassword);
            if (String.IsNullOrEmpty(result))
            {
                return StatusCode(500);
            }

//            EmailService.SendMail(email, "Passwort zurücksetzen",
//                "Hi,\n\njemand (hoffentlich Du) hat Dein Passwort zurückgesetzt.\n" +
//                $"\n\nDein PDFCreator Backend");

            return new JsonResult(new
            {
                result
            });
        }
    }
}