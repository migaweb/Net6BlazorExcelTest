using BlazorApp.WASM.Shared.Contracts;
using BlazorApp.WASM.Shared.Entities;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;

namespace BlazorApp.Services.Services
{
  public class ArticleExcelReader : IExcelReader<Article>
  {
    public async Task<IList<Article>> Read(Stream stream)
    {
      var articles = new List<Article>();
      DataTable dt = new DataTable();

      using (SpreadsheetDocument doc = SpreadsheetDocument.Open(stream, false))
      {
        Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();

        //Get the Worksheet instance.
        Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;
        IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>();

        foreach (Row row in rows)
        {
          if (row.RowIndex.Value == 1)
          {
            foreach (Cell cell in row.Descendants<Cell>())
            {
              dt.Columns.Add(GetValue(doc, cell));
            }
          }
          else
          {
            //Add rows to DataTable.
            dt.Rows.Add();
            int i = 0;
            foreach (Cell cell in row.Descendants<Cell>())
            {
              dt.Rows[dt.Rows.Count - 1][i] = GetValue(doc, cell);
              i++;
            }
          }
        }
      }
      //    if (col == 12) article.Description = worksheet.Cells[row, col].Value.ToString();
      //    if (col == 2) article.SupplierId = worksheet.Cells[row, col].Value.ToString();
      //    if (col == 5) article.SubGroupId = Convert.ToInt32(worksheet.Cells[row, col].Value.ToString());
      //    if (col == 4) article.MainGroupId = Convert.ToInt32(worksheet.Cells[row, col].Value.ToString());
      //    if (col == 3) article.Price = Convert.ToDecimal(worksheet.Cells[row, col].Value.ToString());
      //    if (col == 7) article.Ean = worksheet.Cells[row, col].Value.ToString();

      int count = 0;

      foreach (DataRow row in dt.Rows)
      {
        var article = new Article();

        article.Description = row.Field<string>(11);
        article.Ean = row.Field<string>(6);
        article.Id = count++;
        article.MainGroupId = Convert.ToInt32(row.Field<string>(3));
        article.SubGroupId = Convert.ToInt32(row.Field<string>(4));
        article.Price = Convert.ToDecimal(row.Field<string>(2));
        article.SupplierId = row.Field<string>(1);

        articles.Add(article);
      }

      return articles;
    }

    private string GetValue(SpreadsheetDocument doc, Cell cell)
    {
      string value = cell.CellValue.InnerText;
      if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
      {
        return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
      }
      return value;
    }
  }
}
