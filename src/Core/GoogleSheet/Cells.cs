using System;

namespace Ulearn.Core.GoogleSheet
{
	public interface ISheetTableCell
	{
	}

	public class StringSheetTableCell : ISheetTableCell
	{
		public readonly string Value;

		public StringSheetTableCell(string value)
		{
			Value = value;
		}
	}

	public class DateSheetTableCell : ISheetTableCell
	{
		public readonly DateTime Value;

		public DateSheetTableCell(DateTime value)
		{
			Value = value;
		}
	}

	public class NumberSheetTableCell : ISheetTableCell
	{
		public readonly double Value;

		public NumberSheetTableCell(double value)
		{
			Value = value;
		}
	}
}