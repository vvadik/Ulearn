using System;
using System.Security.Cryptography;
using System.Text;

namespace uLearnToEdx
{
	public class Credentials
	{
		public string Email;
		public string Password;

		public Credentials()
		{
		}

		public Credentials(string email, string password)
		{
			Email = email;
			SetPassword(password);
		}

		public string GetPassword()
		{
			return Encoding.UTF8.GetString(ProtectedData.Unprotect(Convert.FromBase64String(Password), null, DataProtectionScope.CurrentUser));
		}

		public void SetPassword(string password)
		{
			Password = Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes(password), null, DataProtectionScope.CurrentUser));
		}
	}

	public class Config
	{
		public string Hostname;
		public int Port;
		public string Organization;
		public string CourseNumber;
		public string CourseRun;
		public string VideoJson;
		public string ExerciseUrl;
		public string SolutionsUrl;
		public string LtiId;

		public string[] AdvancedModules;
		public string[] LtiPassports;
	}
}
