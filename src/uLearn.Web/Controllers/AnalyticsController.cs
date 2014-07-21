using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class AnalyticsController : Controller
	{
		private readonly AnalyticsTable analyticsTable = new AnalyticsTable();
	}
}