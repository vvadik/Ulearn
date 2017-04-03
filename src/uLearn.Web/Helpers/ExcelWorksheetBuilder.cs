using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace uLearn.Web.Helpers
{
	public class ExcelWorksheetBuilder
	{
		private readonly ExcelWorksheet worksheet;
		private int currentRow;
		private int currentColumn;
		private readonly List<Action<ExcelStyle>> styleRules = new List<Action<ExcelStyle>>();

		public int ColumnsCount;

		public ExcelWorksheetBuilder(ExcelWorksheet worksheet)
		{
			this.worksheet = worksheet;
			currentRow = 1;
			currentColumn = 1;
			ColumnsCount = 1;
		}

		public void AddCell(object value, int colspan=1)
		{
			if (colspan < 1)
				return;

			var cell = worksheet.Cells[currentRow, currentColumn];
			cell.Value = value;
			foreach (var styleRule in styleRules)
				styleRule(cell.Style);
			if (colspan > 1)
				worksheet.Cells[currentRow, currentColumn, currentRow, currentColumn + colspan - 1].Merge = true;
			currentColumn += colspan;
			ColumnsCount = Math.Max(ColumnsCount, currentColumn);
		}
		
		public void GoToNewLine()
		{
			currentRow += 1;
			currentColumn = 1;
		}

		public void AddStyleRule(Action<ExcelStyle> styleFunction)
		{
			styleRules.Add(styleFunction);
		}

		public void PopStyleRule()
		{
			styleRules.RemoveAt(styleRules.Count - 1);
		}
	}
}