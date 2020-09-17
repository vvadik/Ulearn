using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Database;

namespace ManualUtils
{
	public static class GetIpAddresses
	{
		// Шаблон для выгрузки ip-адресов студентов курсов, не состоящих ни в одной группе, заходивших за 6 месяцев.
		public static void Run(UlearnDb db, int lastMonthCount, string[] courses, bool isNotMembersOfGroups, bool onlyRegisteredFrom)
		{
			var time = DateTime.Now.AddMonths(-lastMonthCount);
			var membersOfGroups = new HashSet<string>(db.GroupMembers.Select(gm => gm.UserId).Distinct());
			var data = db.Visits
				.Where(v =>
					courses.Contains(v.CourseId)
					&& v.Timestamp > time
					&& v.IpAddress != null
					&& v.User.EmailConfirmed
					&& v.User.Email != null
					&& (!onlyRegisteredFrom || v.User.Registered > time)
				)
				.Select(v => v.UserId)
				.Distinct()
				.Select(id => new {
						User = db.Users.FirstOrDefault(t => t.Id == id),
						IpAddress = db.Visits.Where(t => t.UserId == id && t.IpAddress != null).OrderByDescending(t => t.Timestamp).FirstOrDefault().IpAddress
					}
				)
				.ToList();
			if (isNotMembersOfGroups)
				data = data.Where(v => !membersOfGroups.Contains(v.User.Id))
				.ToList();
			File.WriteAllText("students.txt", "UserName\tFirstName\tLastName\tEmail\tIpAddress");
			File.WriteAllLines("students.txt", data.Select(v => $"{v.User.UserName}\t{v.User.FirstName}\t{v.User.LastName}\t{v.User.Email}\t{v.IpAddress}"));
		}
	}
}