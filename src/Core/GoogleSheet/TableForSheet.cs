using System;

namespace Ulearn.Core.GoogleSheet
{
    public class TableForSheet
    {
        public readonly ISheetTableCell[,] Cells;
        public readonly int SheetNumber;
        public readonly int Height;
        public readonly int Width;

        public TableForSheet(int height, int width, int sheetNumber)
        {
            Height = height;
            Width = width;
            SheetNumber = sheetNumber;
            Cells = new ISheetTableCell[height, width];
        }

        public void AddCell(int row, int column, string value)
        {
            Cells[row, column] = new StringSheetTableCell(value);
        }
        
        public void AddCell(int row, int column, double value)
        {
            Cells[row, column] = new NumberSheetTableCell(value);
        }

        public void AddCell(int row, int column, DateTime value)
        {
            Cells[row, column] = new DateSheetTableCell(value);
        }
    }
}