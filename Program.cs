using System;


namespace DBTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DBController controller = new DBController();

            Console.WriteLine("\nAdding new player in database...");
            Player player = new Player(-1, "abcd@mail.com", "12345", DateTime.Now, DateTime.Now);
            Console.WriteLine(player);
            controller.AddNewPlayer(player);


            Console.WriteLine("\nSetting new settings to player with id #3");
            Console.WriteLine("\nBefore:");
            Console.WriteLine(controller.GetPlayerSettings(3));
            PlayerSettings settings = new PlayerSettings(-1, true, false, "eu");
            controller.SetSettingsForPlayer(3, settings);
            Console.WriteLine("\nAfter:");
            Console.WriteLine(controller.GetPlayerSettings(3));

            Console.WriteLine("\nAll players:");
            foreach (var item in controller.GetAllPlayers())
            {
                Console.WriteLine(item);
            }

            Console.WriteLine("\nAdd a subject with id #1 to player purchace wiht id #1");
            Console.WriteLine($"\nBefore count: {controller.GetPlayerPurchace(1, 1)}");
            controller.AddPlayerPurchace(1, 1);
            Console.WriteLine($"\nAfter count: {controller.GetPlayerPurchace(1, 1)}");
        }
    }
}
