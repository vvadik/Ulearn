using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;

namespace uLearn.Web.Models
{
	// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
	public class ApplicationUser : IdentityUser
	{
		public ApplicationUser()
		{
			Registered = DateTime.Now;
		}

		public virtual ICollection<UserExerciseSubmission> Solutions { get; set; }
		public virtual ICollection<UserQuestion> Questions { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime Registered { get; set; }
		public DateTime? LastEdit { get; set; }

		// AvatarUrl is empty if user has no avatar
		public string AvatarUrl { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public string FirstAndLastName
		{
			get { return FirstName + " " + LastName; }
			private set { /* Empty for EF */ }
		}

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public string LastAndFirstName
		{
			get { return LastName + " " + FirstName; }
			private set { /* Empty for EF */ }
		}

		public bool HasAvatar
		{
			get { return ! string.IsNullOrEmpty(AvatarUrl); }
		}

		public string VisibleName
		{
			get
			{
				if (FirstName + LastName != "")
					return (FirstName + " " + LastName).Trim();
				if (!string.IsNullOrEmpty(UserName))
					return UserName.Trim();
				return "Пользователь";
			}
		}
	}
}