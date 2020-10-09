using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using Database.Models;
using Ulearn.Common;
using Ulearn.Core;
using Ulearn.Core.Courses.Slides.Blocks;
using Ulearn.Core.Courses.Slides.Exercises.Blocks;
using Ulearn.Web.Api.Models.Common;

namespace Ulearn.Web.Api.Models.Responses.SlideBlocks
{
	[DataContract]
	public class CsProjectExerciseBlockResponse : IApiSlideBlock
	{
		[DefaultValue(false)]
		[DataMember(Name = "hide", EmitDefaultValue = false)]
		public bool Hide { get; set; }

		[DataMember(Name = "language")]
		public Language? Language { get; set; }

		[DataMember(Name = "type")]
		public string Type { get; set; } = "code";
		
		
		[DataMember(Name = "hints")]
		public string[] Hints { get; set; }
		
		
		[DataMember(Name = "exerciseInitialCode")]
		public string ExerciseInitialCode { get; set; }
		
		[DataMember(Name = "exerciseCode")]
		public string ExerciseCode { get; set; }
		
		[DataMember(Name = "submissions")]
		public List<SubmissionInfo> Submissions{ get; set; }
		
		public CsProjectExerciseBlockResponse(CsProjectExerciseBlock codeBlock, IEnumerable<UserExerciseSubmission> submissions)
		{
			Hints = codeBlock.Hints.ToArray();
			ExerciseInitialCode = codeBlock.ExerciseInitialCode;
			Language = codeBlock.Language;
			Submissions = submissions
				.Select(s =>
				{
					var reviews = s
						.GetAllReviews()
						.Select(r => new ReviewInfo
						{
							Comment = r.Comment,
							Author = new ShortUserInfo
							{
								Id = r.Author.Id,
								Login = r.Author.UserName,
								Email = r.Author.Email,
								FirstName = r.Author.FirstName ?? "",
								LastName = r.Author.LastName ?? "",
								VisibleName = r.Author.VisibleName,
								AvatarUrl = r.Author.AvatarUrl,
								Gender = r.Author.Gender,
							},
							AddingTime = r.AddingTime,
							FinishLine = r.FinishLine,
							FinishPosition = r.FinishPosition,
							StartLine = r.StartLine,
							StartPosition = r.StartPosition,
						})
						.ToList();
					return new SubmissionInfo
					{
						Id = s.Id,
						Code = s.SolutionCode.Text,
						Timestamp = s.Timestamp,
						Reviews = reviews,
						Output = s.AutomaticChecking.Output.Text,
						Points = s.AutomaticChecking.Points ?? 0f,
					};
				})
				.ToList();
		}

		public CsProjectExerciseBlockResponse()
		{
		}
	}
}