using System;


namespace DBTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DBController controller = new DBController();

            Console.WriteLine(controller.GetPlayerPurchace(1, 1));
            controller.AddPlayerPurchace(1, 1);
            Console.WriteLine(controller.GetPlayerPurchace(1, 1));
        }
    }
}
