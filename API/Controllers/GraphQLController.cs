using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using API.Models;
using API.Mutations;
using API.Queries;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("/api/v1")]
    public class GraphQlController : Controller
    {
        private readonly RootQuery _query;
        private readonly RootMutation _mutation;
        private readonly IServiceProvider _serviceProvider;

        public GraphQlController(RootQuery rootQuery, RootMutation rootMutation, IServiceProvider serviceProvider)
        {
            _query = rootQuery;
            _mutation = rootMutation;
            _serviceProvider = serviceProvider;
        }

        [HttpPost("graphql")]
        public async Task<IActionResult> Index()
        {
            string query;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                query = await reader.ReadToEndAsync();
            }

            return await ResolveQuery(query);
        }

        private async Task<IActionResult> ResolveQuery(string query)
        {
            // Pure man's DI, graphQL for .Net does not support build in IoC
            AuthType.ServiceProvider = _serviceProvider;
            TemplateType.ServiceProvider = _serviceProvider;
            MutationType.ServiceProvider = _serviceProvider;
            QueryType.ServiceProvider = _serviceProvider;

            var schema = new Schema
            {
                Query = _query,
                Mutation = _mutation,
            };

            try
            {
                var result = await new DocumentExecuter().ExecuteAsync(_ =>
                {
                    _.Schema = schema;
                    _.Query = query;
                }).ConfigureAwait(false);
                return Json(result);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Unauthorized");
                return StatusCode(401);
            }
            catch (Exception)
            {
                Console.WriteLine("Error");
                return StatusCode(500);
            }
        }
    }
}