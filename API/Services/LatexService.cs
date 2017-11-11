using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Internal;
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
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                List<string> outputErrors = new List<string>();
                foreach (Match match in Regex.Matches(output, "!.*"))
                {
                    outputErrors.Add(Regex.Replace(match.Value, "!", String.Empty).Trim());
                }
                throw new Exception(outputErrors.Join(" "));
            }
        }
    }
}