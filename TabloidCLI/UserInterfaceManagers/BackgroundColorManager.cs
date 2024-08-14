using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabloidCLI.Models;

namespace TabloidCLI.UserInterfaceManagers
{
    public class BackgroundColorManager : IUserInterfaceManager
    {
        private readonly IUserInterfaceManager _parentUI;
        private string _connectionString;

        public BackgroundColorManager(IUserInterfaceManager parentUI, string connectionString)
        {
            _parentUI = parentUI;
            _connectionString = connectionString;
        }

        public IUserInterfaceManager Execute()
        {
            ConsoleColor[] colors = (ConsoleColor[])ConsoleColor.GetValues(typeof(ConsoleColor));

            Console.WriteLine("Background Color Menu");
            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i] == Console.BackgroundColor) continue;

                Console.WriteLine($" {i + 1}) {colors[i]}");
            }
            Console.WriteLine(" 0) Go Back");

            //Console.WriteLine(" 0) Black");
            //Console.WriteLine(" 1) Dark Blue");
            //Console.WriteLine(" 2) Dark Green");
            //Console.WriteLine(" 3) Dark Cyan");
            //Console.WriteLine(" 4) Dark Red");
            //Console.WriteLine(" 5) Dark Magenta");
            //Console.WriteLine(" 6) Dark Yellow");
            //Console.WriteLine(" 7) Gray");
            //Console.WriteLine(" 8) Dark Gray");
            //Console.WriteLine(" 9) Blue");
            //Console.WriteLine(" 10) Green");
            //Console.WriteLine(" 11) Cyan");
            //Console.WriteLine(" 12) Red");
            //Console.WriteLine(" 13) Magenta");
            //Console.WriteLine(" 14) Yellow");
            //Console.WriteLine(" 15) White");

            Console.Write("> ");
            string choice = Console.ReadLine();
            if (choice == "0")
            {
                return _parentUI;
            }

            int colorInt = int.Parse(choice);

            Console.BackgroundColor = colors[colorInt - 1];
            return this;
        }
    }
}
