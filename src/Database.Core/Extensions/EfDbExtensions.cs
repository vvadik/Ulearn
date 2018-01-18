using System;
using System.Linq;
using Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Extensions
{
	public static class EfDbExtensions
	{
		public static void RemoveSlideAction<T>(this DbSet<T> dbSet, Guid slideId, string userId) where T : class, ISlideAction
		{
			dbSet.RemoveRange(dbSet.Where(s => s.UserId == userId && s.SlideId == slideId));
		}
	}
}