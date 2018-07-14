using System;
using System.Linq;
using System.Security.Cryptography;
using API.Contexts;
using API.Models;
using API.Shared;
using GraphQL;
using GraphQL.Types;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace API.Services
{
    public class AuthService
    {
        private readonly PDFCreatorContext _context;
        private readonly string _secret;
        public User User { set; get; }
        public string Token { get; set; }

        public AuthService(IConfiguration configuration, PDFCreatorContext context)
        {
            _context = context;
            _secret = configuration["JWT-Secret"];
        }


        public void Authorize(string username, string password)
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            User = _context.Users.Include(_ => _.Role).FirstOrDefault(_ => _.Name == username);

            if (User != null && BCrypt.Net.BCrypt.Verify(password, User.Password))
            {
                string token = encoder.Encode(User, _secret);
                Token = token;
                return;
            }
            throw new Exceptions.WrongLoginException("Credentials incorrect!");
        }

        public void CheckJwt(string token)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                var json = decoder.Decode(token, _secret, verify: true);
                User = JsonConvert.DeserializeObject<User>(json);
                Console.WriteLine("Authorized user: " + User);
            }
            catch (TokenExpiredException)
            {
                throw new UnauthorizedAccessException("Token expired!");
            }
            catch (SignatureVerificationException)
            {
                throw new UnauthorizedAccessException("Signature invalid!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 11);
        }

        public void CheckAuthentication(int id = -1, string role = "admin")
        {
            if (User.Id != id && User.Role.Name != "admin" && User.Role.Name != role)
            {
                throw new UnauthorizedAccessException();
            }
        }

        public static string GenerateRandomToken()
        {
            Guid g = Guid.NewGuid();
            return Convert.ToBase64String(g.ToByteArray()).Replace('/', '_');
        }
    }
}