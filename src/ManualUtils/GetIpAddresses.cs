using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Database;
using Microsoft.EntityFrameworkCore;

namespace ManualUtils
{
	public static class GetIpAddresses
	{
		// Шаблон для выгрузки ip-адресов студентов курсов, не состоящих ни в одной группе, заходивших за 6 месяцев.
		public static void Run(UlearnDb db, int lastMonthCount, string[] courses, bool isNotMembersOfGroups)
		{
			var time = DateTime.Now.AddMonths(-lastMonthCount);
			var membersOfGroups = new HashSet<string>(db.GroupMembers.Select(gm => gm.UserId).Distinct());
			var visits = db.Visits
				.Where(v =>
					courses.Contains(v.CourseId)
					&& v.Timestamp > time
					&& v.IpAddress != null
					&& v.User.EmailConfirmed
					&& v.User.Email != null
				)
				.GroupBy(v => v.UserId)
				.Select(kvp => kvp.OrderByDescending(v => v.Timestamp).FirstOrDefault())
				.ToList();
			if(isNotMembersOfGroups)
				visits = visits.Where(v => !membersOfGroups.Contains(v.UserId))
				.ToList();
			File.WriteAllText("students.txt", "UserName\tFirstName\tLastName\tEmail\tIpAddress");
			File.WriteAllLines("students.txt", visits.Select(v => $"{v.User.UserName}\t{v.User.FirstName}\t{v.User.LastName}\t{v.User.Email}\t{v.IpAddress}"));
		}
	}
}