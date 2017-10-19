using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace API.Services
{
    public class DocumentService
    {
        private readonly string _templateDirectory;
        private readonly LatexService _latexService;
        private readonly ILogger<DocumentService> _logger;
        public DocumentService(IConfiguration configuration, LatexService latexService, ILogger<DocumentService> logger)
        {
            _templateDirectory = configuration["TemplateDirectory"];
            _latexService = latexService;
            _logger = logger;
        }

        public string SaveTemplate(IFormFile templateFile, int id)
        {
            string directory = Path.Combine(_templateDirectory, id.ToString());
            string filePath = Path.Combine(directory, templateFile.FileName);

            if (!IsZipFile(filePath))
            {
                throw new Exception($"File {filePath} is not a zip file.");
            }

            _logger.LogInformation($"Saving template with id {id} to {directory}.");

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
            if (Directory.Exists(documentDirectory))
            {
                Directory.Delete(documentDirectory, true);
            }

            if (!IsZipFile(compressedFile))
            {
                throw new Exception($"File {compressedFile} is not a zip file.");
            }
            _logger.LogInformation($"Extracting to {documentDirectory}.");

            ZipFile.ExtractToDirectory(compressedFile, documentDirectory);

            // replace template sequences in main.tex

            // start compilation
            _logger.LogInformation($"Compiling template.");

            _latexService.Compile(documentDirectory);

            // convert to datauri
            string pdf = Path.Combine(documentDirectory, "main.pdf");
            if (!File.Exists(pdf))
            {
                throw new Exception($"File {pdf} does not exist!");
            }

            var content = File.ReadAllBytes(pdf);
            string datauri = "data:application/pdf;base64," + Convert.ToBase64String(content);

            _logger.LogInformation($"Deleting directory {documentDirectory}.");
            Directory.Delete(documentDirectory, true);

            return datauri;
        }

        private bool IsZipFile(string fileName)
        {
            return Path.GetExtension(fileName) == ".zip";
        }
    }
}