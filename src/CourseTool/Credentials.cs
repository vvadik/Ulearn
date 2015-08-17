using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using uLearn;

namespace uLearn.CourseTool
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

		public static Credentials GetCredentials(string dir)
		{
			Credentials credentials;
			if (File.Exists(dir + "/credentials.xml"))
				credentials = new FileInfo(dir + "/credentials.xml").DeserializeXml<Credentials>();
			else
			{
				Console.WriteLine("Enter email:");
				var email = Console.ReadLine();
				Console.WriteLine("Enter password:");
				var password = Utils.GetPass();
				credentials = new Credentials(email, password);
				File.WriteAllText(dir + "/credentials.xml", credentials.XmlSerialize());
			}
			return credentials;
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
}