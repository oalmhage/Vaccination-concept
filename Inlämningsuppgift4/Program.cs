﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;

namespace Vaccination
{
    public class Person
    {
        public string PersonNummer;
        public string FirstName;
        public string LastName;
        public int WorksInHealthCare;
        public int RiskGroup;
        public int HasBeenInfected;
        public int RequiredDoses;
    }
    public class Program
    {
        public static int doses = 0;
        public static bool vaccinateChildren = false;
        public static string csvInput = @"C:\Windows\Temp\Personer\Patienter.csv";
        public static string csvOutput = @"C:\Windows\Temp\Vaccinationer\Vaccinationer.csv";
        public static void Main()
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("Huvudmeny: ");
                Console.WriteLine();
                Console.WriteLine("Antal tillgängliga doser: " + doses);
                Console.WriteLine("Vaccinering under 18 år: " + (vaccinateChildren ? "Ja" : "Nej"));
                Console.WriteLine("Indatafil: " + csvInput);
                Console.WriteLine("Utdatafil: " + csvOutput);
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
                    string[] input = File.ReadAllLines(csvInput);
                    bool hasErrors = CheckForErrors(input);

                    if (hasErrors)
                    {
                        Console.WriteLine("Det har uppstått felaktiga rader i CSV-filen. Försök igen.");
                    }
                    else
                    {
                        VerifyFileToCreatePriority();
                    }
                }
                else if (option == 1)
                {
                    doses = ChangeVaccineQuantity();
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
                    OutputFile();
                }
                else
                {
                    running = false;
                    Console.WriteLine("Hejdå!");
                }
                Console.WriteLine();
            }
        }

        public static bool CheckForErrors(string[] input)
        {
            List<string> errorList = new List<string>();
            {
                foreach (string line in input)
                    try
                    {
                        string[] values = line.Split(",");
                        if (values.Length != 6 && !WrongOneOrZeroValues(values))
                        {
                            errorList.Add(line);
                        }



                    }
                    catch (Exception e)
                    {
                        errorList.Add(e.Message);
                    }



                if (errorList.Count > 0)
                {



                    foreach (string error in errorList)
                    {
                        Console.WriteLine(error);
                    }
                    //true om det finns felaktiga rader
                    return true;
                }
            }
            //false om det inte finns några felaktiga rader
            return false;
        }
        public static bool WrongOneOrZeroValues(string[] columns)
        {
            for (int i = columns.Length - 3; i < columns.Length; i++)
            {
                if (columns[i] != "0" && columns[i] != "1")
                {
                    return false;
                }
            }
            return true;
        }
        public static void VerifyFileToCreatePriority()
        {
            while (true)
            {
                bool fileExists = File.Exists(csvOutput);
                bool fileNotEmpty = fileExists && new FileInfo(csvOutput).Length > 0;

                if (fileExists && fileNotEmpty)
                {
                    int confirmOption = ShowMenu("Utdatafilen är inte tom. Vill du skriva över den? ", new[]
                    {
                                "Ja",
                                "Nej",
                            });

                    if (confirmOption == 0)
                    {
                        // Användaren har godkänt överwriting, fortsätt med att skapa prioritetsordningen.
                        string[] input = File.ReadAllLines(csvInput);
                        string[] outputLines = CreateVaccinationOrder(input, doses, vaccinateChildren);
                        File.WriteAllLines(csvOutput, outputLines);
                        Console.Clear();
                        Console.WriteLine("Filen har skrivits över.");
                        break;
                    }
                    else if (confirmOption == 1)
                    {
                        Console.Clear();
                        Console.WriteLine("Filen har inte skrivits över.");
                        break; // Användaren har avbrutit, avsluta programmet.
                    }
                }
                else if (fileExists && !fileNotEmpty)
                {
                    // Filen finns men är tom, skapa prioritetsordningen direkt.
                    string[] input = File.ReadAllLines(csvInput);
                    string[] outputLines = CreateVaccinationOrder(input, doses, vaccinateChildren);
                    File.WriteAllLines(csvOutput, outputLines);
                    Console.Clear();
                    Console.WriteLine("Din prioritetsordning har skapats i " + csvOutput);
                    break;
                }
                else if (!fileExists)
                {
                    Console.Clear();
                    Console.WriteLine("Utdatafilen existerar inte. Du måste ange en befintlig fil för att skapa prioritetsordningen.");
                    break;
                }
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
                    doses = newAvailableDoses;
                    Console.WriteLine("Antalet har ändrats.");
                    return doses;
                }
                catch
                {
                    Console.WriteLine("Någonting gick fel. Du måste skriva in en siffra för att ändra antalet.");
                }
            }
        }
        public static bool AgeLimit()
        {
            int option = ShowMenu("Ska personer under 18 år vaccineras?", new[]
            {
                "Ja",
                "Nej"
            });

            if (option == 0)
            {
                vaccinateChildren = true;
            }
            else if (option == 1)
            {
                vaccinateChildren = false;
            }
            Console.Clear();
            return vaccinateChildren;
        }
        public static void InputFile()
        {
            while (true)
            {
                Console.WriteLine("Ange sökvägen till den nya indatafilen: ");
                string filePath = Console.ReadLine();
                try
                {
                    if (!File.Exists(filePath))
                    {
                        throw new FileNotFoundException("Filen existerar inte.");
                        //Avsluta loopen om filen finns.
                    }
                    csvInput = filePath;
                    break;
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("Ett oväntat fel uppstod: " + e.Message);
                }
            }
        }
        public static void OutputFile()
        {
            while (true)
            {
                Console.WriteLine("Ange sökvägen till den nya utdatafilen: ");
                string filePath = Console.ReadLine();

                try
                {
                    string directoryPath = Path.GetDirectoryName(filePath);

                    // Kontrollera om mappen existerar eller kan skapas.
                    if (!Directory.Exists(directoryPath))
                    {
                        throw new DirectoryNotFoundException("Mappen existerar inte.");
                    }

                    // Om filen inte finns, skapa den.
                    if (!File.Exists(filePath))
                    {
                        File.Create(filePath).Close();
                    }

                    //Uppdatera utdatafilens sökväg.
                    csvOutput = filePath;
                    break; // Avsluta loopen om allt är korrekt.
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine("Mappen existerar inte. Ange en giltig mapp." + e.Message);
                }

                catch (Exception e)
                {
                    Console.WriteLine("Ett oväntat fel uppstod vid uppdatering av sökvägen: " + e.Message);
                }
            }
        }

        public static List<Person> ProcessCSVData(string[] input)
        {
            List<Person> patients = new List<Person>();

            foreach (string line in input)
            {
                string[] values = line.Split(",");
                string personNummer = values[0];
                string lastName = values[1];
                string firstName = values[2];
                int worksInHealthCare = int.Parse(values[3]);
                int riskGroup = int.Parse(values[4]);
                int hasBeenInfected = int.Parse(values[5]);

                if (personNummer.Length < 12)
                {
                    personNummer = "19" + personNummer;
                }
                if (!personNummer.Contains("-"))
                {
                    personNummer = personNummer.Insert(8, "-");
                }

                Person person = new Person
                {
                    PersonNummer = personNummer,
                    LastName = lastName,
                    FirstName = firstName,
                    WorksInHealthCare = worksInHealthCare,
                    RiskGroup = riskGroup,
                    HasBeenInfected = hasBeenInfected,
                };
                patients.Add(person);

            }
            return patients;
        }
        public static List<Person> SortPatientList(List<Person> patients)
        {
            var sortedPatients = patients
            .OrderByDescending(p => p.WorksInHealthCare)
            .ThenByDescending(p => CalculateExactAge(p.PersonNummer) >= 65)
            .ThenByDescending(p => p.RiskGroup)
            .ThenBy(p => p.PersonNummer) // Sortera övriga personer på personnummer om ingen prioritering gäller.
            .ToList();

            return sortedPatients;
        }
        public static int CalculateExactAge(string personNummer)
        {

            int year = int.Parse(personNummer.Substring(0, 4));
            year += + 19;

            int month = int.Parse(personNummer.Substring(4, 2));
            int day = int.Parse(personNummer.Substring(6, 2));

            DateTime birthDate = new DateTime(year, month, day);
            DateTime currentDate = DateTime.Now;

            int age = currentDate.Year - birthDate.Year;
            if (currentDate < birthDate.AddYears(age))
            {
                age--;
            }
            return age;
        }
        public static string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            List<Person> patients = ProcessCSVData(input);
            List<Person> sortedPatients = SortPatientList(patients);

            List<string> outputLines = new List<string>();
            int remainingDoses = doses;

            for (int i = 0; i < sortedPatients.Count; i++)
            {
                Person person = sortedPatients[i];

                // Kontrollera om personen är yngre än 18 och om vaccinering av barn inte är aktiverad.
                if (!vaccinateChildren && CalculateExactAge(person.PersonNummer) < 18)
                {
                    continue; // Gå vidare till nästa person om barnvaccination inte är aktiverad.
                }

                int requiredDoses = (person.HasBeenInfected == 1) ? 1 : 2;

                if (requiredDoses > remainingDoses)
                {
                    //Om den nuvarande personen behöver 2 doser och det endast finns 1 kvar, gå vidare till
                    //nästa person som behöver 1 dos
                    continue;
                }

                if (requiredDoses == 2 && remainingDoses == 1)
                {
                    // Om det enbart finns 1 dos kvar och nästa person kräver 2 doser, leta efter en person som behöver 1 dos.
                    // Loopa igenom resten av personerna för att hitta en som behöver 1 dos.
                    bool oneDoseRequired = false;
                    for (int j = i + 1; j < sortedPatients.Count; j++)
                    {
                        Person nextPerson = sortedPatients[j];
                        if ((nextPerson.HasBeenInfected == 1 ? 1 : 2) == 1)
                        {
                            // Hittade en person som behöver 1 dos, använd den sista dosen för att vaccinera den personen.
                            requiredDoses = 1;
                            oneDoseRequired = true;
                            i = j; // Sätt 'i' till nästa person för att undvika dubbelräkning.
                            break;
                        }
                    }

                    if (!oneDoseRequired)
                    {
                        // Om ingen person som behöver 1 dos hittas, avsluta loopen.
                        break;
                    }
                }

                // Skapa en rad för vaccinering med personuppgifter och antal doser.
                string outputLine = $"{person.PersonNummer},{person.LastName},{person.FirstName},{requiredDoses}";
                outputLines.Add(outputLine);

                // Minska antalet tillgängliga doser med de använda doserna.
                remainingDoses -= requiredDoses;
                Console.WriteLine(outputLine);
            }
            return outputLines.ToArray();
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
        public void vaccinateChildrenFalse()
        {
            string[] input =
            {
                "800731-7777, Potter, Harry, 0, 0, 0",
                "800605-6666, Malfoy, Draco, 0, 0, 0",
                "20060913-1313, Albus, Potter, 0, 0, 0",
                "19800730-9876, Weasley, Ronald, 0, 0, 0",
                "491030-3030, Weasley, Molly, 0, 1, 1",
                "490914-4040, Weasley, Arthur, 0, 1, 1"
            };
            int doses = 10;
            bool vaccinateChildren = false;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            Assert.AreEqual(output.Length, 5);
            Assert.AreEqual("19490914-4040, Weasley, Arthur,1", output[0]);
            Assert.AreEqual("19491030-3030, Weasley, Molly,1", output[1]);
            Assert.AreEqual("19800605-6666, Malfoy, Draco,2", output[2]);
            Assert.AreEqual("19800730-9876, Weasley, Ronald,2", output[3]);
            Assert.AreEqual("19800731-7777, Potter, Harry,2", output[4]);
        }

        [TestMethod]
        public void vaccinateChildrenTrue()
        {
            string[] input =
            {
                "800731-7777, Potter, Harry, 0, 0, 0",
                "800605-6666, Malfoy, Draco, 0, 0, 0",
                "20060913-1313, Albus, Potter, 0, 0, 0",
                "19800730-9876, Weasley, Ronald, 0, 0, 0",
                "491030-3030, Weasley, Molly, 0, 1, 1",
                "490914-4040, Weasley, Arthur, 0, 1, 1"
            };
            int doses = 10;
            bool vaccinateChildren = true;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            Assert.AreEqual(output.Length, 6);
            Assert.AreEqual("19490914-4040, Weasley, Arthur,1", output[0]);
            Assert.AreEqual("19491030-3030, Weasley, Molly,1", output[1]);
            Assert.AreEqual("19800605-6666, Malfoy, Draco,2", output[2]);
            Assert.AreEqual("19800730-9876, Weasley, Ronald,2", output[3]);
            Assert.AreEqual("19800731-7777, Potter, Harry,2", output[4]);
            Assert.AreEqual("20060913-1313, Albus, Potter,2", output[5]);
        }

        [TestMethod]
        public void personnummerWithoutCorrectFormat()
        {
            string[] input =
            {
                "8007317777, Potter, Harry, 0, 0, 0",
                "8006056666, Malfoy, Draco, 0, 0, 0",
                "060913-1313, Albus, Potter, 0, 0, 0",
                "198007309876, Weasley, Ronald, 0, 0, 0",
                "4910303030, Weasley, Molly, 0, 1, 1",
                "4909144040, Weasley, Arthur, 0, 1, 1"
            };
            int doses = 10;
            bool vaccinateChildren = true;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            Assert.AreEqual(output.Length, 6);
            Assert.AreEqual("19060913-1313, Albus, Potter,2", output[0]);
            Assert.AreEqual("19490914-4040, Weasley, Arthur,1", output[1]);
            Assert.AreEqual("19491030-3030, Weasley, Molly,1", output[2]);
            Assert.AreEqual("19800605-6666, Malfoy, Draco,2", output[3]);
            Assert.AreEqual("19800730-9876, Weasley, Ronald,2", output[4]);
            Assert.AreEqual("19800731-7777, Potter, Harry,2", output[5]);
        }

        [TestMethod]
        public void RemainingDosesIsLessThanRequiredDoses()
        {
            string[] input =
            {
                "19580907-9999, White, Walter, 0, 1, 1",
                "19840924-8888, Pinkman, Jesse, 0, 1, 1",
                "19700811-7777, White, Skylar, 0, 1, 0",
                "19720730-6666, Schrader, Marie, 1, 0, 1",
                "19720102-5555, Schrader, Hank, 0, 0, 0"
            };
            int doses = 6;
            bool vaccinateChildren = true;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            Assert.AreEqual(output.Length, 4);
            Assert.AreEqual("19720730-6666, Schrader, Marie,1", output[0]);
            Assert.AreEqual("19580907-9999, White, Walter,1", output[1]);
            Assert.AreEqual("19700811-7777, White, Skylar,2", output[2]);
            Assert.AreEqual("19840924-8888, Pinkman, Jesse,1", output[3]);

        }

    }
}
