using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using uLearn.Web.DataContexts;
using uLearn.Web.Extensions;
using uLearn.Web.Models;

namespace uLearn.Web.Controllers
{
	public class CertificatesController : Controller
	{
		private readonly CertificatesRepo certificatesRepo = new CertificatesRepo();
		private readonly ULearnUserManager userManager = new ULearnUserManager();
		private readonly CourseManager courseManager = WebCourseManager.Instance;

		public CertificatesController()
		{
		}

		[AllowAnonymous]
		public ActionResult Index(string userId="")
		{
			if (string.IsNullOrEmpty(userId) && User.Identity.IsAuthenticated)
				userId = User.Identity.GetUserId();

			if (string.IsNullOrEmpty(userId))
				return HttpNotFound();

			var certificates = certificatesRepo.GetUserCertificates(userId);
			var coursesTitles = courseManager.GetCourses().ToDictionary(c => c.Id, c => c.Title);

			return View("List", new UserCertificatesViewModel
			{
				User = userManager.FindById(userId),
				Certificates = certificates,
				CoursesTitles = coursesTitles,
			});
		}

		public ActionResult CertificateById(Guid certificateId)
		{
			var redirect = this.GetRedirectToUrlWithTrailingSlash();
			if (redirect != null)
				return RedirectPermanent(redirect);

			var certificate = certificatesRepo.FindCertificateById(certificateId);
			if (certificate == null)
				return HttpNotFound();

			var course = courseManager.GetCourse(certificate.Template.CourseId);

			certificatesRepo.EnsureCertificateTemplateIsUnpacked(certificate.Template);
			
			var renderedCertificate = certificatesRepo.RenderCertificate(certificate, course);

			return View("Certificate", new CertificateViewModel
			{
				Course = course,
				RenderedCertificate = renderedCertificate,
			});
		}

		public ActionResult CertificateFile(Guid certificateId, string path)
		{
			var certificate = certificatesRepo.FindCertificateById(certificateId);
			if (certificate == null)
				return HttpNotFound();

			if (path.Contains(".."))
				return HttpNotFound();

			return RedirectPermanent($"/Certificates/{certificate.Template.ArchiveName}/{path}");
		}
	}

	public class UserCertificatesViewModel
	{
		public ApplicationUser User { get; set; }
		public List<Certificate> Certificates { get; set; }
		public Dictionary<string, string> CoursesTitles { get; set; }
	}

	public class CertificateViewModel
	{
		public Course Course { get; set; }
		public string RenderedCertificate { get; set; }
	}
}