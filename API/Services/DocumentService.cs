using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
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
            if (compressedFile == "")
            {
                return "";
            }

            string tmpDirectory = "/tmp/pdfcreator";
            Directory.CreateDirectory(tmpDirectory);
            string documentDirectory = Path.Combine(tmpDirectory, "test");

            // uncompress file to tmp directory
            Directory.Delete(documentDirectory, true);
            ZipFile.ExtractToDirectory(compressedFile, documentDirectory);

            // replace template sequences in main.tex

            // start compilation
            _latexService.Compile(documentDirectory);

            // convert to datauri
            string pdf = Path.Combine(documentDirectory, "main.pdf");
            if (!File.Exists(pdf))
            {
                throw new Exception($"File {pdf} does not exist!");
            }

            var content = File.ReadAllBytes(pdf);
            string datauri = "data:application/pdf;base64," + Convert.ToBase64String(content);

            return datauri;
        }
    }
}