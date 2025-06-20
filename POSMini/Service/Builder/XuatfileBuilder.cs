using System;
using System.Data;
using ClosedXML.Excel;
using Xceed.Words.NET;
using POSMini.Models;
using Xceed.Document.NET;

namespace POSMini.Service.Builder
{
    public interface IThongKeExporterBuilder
    {
        void SetFileName(string fileName);
        void SetTitle(string title);
        void SetThongKeData(ThongKe thongKe, DataTable chiTiet);
        void BuildHeader();
        void BuildBody();
        void BuildFooter();
        string GetResult();
    }

    
    public class ExcelExporterBuilder : IThongKeExporterBuilder
    {
        private string fileName;
        private string title;
        private ThongKe thongKe;
        private DataTable chiTiet;
        private XLWorkbook workbook;
        private IXLWorksheet worksheet;

        public void SetFileName(string fileName) => this.fileName = fileName;
        public void SetTitle(string title) => this.title = title;

        public void SetThongKeData(ThongKe thongKe, DataTable chiTiet)
        {
            this.thongKe = thongKe;
            this.chiTiet = chiTiet;
        }

        public void BuildHeader()
        {
            workbook = new XLWorkbook();
            worksheet = workbook.Worksheets.Add("ThongKe");

            worksheet.Cell("A1").Value = title;
            worksheet.Cell("A2").Value = $"Từ ngày: {thongKe.TuNgay:dd/MM/yyyy}";
            worksheet.Cell("B2").Value = $"Đến ngày: {thongKe.DenNgay:dd/MM/yyyy}";
            worksheet.Cell("C2").Value = $"Loại: {thongKe.LoaiThongKe}";
        }

        public void BuildBody()
        {
            int row = 4;

            for (int i = 0; i < chiTiet.Columns.Count; i++)
                worksheet.Cell(row, i + 1).Value = chiTiet.Columns[i].ColumnName;

            for (int i = 0; i < chiTiet.Rows.Count; i++)
            {
                for (int j = 0; j < chiTiet.Columns.Count; j++)
                {
                    var value = chiTiet.Rows[i][j];
                    worksheet.Cell(i + row + 1, j + 1).Value = value?.ToString();
                }
            }
        }

        public void BuildFooter()
        {
            int lastRow = worksheet.LastRowUsed().RowNumber() + 2;
            worksheet.Cell(lastRow, 1).Value = "Tổng doanh thu:";
            worksheet.Cell(lastRow, 2).Value = thongKe.TongDoanhThu;

            worksheet.Cell(lastRow + 1, 1).Value = "Tổng vốn hàng bán:";
            worksheet.Cell(lastRow + 1, 2).Value = thongKe.TongVonHangBan;

            worksheet.Cell(lastRow + 2, 1).Value = "Lợi nhuận:";
            worksheet.Cell(lastRow + 2, 2).Value = thongKe.TongDoanhThu - thongKe.TongVonHangBan;
        }

        public string GetResult()
        {
            workbook.SaveAs(fileName);
            return fileName;
        }
    }

   
    public class WordExporterBuilder : IThongKeExporterBuilder
    {
        private string fileName;
        private string title;
        private ThongKe thongKe;
        private DataTable chiTiet;
        private DocX document;

        public void SetFileName(string fileName) => this.fileName = fileName;
        public void SetTitle(string title) => this.title = title;

        public void SetThongKeData(ThongKe thongKe, DataTable chiTiet)
        {
            this.thongKe = thongKe;
            this.chiTiet = chiTiet;
        }

        public void BuildHeader()
        {
            document = DocX.Create(fileName);
            document.InsertParagraph(title)
                    .FontSize(16).Bold()
                    .Alignment = Alignment.center;

            document.InsertParagraph($"Từ ngày: {thongKe.TuNgay:dd/MM/yyyy}  Đến ngày: {thongKe.DenNgay:dd/MM/yyyy}")
                    .SpacingAfter(5);

            document.InsertParagraph($"Loại thống kê: {thongKe.LoaiThongKe}")
                    .Italic().SpacingAfter(10);
        }

        public void BuildBody()
        {
            var table = document.AddTable(chiTiet.Rows.Count + 1, chiTiet.Columns.Count);

            table.Alignment = Alignment.center;

            
            for (int j = 0; j < chiTiet.Columns.Count; j++)
            {
                table.Rows[0].Cells[j].Paragraphs[0].Append(chiTiet.Columns[j].ColumnName).Bold();
            }

           
            for (int i = 0; i < chiTiet.Rows.Count; i++)
            {
                for (int j = 0; j < chiTiet.Columns.Count; j++)
                {
                    var text = chiTiet.Rows[i][j]?.ToString();
                    table.Rows[i + 1].Cells[j].Paragraphs[0].Append(text);
                }
            }

            document.InsertParagraph().SpacingAfter(5);
            document.InsertTable(table);
        }

        public void BuildFooter()
        {
            document.InsertParagraph().SpacingAfter(10);
            document.InsertParagraph($"Tổng doanh thu: {thongKe.TongDoanhThu:N0} VNĐ");
            document.InsertParagraph($"Tổng vốn hàng bán: {thongKe.TongVonHangBan:N0} VNĐ");
            document.InsertParagraph($"Lợi nhuận: {(thongKe.TongDoanhThu - thongKe.TongVonHangBan):N0} VNĐ");
        }

        public string GetResult()
        {
            document.Save();
            return fileName;
        }
    }

   
    public class ThongKeExporterDirector
    {
        private readonly IThongKeExporterBuilder builder;

        public ThongKeExporterDirector(IThongKeExporterBuilder builder)
        {
            this.builder = builder;
        }

        public void Export(string fileName, string title, ThongKe thongKe, DataTable chiTiet)
        {
            builder.SetFileName(fileName);
            builder.SetTitle(title);
            builder.SetThongKeData(thongKe, chiTiet);
            builder.BuildHeader();
            builder.BuildBody();
            builder.BuildFooter();
            builder.GetResult();
        }
    }
}
