using System.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace API.Services
{
    public class LatexService
    {
        private readonly string _latexCommand;
        public LatexService(IConfiguration configuration)
        {
            _latexCommand = configuration["LatexCommand"];
        }

        public void Compile(string directory)
        {
            Process process = new Process();
            process.StartInfo.FileName = _latexCommand;
            process.StartInfo.Arguments = directory;
            process.Start();
        }
    }
}