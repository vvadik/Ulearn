using System;
using Microsoft.AspNetCore.Mvc;

namespace Ulearn.Web.Api.Models.Parameters.Notifications
{
	public class NotificationsCountParameters : ApiParameters
	{
		[FromQuery(Name = "last_timestamp")]
		public DateTime? LastTimestamp { get; set; }
	}
}