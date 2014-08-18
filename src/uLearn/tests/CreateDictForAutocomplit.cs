using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace uLearn.tests
{
	class CreateDictForAutocomplit
	{
		public static Dictionary<Type, List<string>> ReturnTypeDictionary = new Dictionary<Type, List<string>>();

		public static int TotalWordCount = 0;

		[Explicit]
		[Test]
		public static void CreateDict()
		{
			var myTypes = new[]
			{
				typeof (int),
				typeof (string),
				typeof (double),
				typeof (Console),
				typeof (Math)
			};

			WalkThroughTypes(myTypes);
		}

		private static void WalkThroughTypes(Type[] myTypes)
		{
			const string methodDict = "dictWithMethods";
			VisitTypesElements(methodDict, GetMethods, myTypes);

			const string propertiesDict = "dictWithProperties";
			VisitTypesElements(propertiesDict, GetProperties, myTypes);

			const string constantDict = "dictWithConstants";
			VisitTypesElements(constantDict, GetConstants, myTypes);
			
			PrintReturnTypeDictionary(myTypes);
			Console.WriteLine("Total word count in all dictionary: {0}", TotalWordCount);
		}

		private static void PrintReturnTypeDictionary(IEnumerable<Type> myTypes)
		{
			Console.WriteLine("var returnTypeDict = [];\n");
			foreach (
				var type in
					ReturnTypeDictionary.Keys.Where(
						type => !type.ToString().Contains("TSource") && !type.ToString().Contains("TResult") && myTypes.Contains(type)))
			{
				Console.WriteLine("returnTypeDict['{0}'] = [{1}];", ToPrettyString(type),
					ToDictString(ReturnTypeDictionary[type].Distinct()));
				Console.WriteLine();
				TotalWordCount += ReturnTypeDictionary[type].Distinct().Count();
			}
		}

		private static void VisitTypesElements(string dictName, Func<Type, IEnumerable<string>> func, IEnumerable<Type> myTypes)
		{
			Console.WriteLine("var {0} = [];", dictName);
			Console.WriteLine();
			foreach (var myType in myTypes)
			{
				var collection = func(myType).ToList();
				TotalWordCount += collection.Count();
				if (!collection.Any())
					continue;
				Console.WriteLine("{0}['{1}'] = [{2}];", dictName, ToPrettyString(myType), ToDictString(collection));
				Console.WriteLine();
			}
			Console.WriteLine();
		}

		private static string ToPrettyString(Type myType)
		{
			var type = myType.ToString().Replace("System.", "");
			if (type == "Int32")
				return "int";
			if (type == "String")
				return "string";
			if (type == "Double")
				return "double";
			return type;
		}

		private static string ToDictString(IEnumerable<string> collection)
		{
			return "'" + string.Join("', '", collection) + "'";
		}

		private static void WriteMyCollection(IEnumerable<KeyValuePair<Type, IEnumerable<string>>> myTypesWithMethods)
		{
			foreach (var typeWithMethod in myTypesWithMethods)
			{
				Console.WriteLine("методы у {0}: \n", typeWithMethod.Key);
				foreach (var method in typeWithMethod.Value)
					Console.WriteLine(method);
				Console.WriteLine();
			}
		}

		private static IEnumerable<string> GetMethods(Type myType)
		{
			var methods = myType.GetMethods().Where(y => y.Name[0] != y.Name.ToLower()[0]).ToArray();
			foreach (var methodInfo in methods)
			{
				var type = methodInfo.ReturnType;
				if (!ReturnTypeDictionary.ContainsKey(type))
					ReturnTypeDictionary[type] = new List<string>();
				ReturnTypeDictionary[type].Add(methodInfo.Name);
			}
			return methods.Select(x => x.Name).Distinct();
		}

		private static IEnumerable<string> GetProperties(Type myType)
		{
			var properties = myType.GetProperties().Where(y => y.Name[0] != y.Name.ToLower()[0]).ToArray();
			foreach (var propertyInfo in properties)
			{
				var type = propertyInfo.PropertyType;
				if (!ReturnTypeDictionary.ContainsKey(type))
					ReturnTypeDictionary[type] = new List<string>();
				ReturnTypeDictionary[type].Add(propertyInfo.Name);
			}
			return properties.Select(x => x.Name).Distinct();
		}

		private static IEnumerable<string> GetConstants(Type myType)
		{
			var fields = myType.GetFields().Where(y => y.Name[0] != y.Name.ToLower()[0]).ToArray();
			foreach (var fieldInfo in fields)
			{
				var type = fieldInfo.FieldType;
				if (!ReturnTypeDictionary.ContainsKey(type))
					ReturnTypeDictionary[type] = new List<string>();
				ReturnTypeDictionary[type].Add(fieldInfo.Name);
			}
			return fields.Select(x => x.Name).Distinct();
		}
	}
}
