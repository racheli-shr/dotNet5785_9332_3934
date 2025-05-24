// See https://aka.ms/new-console-template for more information
using System;
namespace targil0
{

    partial class Program
    {
        // Main method: Entry point of the program; calls two welcome methods and waits for a key press.

        private static void Main(string[] args)
        {
            Welcome3934();
            Welcome9332();
            Console.ReadKey();
           
        }
        // Welcome3934: Partial method declaration (implementation not shown here).

        static partial void Welcome3934();

        // Welcome9332: Prompts the user to enter their name, reads input, and displays a greeting message.

        private static void Welcome9332()
        {
            Console.WriteLine("enter your name:");
            string name = Console.ReadLine();
            Console.WriteLine(name + ",wehvdgsghdsg");
        }
    }
}