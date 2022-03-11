using DinkToPdf;
using DinkToPdf.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace TryMaui.DinkToPdf
{
    public class PdfGenerationService : IReportGenerationService
    {
        private readonly IConverter converter;

        public PdfGenerationService()
        {
            converter = new BasicConverter(new PdfTools());
        }

        public void GenerateAndOpenReport(string reportName, string dataXml)
        {
            string tempFile = Path.GetTempFileName();
            string tempDir = Path.GetDirectoryName(tempFile);
            string tempFilename = Path.GetFileName(tempFile);
            tempFilename = $"Report_{tempFilename}";
            tempFilename = Path.ChangeExtension(tempFilename, "pdf");
            tempFile = Path.Combine(tempDir, tempFilename);

            GenerateReport(reportName, dataXml, tempFile);

            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = tempFile
            };
            Process.Start(startInfo);
        }

        public void GenerateReport(string reportName, string dataXml, string outFilename)
        {
            string reportHtml = GetReportHtml(reportName);

            reportHtml = RenderXmlData(reportHtml, dataXml);

            var doc = new HtmlToPdfDocument
            {
                GlobalSettings =
                {
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Out = outFilename
                },
                Objects =
                {
                    new ObjectSettings
                    {
                        HtmlContent = reportHtml,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };
            converter.Convert(doc);
        }

        private string GetReportHtml(string reportName)
        {
            string reportHtml;
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream($"TryMaui.ReportTemplates.{reportName}.html"))
            using (var reader = new StreamReader(stream))
            {
                reportHtml = reader.ReadToEnd();
            }

            return reportHtml;
        }

        private string RenderXmlData(string reportHtml, string dataXml)
        {
            reportHtml = $@"
                <xsl:stylesheet version=""1.0"" xmlns:xsl=""http://www.w3.org/1999/XSL/Transform"">
                    <xsl:output method=""html"" omit-xml-declaration=""yes"" />
                    <xsl:template match=""/"">
                        {reportHtml}
                    </xsl:template>
                </xsl:stylesheet>
            ";

            dataXml = $@"
                <root>
                    {dataXml}
                </root>
            ";

            using (var srXsl = new StringReader(reportHtml))
            using (var xrXsl = XmlReader.Create(srXsl))
            using (var srXml = new StringReader(dataXml))
            using (var xrXml = XmlReader.Create(srXml))
            {
                var xslt = new XslCompiledTransform();
                xslt.Load(xrXsl);

                using (var sw = new StringWriter())
                using (var xw = XmlWriter.Create(sw, xslt.OutputSettings))
                {
                    xslt.Transform(xrXml, xw);
                    reportHtml = sw.ToString();
                }
            }

            return reportHtml;
        }
    }
}
