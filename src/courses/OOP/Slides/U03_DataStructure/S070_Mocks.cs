using System;
using System.Collections.Generic;
using uLearn;
using System.Linq;

namespace OOP.Slides.U03_DataStructure
{
    [Slide("Задача. Mock-объект", "{1AD6311A-E364-4F35-8C1F-42FEAF2E796A}")]
    public class S070_Mocks
    {
        /*
        Данная задача является необязательной.
        
        Предположим у вас есть сервис NumbersService, который использует абстракции INumbersCollection и INumberProcessor,
        а так же реализации их абстракций MySqlNumbersCollections и FibonaciNumberProcessor соответственно:
        */

        public class MySqlNumberCollection : INumbersCollection
        {
            public IEnumerable<int> GetNumbers()
            {
                //select * from numbers
                yield break;
            }
        }

        public interface INumbersCollection
        {
            IEnumerable<int> GetNumbers();
        }

        public interface INumberProcessor
        {
            int ProcessNumber(int number);
        }

        public class FibonaciNumberProcessor : INumberProcessor
        {
            public int ProcessNumber(int number)
            {
                // Вычисление числа фибоначи с номером number
                return 0;
            }
        }

        public class NumbersService
        {
            private readonly INumbersCollection collection;
            private readonly INumberProcessor processor;

            public NumbersService(INumbersCollection collection, INumberProcessor processor)
            {
                this.collection = collection;
                this.processor = processor;
            }

            public int GetTotal()
            {
                return collection.GetNumbers()
                    .Select(processor.ProcessNumber)
                    .Sum();
            }
        }

        /*
        Для тестировании данного сервиса есть несколько подходов:
        * Использовать честные реализации INumbersCollection и INumberProcessor - MySqlNumberCollection и FibonaciNumberProcessor.
        Однако, в этом случае вам необходим MySql сервер. Так же, данный подход противоречит парадигме модульного тестирования.
        * Написать тестовые реализации интерфейса  INumbersCollection и INumberProcessor:
        */

        public class TestNumbersCollection : INumbersCollection
        {
            private IEnumerable<int> internalNumbers;

            public void SetNumber(IEnumerable<int> numbers)
            {
                internalNumbers = numbers;
            }

            public IEnumerable<int> GetNumbers()
            {
                return internalNumbers;
            }
        }

        public class TestNumberProccessor : INumberProcessor
        {
            private readonly Dictionary<int, int> processed = new Dictionary<int, int>(); 

            public void SetProcessedNumber(int number, int processedNumber)
            {
                processed[number] = processedNumber;
            }

            public int ProcessNumber(int number)
            {
                return processed[number];
            }
        }

        /*
        Такие тестовые реализации называются Mock-объектами(https://en.wikipedia.org/wiki/Mock_object). Минусом данного подхода является то, что постоянно приходится писать однообразные тестовые рализации.
        Данную проблему решают фреймворки NMock, RhinoMocks. Подробно, например, о тестировании с RhinoMocks, можно почитать здесь(http://vsukhachev.wordpress.com/2011/03/23/unit-%D1%82%D0%B5%D1%81%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5-%D1%81-rhino-mocks/)
        
        ## Задача
        Необходимо протестировать ваш сервис автокомплита с помощью RhinoMocks.
        */
    }
}