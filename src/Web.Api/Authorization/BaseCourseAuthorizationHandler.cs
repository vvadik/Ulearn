using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace Ulearn.Web.Api.Authorization
{
	public class BaseCourseAuthorizationHandler<T> : AuthorizationHandler<T> where T : IAuthorizationRequirement
	{
		protected readonly ILogger logger;

		public BaseCourseAuthorizationHandler(ILogger logger)
		{
			this.logger = logger;
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, T requirement)
		{
			throw new System.NotImplementedException();
		}
		
		/* Find `course_id` arguments in request. Try to get course_id in following order:
		 * route data (/groups/<course_id>/)
		 * query string (/groups/?course_id=<course_id>)
		 * JSON request body ({"course_id": <course_id>})
		 */
		protected async Task<string> GetCourseIdFromRequestAsync(AuthorizationFilterContext mvcContext)
		{
			/* 1. Route data */
			var routeData = mvcContext.RouteData;
			if (routeData.Values["courseId"] is string courseIdFromRoute)
				return courseIdFromRoute;

			/* 2. Bind models */
			var mvcOptions = mvcContext.HttpContext.RequestServices.GetRequiredService<IOptions<MvcOptions>>().Value;
			var parameterBinder = mvcContext.HttpContext.RequestServices.GetRequiredService<ParameterBinder>();
			var modelBinderFactory = mvcContext.HttpContext.RequestServices.GetRequiredService<IModelBinderFactory>();
			var modelMetadataProvider = mvcContext.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();

			var controllerContext = new ControllerContext(mvcContext)
			{
				ValueProviderFactories = new CopyOnWriteList<IValueProviderFactory>(mvcOptions.ValueProviderFactories.ToArray())
			};

			var valueProvider = await CompositeValueProvider.CreateAsync(controllerContext).ConfigureAwait(false);
			var parameters = controllerContext.ActionDescriptor.Parameters;
			
			var parameterBindingInfo = GetParameterBindingInfo(
				modelBinderFactory,
				modelMetadataProvider,
				controllerContext.ActionDescriptor,
				mvcOptions);
			
			for (var i = 0; i < parameters.Count; i++)
			{
				var parameter = parameters[i];
				if (!typeof(ICourseAuthorizationParameters).IsAssignableFrom(parameter.ParameterType))
					continue;
				
				var bindingInfo = parameterBindingInfo[i];
				var modelMetadata = bindingInfo.ModelMetadata;

				if (!modelMetadata.IsBindingAllowed)
					continue;

				var model = await parameterBinder.BindModelAsync(
					controllerContext,
					bindingInfo.ModelBinder,
					valueProvider,
					parameter,
					modelMetadata,
					value: null
				).ConfigureAwait(false);

				if (!model.IsModelSet)
					continue;

				var courseAuthorizationParameters = model.Model as ICourseAuthorizationParameters;
				if (courseAuthorizationParameters == null)
					continue;

				return courseAuthorizationParameters.CourseId;
			}

			logger.Error("Can't find `courseId` parameter in request for checking course role requirement. You should inherit your parameters models from ICourseAuthorizationParameters.");
			return null;
		}
		
		/* See https://stackoverflow.com/questions/53751938/invoking-default-model-binder-for-query-parameters-from-an-authorizationhandler for details */
		private static BinderItem[] GetParameterBindingInfo(IModelBinderFactory modelBinderFactory, IModelMetadataProvider modelMetadataProvider, ControllerActionDescriptor actionDescriptor, MvcOptions mvcOptions)
        {
            var parameters = actionDescriptor.Parameters;
            if (parameters.Count == 0)
            {
                return null;
            }

            var parameterBindingInfo = new BinderItem[parameters.Count];
            for (var i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];

                ModelMetadata metadata;
                if (mvcOptions.AllowValidatingTopLevelNodes &&
                    modelMetadataProvider is ModelMetadataProvider modelMetadataProviderBase &&
                    parameter is ControllerParameterDescriptor controllerParameterDescriptor)
                {
                    // The default model metadata provider derives from ModelMetadataProvider
                    // and can therefore supply information about attributes applied to parameters.
                    metadata = modelMetadataProviderBase.GetMetadataForParameter(controllerParameterDescriptor.ParameterInfo);
                }
                else
                {
                    // For backward compatibility, if there's a custom model metadata provider that
                    // only implements the older IModelMetadataProvider interface, access the more
                    // limited metadata information it supplies. In this scenario, validation attributes
                    // are not supported on parameters.
                    metadata = modelMetadataProvider.GetMetadataForType(parameter.ParameterType);
                }

                var binder = modelBinderFactory.CreateBinder(new ModelBinderFactoryContext
                {
                    BindingInfo = parameter.BindingInfo,
                    Metadata = metadata,
                    CacheToken = parameter,
                });

                parameterBindingInfo[i] = new BinderItem(binder, metadata);
            }

            return parameterBindingInfo;
        }
		
		private struct BinderItem
		{
			public BinderItem(IModelBinder modelBinder, ModelMetadata modelMetadata)
			{
				ModelBinder = modelBinder;
				ModelMetadata = modelMetadata;
			}
	
			public IModelBinder ModelBinder { get; }
	
			public ModelMetadata ModelMetadata { get; }
		}
	}
}