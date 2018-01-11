using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AntiPlagiarism.Web.Database.Models
{
	public enum SnippetType : short
	{
		TokensKindsOnly = 1,
		TokensKindsAndValues = 2,
	}
	
	public class Snippet
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }
		
		[Required]
		public int TokensCount { get; set; }
		
		[Required]
		public SnippetType SnippetType { get; set; } 
		
		[Required]
		public int Hash { get; set; }
	}
}