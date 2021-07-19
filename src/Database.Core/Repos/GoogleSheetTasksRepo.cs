using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Database.Repos
{
	public class GoogleSheetTasksRepo
	{
		private readonly UlearnDb db;

		public GoogleSheetTasksRepo(UlearnDb db)
		{
			this.db = db;
		}

		public async Task<List<List<int>>> GetGroupsIds()
		{ 
			throw new NotImplementedException();
		}
	}
}