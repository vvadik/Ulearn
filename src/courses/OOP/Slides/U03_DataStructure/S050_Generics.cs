using uLearn;

namespace OOP.Slides.U03_DataStructure
{
	[Slide("Задача. Generics", "{BB5A40EC-B9FE-48E8-A441-3F368C65DF12}")]
	public class S050_Generics
	{
		/*
		Решая две предыдущие задачи можно было заметить, что для данных структур данных не важно, 
		над какими типами они работают. Ассоциативный массив, в котором ключами и значениями являются строки, ничем не отличается от ассоциативного массива,
		в котором ключами и значениями являются целые числа. Вместо отдельной реализации под каждую пару типов ключ-значение, 
		желательно иметь одну общую реализацию ассоциативного массива, для которого можно задавать необходимые типы для ключа и значения.

		Для решения этой проблемы в языках программирования есть так называемые обобщенные типы (Generic types).
		Они позволяют описать ассоциативный массив следующим образом (здесь TKey — тип ключа, а TValue — тип значения):
		*/

		// ReSharper disable InconsistentNaming
		public interface SimpleMap<TKey, TValue>
		{
			TValue get(TKey key);
			TValue put(TKey key, TValue value);
			TValue remove(TKey key);
		}
		// ReSharper restore InconsistentNaming

		/*
		В данном примере TKey и TValue называются типами-параметрами, а SimpleMap<TKey, TValue> — обобщенным типом.

		При реализации обобщенных типов часто возникает необходимость наложения ограничений на типы-параметры.
		Например, при реализации ассоциативного массива с помощью самобалансирующегося дерева поиска, необходимо 
		чтобы экземпляры TKey можно было сравнивать. 
		
		Одна из возможностей сделать так, чтобы значения вашего типа можно было сравнивать — это реализовать в вашем типе интерфейс
		[Comparable](http://docs.oracle.com/javase/7/docs/api/java/lang/Comparable.html).
		Значит, необходимо наложить на TKey следующее ограничение: он должен реализовывать интерфейс Comparable<TKey>:


			public interface OrderedSimpleMap<TKey extends Comparable<TKey>, TValue>
			{
				TValue get(TKey key);
				TValue put(TKey key, TValue value);
				TValue remove(TKey key);
			}

		## Задача

		Сделайте ваши реализации SimpleMap и Heap обобщенными типами. 
		*/
	}
}