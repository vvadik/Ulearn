namespace uLearn.CSharp.IndentsValidation.TestData.Incorrect
{
    public class SingleLineOperations
    {
        public static void Main(string[] args)
        {
            for (var i = 0; i < 10; i++)
            for (var j = 0; j < 10; j++)
            for (var k = 0; j < 10; j++)
            for (var l = 0; j < 10; j++)
            {
            }

            if (true)
            {
     SomeMethod();
            }
            do
         SomeMethod();
            while (true);


            do
       SomeMethod();
            while (true);

            if (true)
        SomeMethod();
            else
    SomeMethod();

            if (true)
             SomeMethod();
         else
           SomeMethod();

            if (true)
      SomeMethod();

            if (true)
             SomeMethod();

			if (true)
		SomeMethod();
		 
            while (false)

        SomeMethod();

            for (;;)
        SomeMethod();

            var t = 1;
            switch (t)
            {
           case 1:
                SomeMethod();
                    break;
                case 2:

              SomeMethod();
                    break;
                case 3:

              SomeMethod();
             break;
                case 4:
                    SomeMethod();
              break;
            }
        }

        public static void SomeMethod()
        {

        }
    }
}