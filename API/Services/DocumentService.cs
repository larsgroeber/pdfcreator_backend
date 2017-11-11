using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using API.Models;
using API.Utils.TemplateParser;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace API.Services
{
    public struct Document
    {
        public string Template { get; set; }
        public string DataUri { get; set; }
        public string pdfPath { get; set; }
        public List<TemplateField> TemplateFields { get; set; }
    }

    public class DocumentService
    {
        private readonly string _templateDirectory;
        private readonly CompileService _compileService;
        private readonly ILogger<DocumentService> _logger;

        public DocumentService(IConfiguration configuration,
            ILogger<DocumentService> logger,
            CompileService compileService)
        {
            _templateDirectory = configuration["TemplateDirectory"];
            _compileService = compileService;
            _logger = logger;
        }

        public string SaveTemplate(IFormFile templateFile, int id)
        {
            string directory = Path.Combine(_templateDirectory, id.ToString());
            string filePath = Path.Combine(directory, templateFile.FileName);

            if (!CompileService.IsZipFile(filePath))
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

        public Document CompileTemplate(Template template, List<TemplateField> inputTemplateFields)
        {
            return CompileSingle(template, inputTemplateFields);
        }

        public Document CompileTemplate(Template template, List<List<TemplateField>> inputTemplateFields)
        {
            return CompileMultiple(template, inputTemplateFields);
        }

        private Document CompileSingle(Template template, List<TemplateField> inputTemplateFields)
        {
            Document document = new Document();
            if (template.Path == "")
            {
                return document;
            }

            return _compileService.CompileSingle(template, inputTemplateFields);
        }

        private Document CompileMultiple(Template template, List<List<TemplateField>> inputTemplateFields)
        {
            Document document = new Document();
            if (template.Path == "")
            {
                return document;
            }

            return _compileService.CompileMultiple(template, inputTemplateFields);
        }
    }
}