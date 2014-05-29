using System.ComponentModel.DataAnnotations;

namespace uLearn.Web.Models
{
	public class ExternalLoginConfirmationViewModel
	{
		[Required]
		[Display(Name = "Имя (логин)")]
		public string UserName { get; set; }
	}

	public class ManageUserViewModel
	{
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Текущий пароль")]
		public string OldPassword { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "{0} должен быть длиной как минимум {2} символов.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Новый пароль")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Подтвердите новый пароль")]
		[Compare("NewPassword", ErrorMessage = "Подтверждение нового пароля и сам новый пароль отличаются.")]
		public string ConfirmPassword { get; set; }
	}

	public class LoginViewModel
	{
		[Required]
		[Display(Name = "Имя (логин)")]
		public string UserName { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string Password { get; set; }

		[Display(Name = "Запомнить меня")]
		public bool RememberMe { get; set; }
	}

	public class RegisterViewModel
	{
		[Required]
		[Display(Name = "Имя (логин)")]
		public string UserName { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "{0} должен быть длиной как минимум {2} символов.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Подтвердите пароль")]
		[Compare("Password", ErrorMessage = "Подтверждение пароля и пароль отличаются.")]
		public string ConfirmPassword { get; set; }
	}
}