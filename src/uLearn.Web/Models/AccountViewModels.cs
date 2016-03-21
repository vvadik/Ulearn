using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace uLearn.Web.Models
{
	public class ExternalLoginConfirmationViewModel
	{
		[Required(ErrorMessage = "{0} есть у каждого пользователя")]
		[Display(Name = "Логин")]
		public string UserName { get; set; }
	}

	public class ManageUserViewModel
	{
		[Required(ErrorMessage = "{0} обязателен")]
		[DataType(DataType.Password)]
		[Display(Name = "Текущий пароль")]
		public string OldPassword { get; set; }

		[Required(ErrorMessage = "{0} нужно обязательно ввести")]
		[StringLength(100, ErrorMessage = "{0} не может быть короче {2} символов", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Новый пароль")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Ещё раз")]
		[System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Пароли отличаются")]
		public string ConfirmPassword { get; set; }
	}

	public class LoginViewModel
	{
		[Required(ErrorMessage = "Ты забыл логин?")]
		[Display(Name = "Логин")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Введи пароль")]
		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string Password { get; set; }

		[Display(Name = "Запомнить меня")]
		public bool RememberMe { get; set; }
	}

	public class RegisterViewModel
	{
		[Required(ErrorMessage = "{0} должен быть у каждого пользователя")]
		[Display(Name = "Логин")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "{0} обязателен")]
		[StringLength(100, ErrorMessage = "{0} не может быть короче {2} символов", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Ещё раз")]
		// Bug workaround. Details: http://stackoverflow.com/questions/19978239/custom-errormessage-for-compare-attribute-does-not-work
#pragma warning disable 0618
		// ReSharper disable once CSharpWarnings::CS0618
		[System.Web.Mvc.Compare("Password", ErrorMessage = "Пароли отличаются")]
#pragma warning restore 0618
		public string ConfirmPassword { get; set; }
		
		public string ReturnUrl { get; set; }

		public bool RegistrationFinished { get; set; }
	}

	public class UserViewModel
	{
		[HiddenInput]
		public string UserId { get; set; }

		public bool HasPassword { get; set; }

		[Required(ErrorMessage = "{0} должен быть у каждого пользователя")]
		[Display(Name = "Логин")]
		public string Name { get; set; }

		[Display(Name = "Учебная группа")]
		public string GroupName { get; set; }

		[Required(ErrorMessage = "{0} — это обязательное поле")]
		[StringLength(100, ErrorMessage = "{0} не может быть короче {2} символов", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Ещё раз")]
		// Bug workaround. Details: http://stackoverflow.com/questions/19978239/custom-errormessage-for-compare-attribute-does-not-work
#pragma warning disable 0618
		// ReSharper disable once CSharpWarnings::CS0618
		[System.Web.Mvc.Compare("Password", ErrorMessage = "Пароли отличаются")]
#pragma warning restore 0618
		public string ConfirmPassword { get; set; }

		[Display(Name = "Имя")]
		public string FirstName { get; set; }

		[Display(Name = "Фамилия")]
		public string LastName { get; set; }

		[Display(Name = "Почта")]
		public string Email { get; set; }

	}

	public class LtiUserViewModel
	{
		[Display(Name = "Имя")]
		public string FirstName { get; set; }

		[Display(Name = "Фамилия")]
		public string LastName { get; set; }

		[Display(Name = "Группа и вуз")]
		public string GroupName { get; set; }

		[Display(Name = "Email")]
		public string Email { get; set; }
	}

	public class UserInfoModel
	{
		public ApplicationUser User { get; set; }
		public Course[] Courses { get; private set; }

		public UserInfoModel(ApplicationUser user, Course[] courses)
		{
			User = user;
			Courses = courses;
		}
	}
}