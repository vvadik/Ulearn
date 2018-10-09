using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Database.Models;
using Serilog;
using Ulearn.Web.Api.Models.Responses.Groups;

namespace Ulearn.Web.Api.Controllers.Groups
{
	public abstract class BaseGroupController : BaseController
	{
		protected BaseGroupController(ILogger logger, WebCourseManager courseManager, UlearnDb db)
			: base(logger, courseManager, db)
		{
		}
		
		protected GroupInfo BuildGroupInfo(Group group, int? membersCount=null, IEnumerable<GroupAccess> accesses=null)
		{
			if (group == null)
				throw new ArgumentNullException(nameof(group));

			return new GroupInfo
			{
				Id = group.Id,
				CreateTime = group.CreateTime,
				Name = group.Name,
				Owner = BuildShortUserInfo(group.Owner),
				InviteHash = group.InviteHash,
				IsInviteLinkEnabled = group.IsInviteLinkEnabled,
				IsArchived = group.IsArchived,
				
				IsManualCheckingEnabled = group.IsManualCheckingEnabled,
				IsManualCheckingEnabledForOldSolutions = group.IsManualCheckingEnabledForOldSolutions,
				DefaultProhibitFurtherReview = group.DefaultProhibitFutherReview,
				CanStudentsSeeGroupProgress = group.CanUsersSeeGroupProgress,
				
				StudentsCount = membersCount,
				Accesses = accesses?.Select(BuildGroupAccessesInfo).ToList(),
			};
		}
		
		protected GroupAccessesInfo BuildGroupAccessesInfo(GroupAccess access)
		{
			if (access == null)
				throw new ArgumentNullException(nameof(access));
			
			return new GroupAccessesInfo
			{
				User = BuildShortUserInfo(access.User),
				AccessType = access.AccessType,
				// GrantedBy = BuildShortUserInfo(access.GrantedBy),
				GrantTime = access.GrantTime
			};
		}
	}
}