using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ulearn.Common.Api.Swagger
{
	public class PolymorphismSchemaFilter<T> : ISchemaFilter
	{
		private readonly Lazy<HashSet<Type>> derivedTypes = new Lazy<HashSet<Type>>(Init);

		public void Apply(OpenApiSchema schema, SchemaFilterContext context)
		{
			var type = context.Type;
			if (!derivedTypes.Value.Contains(type))
				return;

			var clonedSchema = new OpenApiSchema
			{
				Properties = schema.Properties,
				Type = schema.Type,
				Required = schema.Required
			};

			// schemaRegistry.Definitions[typeof(T).Name]; does not work correctly in SwashBuckle
			if(context.SchemaRepository.Schemas.TryGetValue(typeof(T).Name, out OpenApiSchema _))
			{
				schema.AllOf = new List<OpenApiSchema> {
					new OpenApiSchema { Reference = new OpenApiReference { Id = typeof(T).Name, Type = ReferenceType.Schema } },
					clonedSchema
				};
			}

			var assemblyName = Assembly.GetAssembly(type).GetName();
			schema.Discriminator = new OpenApiDiscriminator { PropertyName = "$type" };
			schema.AddExtension("x-ms-discriminator-value", new OpenApiString($"{type.FullName}, {assemblyName.Name}"));

			// reset properties for they are included in allOf, should be null but code does not handle it
			schema.Properties = new Dictionary<string, OpenApiSchema>();
		}

		private static HashSet<Type> Init()
		{
			var abstractType = typeof(T);
			var dTypes = abstractType.GetTypeInfo().Assembly
				.GetTypes()
				.Where(x => abstractType != x && abstractType.IsAssignableFrom(x));

			var result = new HashSet<Type>();

			foreach (var item in dTypes)
				result.Add(item);

			return result;
		}
	}
}