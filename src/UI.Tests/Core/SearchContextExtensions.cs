using System;
using OpenQA.Selenium;

namespace UI.Tests.Core
{
	public static class SearchContextExtensions
	{
		public static TPageObject Get<TPageObject>(this ISearchContext context)
		{
			/*
			Создаем TPageObject, инжектируем в его поля Browser и Element this и найденный по атрибутам FindByXXX элемент.
			В поля типа массив инжектируем массивы найденных объектов (с помощью метода All ниже)
			В поля типа Lazy<T> инжектируем лямбды ищущие объекты лениво.
			Если что, падаем красиво — пишем лог, сохраняем скриншот.

			Потом сюда можно будет встроить ожидалку появления элемента с таймаутом.
			*/
			throw new NotImplementedException();
		}

		public static TPageObject[] All<TPageObject>(this ISearchContext context)
		{
			// Аналогично Get, только по атрибутам FinByXXX ищем все возможные элементы и создаем TPageObject из каждого.
			throw new NotImplementedException();
		}
	}
}