using System;
using System.Linq;
using System.IO;

using Documents.Models.Entities;
using System.Text;
using HandlebarsDotNet;
using System.Dynamic;
using System.Collections.Generic;

using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace Documents.Services.Printing
{
    public static class DocumentPrinter
    {
        private static dynamic FillData(Document docData, Template docTemplate)
        {
            dynamic data = new ExpandoObject();

            foreach (var item in docTemplate.TemplateItems)
            {
                if (item is TemplateField field)
                {
                    string value = docData.DocumentDataItems.First(i => i.FieldId == field.Id).Value;
                    ((IDictionary<string, object>)data).Add(field.Name.Replace(" ", "_"), value);
                }
                else if (item is TemplateTable table)
                {
                    foreach (var col in table.TemplateFields)
                        foreach (var cell in docData.DocumentDataItems.Where(i => i.FieldId == col.Id))
                            ((IDictionary<string, object>)data).Add($"{col.Name.Replace(" ", "_")}-{cell.Row}", cell.Value);
                }
            }
            return data;
        }


        public static TextWriter GetHtml(Document docData, Template docTemplate)
        {
            var start = DateTime.Now;

            TextWriter writer = new StringWriter();
            TextReader reader = new StreamReader(new MemoryStream(docTemplate.HtmlTemplate.Data),
                                                 Encoding.UTF8);
            var htmlTemplate = Handlebars.Compile(reader);
            htmlTemplate(writer, FillData(docData, docTemplate));

            reader.Close();
            LastHtmlProcessTime = (DateTime.Now - start).TotalSeconds;
            return writer;
        }

        public static string GetHtmlString(Document docData, Template docTemplate)
        {
            var start = DateTime.Now;

            TextWriter writer = new StringWriter();
            TextReader reader = new StreamReader(new MemoryStream(docTemplate.HtmlTemplate.Data),
                                                 Encoding.UTF8);
          
            var htmlTemplate = Handlebars.Compile(reader);
            htmlTemplate(writer, FillData(docData, docTemplate));

            string result = writer.ToString();
            reader.Close();
            writer.Close();

            LastHtmlProcessTime = (DateTime.Now - start).TotalSeconds;
            return result;
        }

        public static byte[] GetPdfBytes(Document docData, Template docTemplate, string pageFormat = "A4")
        {
            var start = DateTime.Now;
            TextWriter writer = GetHtml(docData, docTemplate);
            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                PdfGenerator.GeneratePdf(writer.ToString(), GetPageSize(pageFormat)).Save(ms);
                data = ms.ToArray();
                writer.Close();
            }
            LastPdfProcessTime = (DateTime.Now - start).TotalSeconds;
            return data;
        }


        public static MemoryStream GetPdf(Document docData, Template docTemplate, string pageFormat = "A4")
        {
            var start = DateTime.Now;
            TextWriter writer = GetHtml(docData, docTemplate);
            MemoryStream ms = new MemoryStream();
            PdfGenerator.GeneratePdf(writer.ToString(), GetPageSize(pageFormat)).Save(ms);
            writer.Close();
            LastPdfProcessTime = (DateTime.Now - start).TotalSeconds;
            return ms;
        }


        public static PdfSharp.PageSize GetPageSize(string format)
        {
            switch (format.ToUpper())
            {
                case "A0": return PdfSharp.PageSize.A0;
                case "A1": return PdfSharp.PageSize.A1;
                case "A2": return PdfSharp.PageSize.A2;
                case "A3": return PdfSharp.PageSize.A3;
                case "A5": return PdfSharp.PageSize.A5;
                case "B0": return PdfSharp.PageSize.B0;
                case "B1": return PdfSharp.PageSize.B0;
                case "B2": return PdfSharp.PageSize.B0;
                case "B3": return PdfSharp.PageSize.B0;
                case "B4": return PdfSharp.PageSize.B0;
                case "B5": return PdfSharp.PageSize.B0;
                case "LETTER": return PdfSharp.PageSize.Letter;
                default: return PdfSharp.PageSize.A4;
            }
        }

        public static double LastHtmlProcessTime { get; set; } = -1;
        public static double LastPdfProcessTime { get; set; } = -1;
    }
}
