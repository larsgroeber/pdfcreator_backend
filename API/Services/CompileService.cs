using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using API.Models;
using API.TemplateParser;
using Microsoft.Extensions.Logging;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace API.Services
{
    public class CompileService
    {
        private LatexService _latexService;
        private readonly ILogger<CompileService> _logger;

        public CompileService(LatexService latexService, ILogger<CompileService> logger)
        {
            _latexService = latexService;
            _logger = logger;
        }

        public Document CompileSingle(Template template, List<TemplateField> inputTemplateFields)
        {
            string tmpDirectory = CreateTmpDirectory();
            string documentDirectory = Path.Combine(tmpDirectory, template.Id.ToString());
            Document document = CompileDocument(template, inputTemplateFields, documentDirectory);

            document.DataUri = ConvertPdfToDataUri(document.pdfPath);

            return document;
        }

        private Document CompileDocument(Template template, List<TemplateField> inputTemplateFields, string documentDirectory)
        {
            Document document = new Document();

            UncompressToDirectory(template.Path, documentDirectory);
            AssembleDocument(documentDirectory, inputTemplateFields, ref document);

            _logger.LogInformation("Compiling template.");
            _latexService.Compile(documentDirectory);

            document.pdfPath = Path.Combine(documentDirectory, "main.pdf");
            return document;
        }

        private void AssembleDocument(string documentDirectory, List<TemplateField> inputTemplateFields, ref Document document)
        {
            string pathToMainTex = documentDirectory + "/main.tex";

            if (!File.Exists(pathToMainTex))
            {
                throw new Exception("Cannot compile the document without a main.tex file!");
            }

            document.Template = File.ReadAllText(pathToMainTex);

            TemplateParser.TemplateParser templateParser = new TemplateParser.TemplateParser();
            // Concat inputTemplateFields with parsed templateFields and choose the ones where
            // replacement is filled
            document.TemplateFields = templateParser.GetInputFields(document.Template)
                .Concat(inputTemplateFields)
                .GroupBy(_ => _.Content)
                .Select(_ => _.OrderByDescending(__ => __.Replacement).First())
                .ToList();

            if (inputTemplateFields.Count > 0)
            {
                _logger.LogInformation("Replacing template fields");
                TemplateAssembler templateAssembler = new TemplateAssembler(document.Template);
                document.Template = templateAssembler.Assemble(inputTemplateFields);
                File.WriteAllText(pathToMainTex, document.Template);
            }
        }

        private string ConvertPdfToDataUri(string pdf)
        {
            if (!File.Exists(pdf))
            {
                throw new Exception($"File {pdf} does not exist!");
            }

            var content = File.ReadAllBytes(pdf);
            return "data:application/pdf;base64," + Convert.ToBase64String(content);
        }

        public Document CompileMultiple(Template template, List<List<TemplateField>> inputTemplateFields)
        {
            Document document = new Document();
            List<string> pdfPaths = new List<string>();
            string tmpDirectory = CreateTmpDirectory();
            string documentParentDirectory = Path.Combine(tmpDirectory, template.Id.ToString());
            Directory.CreateDirectory(documentParentDirectory);

            for (int index = 0; index < inputTemplateFields.Count; index++)
            {
                string documentDirectory = Path.Combine(documentParentDirectory, index.ToString());
                document = CompileDocument(template, inputTemplateFields[index], documentDirectory);

                pdfPaths.Add(document.pdfPath);
            }

            PdfDocument pdfDocument = ConcatPdFs(pdfPaths);
            string resultPdf = Path.Combine(documentParentDirectory, "result.pdf");
            pdfDocument.Save(resultPdf);

            document.DataUri = ConvertPdfToDataUri(resultPdf);

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

        private string CreateTmpDirectory()
        {
            string tmpDirectory = "/tmp/pdfcreator";
            Directory.CreateDirectory(tmpDirectory);
            return tmpDirectory;
        }

        public static bool IsZipFile(string fileName)
        {
            return Path.GetExtension(fileName) == ".zip";
        }

        private PdfDocument ConcatPdFs(List<string> pdfPaths)
        {
            PdfDocument pdfDocument = new PdfDocument();
            pdfPaths.ForEach(path =>
            {
                CopyPages(PdfReader.Open(path, PdfDocumentOpenMode.Import), pdfDocument);
            });
            return pdfDocument;
        }

        private void CopyPages(PdfDocument open, PdfDocument pdfDocument)
        {
            for (int i = 0; i < open.PageCount; i++)
            {
                pdfDocument.AddPage(open.Pages[i]);
            }
        }
    }
}