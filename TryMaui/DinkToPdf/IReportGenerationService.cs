using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryMaui.DinkToPdf
{
    public interface IReportGenerationService
    {
        void GenerateReport(string reportName, string dataXml, string outFilename);
        void GenerateAndOpenReport(string reportName, string dataXml);
    }
}
