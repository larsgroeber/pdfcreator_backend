using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
//            TemplateParser.TemplateParser t = new TemplateParser.TemplateParser();
//            t.GetInputFields("<$ Test $> not not <<Test2 ; Kommentar test test >>");
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}