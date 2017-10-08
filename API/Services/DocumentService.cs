using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace API.Services
{
    public class DocumentService
    {
        private readonly string _templateDirectory;
        private readonly LatexService _latexService;
        public DocumentService(IConfiguration configuration, LatexService latexService)
        {
            _templateDirectory = configuration["TemplateDirectory"];
            _latexService = latexService;
        }

        public string SaveTemplate(IFormFile templateFile, int id)
        {
            string directory = Path.Combine(_templateDirectory, id.ToString());
            string filePath = Path.Combine(directory, templateFile.FileName);

            Directory.CreateDirectory(directory);

            using (FileStream file = File.Create(filePath))
            {
                templateFile.OpenReadStream().Seek(0, SeekOrigin.Begin);
                templateFile.OpenReadStream().CopyTo(file);
                file.Close();
            }
            return filePath;
        }

        public void DeleteTemplate(string path)
        {
            File.Delete(path);
        }

        public string CompileTemplate(string compressedFile)
        {
            // uncompress file to tmp directory

            // replace template sequences in main.tex

            // start compilation
            _latexService.Compile(compressedFile);

            // convert to datauri
        }
    }
}