using System;

namespace ManualUtils
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			//CampusRegistration();
			//GetIps();
		}

		private static void CampusRegistration()
		{
			ManualUtils.CampusRegistration.Run(courseId: "Campus1920", groupId: 803, slideWithRegistrationQuiz: new Guid("67bf45bd-bebc-4bde-a705-c16763b94678"), false);
		}

		private static void GetIps()
		{
			// Для получения городов см. geoip.py
			// Где взять GeoLite2-City.mmdb читай в GeoLite2-City.mmdb.readme.txt
			var courses = new[] { "BasicProgramming", "BasicProgramming2", "Linq", "complexity", "CS2" };
			GetIpAddresses.Run(lastMonthCount: 6, courses, isNotMembersOfGroups: true);
		}
	}
}