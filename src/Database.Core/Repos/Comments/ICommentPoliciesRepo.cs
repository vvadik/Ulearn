using System.Threading.Tasks;
using Database.Models;

namespace Database.Repos.Comments
{
	public interface ICommentPoliciesRepo
	{
		Task<CommentsPolicy> GetCommentsPolicyAsync(string courseId);
		Task SaveCommentsPolicyAsync(CommentsPolicy policy);
	}
}