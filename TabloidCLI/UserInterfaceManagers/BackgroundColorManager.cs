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
