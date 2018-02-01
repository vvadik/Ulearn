using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Vostok.Hosting;

namespace AntiPlagiarism.Web.Database
{
	/* TODO (andgein): is this class needed? */
	public class AntiPlagiarismDbFactory : IDesignTimeDbContextFactory<AntiPlagiarismDb>
	{
		public AntiPlagiarismDb CreateDbContext(string[] args)
		{
			var host = EntryPoint.BuildVostokHost();
			var hostingEnvironment = host.HostingEnvironment;
			
			var optionsBuilder = new DbContextOptionsBuilder<AntiPlagiarismDb>();
			optionsBuilder.UseSqlServer(hostingEnvironment.Configuration["database"]);
			
			return new AntiPlagiarismDb(optionsBuilder.Options);
		}
	}
}