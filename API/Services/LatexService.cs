using System;
using System.Diagnostics;
using System.IO;
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
            string mainFile = Path.Combine(directory, "main.tex");
            string arguments = $"-halt-on-error -interaction=nonstopmode -output-directory {directory} {mainFile}";

            Process process = new Process();
            process.StartInfo.FileName = _latexCommand;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.WorkingDirectory = directory;
            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception("An error occured while compiling the template!");
            }
        }
    }
}