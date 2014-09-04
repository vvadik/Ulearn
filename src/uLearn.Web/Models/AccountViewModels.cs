using System.ComponentModel.DataAnnotations;

namespace uLearn.Web.Models
{
	public class ExternalLoginConfirmationViewModel
	{
		[Required(ErrorMessage = "Это обязательное поле")]
		[Display(Name = "Имя (логин)")]
		public string UserName { get; set; }
	}

	public class ManageUserViewModel
	{
		[Required(ErrorMessage = "{0} — это обязательное поле")]
		[DataType(DataType.Password)]
		[Display(Name = "Текущий пароль")]
		public string OldPassword { get; set; }

		[Required(ErrorMessage = "{0} — это обязательное поле")]
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
		[Required(ErrorMessage = "{0} — это обязательное поле")]
		[Display(Name = "Имя (логин)")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "{0} — это обязательное поле")]
		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string Password { get; set; }

		[Display(Name = "Запомнить меня")]
		public bool RememberMe { get; set; }
	}

	public class RegisterViewModel
	{
		[Required(ErrorMessage = "{0} — это обязательное поле")]
		[Display(Name = "Имя (логин)")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "{0} — это обязательное поле")]
		[StringLength(100, ErrorMessage = "{0} должен быть длиной как минимум {2} символов.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Подтвердите пароль")]
		// Bug workaround. Details: http://stackoverflow.com/questions/19978239/custom-errormessage-for-compare-attribute-does-not-work
#pragma warning disable 0618
		// ReSharper disable once CSharpWarnings::CS0618
		[System.Web.Mvc.Compare("Password", ErrorMessage = "Подтверждение пароля и пароль отличаются.")]
#pragma warning restore 0618
		public string ConfirmPassword { get; set; }
	}
}