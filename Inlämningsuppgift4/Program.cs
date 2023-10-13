using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;

namespace Vaccination
{
    public class Person
    {
        public string Name;
        public string Age;
        public bool WorksInHealthCare;
        public bool RiskGroup;
        public bool HasBeenInfected;
        public int RequiredDoses;
    }
    public class Program
    {
        public static int availableDoses = 0;
        public static string aboveEighteen = "Nej";
        private static string csvPath = @"C:\Windows\Temp\Personer.csv";
        private static string inputFile = @"C:\Windows\Temp\Personer.csv";


        public static void Main()
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("Huvudmeny: ");
                Console.WriteLine();
                Console.WriteLine("Antal tillgängliga doser: " + availableDoses);
                Console.WriteLine("Vaccinering under 18 år: " + aboveEighteen);
                Console.WriteLine("Indatafil: " + inputFile) ;
                Console.WriteLine("Utdatafil: ");
                Console.WriteLine();

                int option = ShowMenu("Vad vill du göra? ", new[]
                {

                 "Skapa prioritetsordning",
                 "Ändra antal vaccindoser",
                 "Ändra åldersgräns",
                 "Ändra indatafil",
                 "Ändra utdatafill",
                 "Avsluta",

               });

                Console.Clear();

                if (option == 0)
                {
                    //PriorityOrder();
                }
                else if (option == 1)
                {
                    availableDoses = ChangeVaccineQuantity();
                }
                else if (option == 2)
                {
                    AgeLimit();
                }
                else if (option == 3)
                {
                    InputFile();
                }
                else if (option == 4)
                {
                    //OutputFile();
                }
                else
                {
                    running = false;
                    Console.WriteLine("Hejdå!");
                }
                Console.WriteLine();
            }
        }

        public static int ChangeVaccineQuantity()
        {
            while (true)
            {
                Console.WriteLine("Ange nytt antal vaccindoser: ");
                string userInput = Console.ReadLine();
                try
                {
                    int newAvailableDoses = int.Parse(userInput);
                    availableDoses = newAvailableDoses;
                    Console.WriteLine("Antalet har ändrats.");
                    return availableDoses;
                }
                catch
                {
                    Console.WriteLine("Någonting gick fel. Du måste skriva in en siffra för att ändra antalet.");
                }
            }
        }

        public static string AgeLimit()
        {
            int option = ShowMenu("Ska personer under 18 år vaccineras?", new[]
            {
                "Ja",
                "Nej,"
            });

            if (option == 0)
            {
                aboveEighteen = "Ja";
            }
            else if (option == 1)
            {
                aboveEighteen = "Nej";
            }
            Console.Clear();
            return aboveEighteen;
        }

        public static string InputFile()
        {
            while (true)
            {
                Console.WriteLine("Ändra indatafil");
                Console.WriteLine("__________");
                Console.WriteLine("Ange ny sökväg: ");
                string newInputFile = Console.ReadLine();

                try 
                {
                    File.Exists(newInputFile);
                    inputFile = newInputFile;
                    Console.WriteLine("Indatafilen har ändrats till: " + newInputFile);
                    return inputFile;
                }
                catch
                {
                    Console.WriteLine("Filen finns inte. Försök igen.");
                }
            }
            
        }

        // Create the lines that should be saved to a CSV file after creating the vaccination order.
        //
        // Parameters:
        //
        // input: the lines from a CSV file containing population information
        // doses: the number of vaccine doses available
        // vaccinateChildren: whether to vaccinate people younger than 18
        public static string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            // Replace with your own code.
            return new string[0];
        }

        public static int ShowMenu(string prompt, IEnumerable<string> options)
        {
            if (options == null || options.Count() == 0)
            {
                throw new ArgumentException("Cannot show a menu for an empty list of options.");
            }

            Console.WriteLine(prompt);

            // Hide the cursor that will blink after calling ReadKey.
            Console.CursorVisible = false;

            // Calculate the width of the widest option so we can make them all the same width later.
            int width = options.Max(option => option.Length);

            int selected = 0;
            int top = Console.CursorTop;
            for (int i = 0; i < options.Count(); i++)
            {
                // Start by highlighting the first option.
                if (i == 0)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                var option = options.ElementAt(i);
                // Pad every option to make them the same width, so the highlight is equally wide everywhere.
                Console.WriteLine("- " + option.PadRight(width));

                Console.ResetColor();
            }
            Console.CursorLeft = 0;
            Console.CursorTop = top - 1;

            ConsoleKey? key = null;
            while (key != ConsoleKey.Enter)
            {
                key = Console.ReadKey(intercept: true).Key;

                // First restore the previously selected option so it's not highlighted anymore.
                Console.CursorTop = top + selected;
                string oldOption = options.ElementAt(selected);
                Console.Write("- " + oldOption.PadRight(width));
                Console.CursorLeft = 0;
                Console.ResetColor();

                // Then find the new selected option.
                if (key == ConsoleKey.DownArrow)
                {
                    selected = Math.Min(selected + 1, options.Count() - 1);
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    selected = Math.Max(selected - 1, 0);
                }

                // Finally highlight the new selected option.
                Console.CursorTop = top + selected;
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                string newOption = options.ElementAt(selected);
                Console.Write("- " + newOption.PadRight(width));
                Console.CursorLeft = 0;
                // Place the cursor one step above the new selected option so that we can scroll and also see the option above.
                Console.CursorTop = top + selected - 1;
                Console.ResetColor();
            }

            // Afterwards, place the cursor below the menu so we can see whatever comes next.
            Console.CursorTop = top + options.Count();

            // Show the cursor again and return the selected option.
            Console.CursorVisible = true;
            return selected;
        }
    }

    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void ExampleTest()
        {
            // Arrange
            string[] input =
            {
                "19720906-1111,Elba,Idris,0,0,1",
                "8102032222,Efternamnsson,Eva,1,1,0"
            };
            int doses = 10;
            bool vaccinateChildren = false;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(output.Length, 2);
            Assert.AreEqual("19810203-2222,Efternamnsson,Eva,2", output[0]);
            Assert.AreEqual("19720906-1111,Elba,Idris,1", output[1]);
        }
    }
}
