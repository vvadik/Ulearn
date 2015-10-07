using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace uLearn.Web.Models
{

	public class SetNewPasswordModel
	{
		[HiddenInput]
		[Required]
		public string RequestId { get; set; }

		[Required(ErrorMessage = "{0} — это обязательное поле")]
		[StringLength(100, ErrorMessage = "{0} должен быть длиной как минимум {2} символов.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Новый пароль")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Подтвердите новый пароль")]
		[System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Подтверждение нового пароля и сам новый пароль отличаются.")]
		public string ConfirmPassword { get; set; }

		public string[] Errors { get; set; }
	}

	public class RestorePasswordModel
	{
		public string UserName { get; set; }
		public string Message { get; set; }
		public bool HasError { get; set; }
	}
}