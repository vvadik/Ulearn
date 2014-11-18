# Аудиторные практики

## Коллоквиум

Разобрать третью и четвертую задачи 
[коллоквиума](https://docs.google.com/document/d/1jRcE6tFSUGsXUrmaTaabeCkrfCKHkwBB9al7o-27Km0/edit?usp=sharing)

## Вернуться к вопросу о составлении карт памяти. Можно рассмотреть следующие примеры:

### Пример 1

Вариация на тему коллоквиума (некоторые почему-то решили эту задачу вместо поставленной, нужно проговорить)

	static void Make(string[] array)
	{
		if (array.Length==0) return;
		var a=new array[array.Length-1];
		for (int i=0;i<a.Length;i++)
			a[i]=array[i+1];
	}

	static void Main()
	{
		Make(new [] { "A", "B", "C" });
	}

### Пример 1

	static void ProcessRow(int[] row)
	{
		for (int i=0;i<row.Length;i++)
			row[i]*=2;
	}

	static void Main()
	{
		var array=new int[3][];
		for (int i=0;i<array.Length;i++)
			array[i]=new int[i];
		ProcessRow(array[0]);
	}

Разобрать, почему этот фокус не пройдет с int[,]

## Ликвидировать долги по предыдущим парам

У каждого преподавателя пропадали пары из-за медосмотров и праздников, 
в оставшееся время можно наверстать их.