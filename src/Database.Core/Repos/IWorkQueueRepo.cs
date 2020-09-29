﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos
{
	public interface IWorkQueueRepo
	{
		Task Add(int queueId, string itemId, string type, int priority = 0);
		Task<WorkQueueItem> Take(int queueId, List<string> types, TimeSpan? timeLimit = null);
		Task Remove(int id);
		Task ReturnToQueue(int id);
	}
}