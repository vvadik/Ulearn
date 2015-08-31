using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace uLearn.CourseTool
{
	public class CredentialsArray
	{
		public Credentials[] Credentials;
	}

	public class Credentials
	{
		[XmlAttribute("profile")]
		public string Profile;

		public string Email;
		public string Password;

		public Credentials()
		{
		}

		public Credentials(string profile, string email, string password)
		{
			Profile = profile;
			Email = email;
			SetPassword(password);
		}

		public static Credentials GetCredentialsFromIo(string profile)
		{
			Console.WriteLine("Enter email:");
			var email = Console.ReadLine();
			Console.WriteLine("Enter password:");
			var password = Utils.GetPass();
			return new Credentials(profile, email, password);
		}

		public static Credentials GetCredentials(string dir, string profile)
		{
			Credentials credentials;
			if (File.Exists(dir + "/credentials.xml"))
			{
				var cred = new FileInfo(dir + "/credentials.xml").DeserializeXml<CredentialsArray>();
				if (cred.Credentials.Any(x => x.Profile == profile))
					credentials = cred.Credentials.First(x => x.Profile == profile);
				else
				{
					credentials = GetCredentialsFromIo(profile);
					cred.Credentials = new List<Credentials>(cred.Credentials) { credentials }.ToArray();
					File.WriteAllText(dir + "/credentials.xml", cred.XmlSerialize());
				}
			}
			else
			{
				credentials = GetCredentialsFromIo(profile);
				File.WriteAllText(dir + "/credentials.xml", new CredentialsArray { Credentials = new[] { credentials } }.XmlSerialize());
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