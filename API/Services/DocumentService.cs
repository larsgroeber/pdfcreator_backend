using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace API.Services
{
    public struct Document
    {
        public string Template { get; set; }
        public string DataUri { get; set; }
        public List<TemplateField> TemplateFields { get; set; }
    }

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

        public Document CompileTemplate(string compressedFile, List<TemplateField> inputTemplateFields)
        {
            Document document = new Document();
            if (compressedFile == "")
            {
                return document;
            }

            string documentDirectory = CreateTmpDirector(compressedFile);

            // uncompress file to tmp directory
            UncompressToDirectory(compressedFile, documentDirectory);

            string pathToMainTex = documentDirectory + "/main.tex";

            if (!File.Exists(pathToMainTex))
            {
                throw new Exception("Cannot compile the document without a main.tex file!");
            }

            {
                document.Template = File.ReadAllText(pathToMainTex);
                Console.WriteLine(document.Template);
                TemplateParser.TemplateParser templateParser = new TemplateParser.TemplateParser();
                document.TemplateFields = templateParser.GetInputFields(document.Template);

                _logger.LogInformation("Replacing template fields");
                document.Template = templateParser.ReplaceFields(document.Template, document.TemplateFields);
                File.WriteAllText(pathToMainTex, document.Template);
                Console.WriteLine(document.Template);
            }

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
            document.DataUri = "data:application/pdf;base64," + Convert.ToBase64String(content);

//            _logger.LogInformation($"Deleting directory {documentDirectory}.");
//            Directory.Delete(documentDirectory, true);

            return document;
        }

        private void UncompressToDirectory(string compressedFile, string documentDirectory)
        {
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
        }

        private string CreateTmpDirector(string compressedFile)
        {
            string tmpDirectory = "/tmp/pdfcreator";

            Directory.CreateDirectory(tmpDirectory);
            // TODO add templatename/id here
            return Path.Combine(tmpDirectory, "test");
        }

        private bool IsZipFile(string fileName)
        {
            return Path.GetExtension(fileName) == ".zip";
        }
    }
}