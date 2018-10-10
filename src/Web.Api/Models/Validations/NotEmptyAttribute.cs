using System.ComponentModel.DataAnnotations;

namespace Ulearn.Web.Api.Models.Validations
{
	public class NotEmptyAttribute : ValidationAttribute
	{
		public override bool IsValid(object value)
		{
			var stringValue = value as string;
			return !string.IsNullOrEmpty(stringValue);
		}
	}
}