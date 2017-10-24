using API.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // BuildWebHost(args).Run();
            TemplateParseService t = new TemplateParseService();
            t.GetFields("<$ Test $> not not <<Test2>>");
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}