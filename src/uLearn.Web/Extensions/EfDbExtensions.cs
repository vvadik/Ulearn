using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using uLearn.Web.Models;

namespace uLearn.Web.Extensions
{
	public static class EfDbExtensions
	{
		public static void RemoveSlideAction<T>(this DbSet<T> dbSet, Guid slideId, string userId) where T : class, ISlideAction
		{
			dbSet.RemoveRange(dbSet.Where(s => s.UserId == userId && s.SlideId == slideId));
		}
	}
}