using System.IO;
using System.Linq;
using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace ManualUtils
{
	public class Users
	{
		public static void PrintCourseAdmins(UlearnDb db)
		{
			var admins = db.CourseRoles.Include(r => r.User).Where(r => r.Role == CourseRoleType.CourseAdmin && (r.IsEnabled ?? false)).ToList();
			var users = admins.GroupBy(a => a.UserId).Select(
				g =>
				{
					var values = g.ToList();
					return new
					{
						Name = values[0].User.VisibleName,
						Email = values[0].User.Email,
						Courses = string.Join(", ", values.Select(e => e.CourseId).ToList()),
						Comments = string.Join(", ", values.Select(e => e.Comment).Where(c => !string.IsNullOrWhiteSpace(c)).ToList())
					};
				}).ToList();
			var lines = users.Select(u => $"{u.Name}\t{u.Email}\t{u.Courses}\t{u.Comments}");
			File.WriteAllLines("admins.txt", lines);
		}
	}
}