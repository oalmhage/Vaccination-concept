using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.Utilities;

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
        public static string csvOutput = @"C:\Windows\Temp\Personer\Vaccinationer.csv";
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
                    bool hasErrors = SearchForErrors(input);

                    if (hasErrors)
                    {
                        Console.WriteLine("Det har uppstått felaktiga rader i CSV-filen. Försök igen.");
                        
                    }
                    else
                    {
                        VerifyFileToCreatePriority(input);
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
                    Console.WriteLine("Hejdå! :)");
                }
                Console.WriteLine();
            }
        }
        public static bool SearchForErrors(string[] input)
        {
            List<string> errorList = new List<string>();
            bool hasErrors = false;

            foreach (string line in input)
            {
                try
                {
                    string[] values = line.Split(",");
                    if (values.Length != 6 || InvalidFormat(values, errorList))
                    {
                        errorList.Add(line);
                        hasErrors = true;
                    }
                }
                catch (Exception e)
                {
                    errorList.Add(e.Message);
                }
            }

            if (hasErrors)
            {
                foreach (string error in errorList)
                {
                    Console.WriteLine(error);
                }
                return true;
            }

            return false;
        }
        public static bool InvalidFormat(string[] values, List<string> errorList)
        {
            bool hasErrors = false;
            for (int i = values.Length - 3; i < values.Length; i++)
            {
                if (!values[i].Contains("0") && !values[i].Contains("1"))
                {
                   
                    hasErrors = true;
                }
            }
            return hasErrors;
        }
        public static void VerifyFileToCreatePriority(string[] input)
        {
            while (true)
            {
                bool fileExists = File.Exists(csvOutput);
                if (!fileExists)
                { 
                    Console.Clear();
                    Console.WriteLine("Utdatafilen existerar inte. Du måste ange en befintlig mapp " +
                        "eller fil för att skapa prioritetsordningen.");
                    break;
                }

                bool fileNotEmpty = new FileInfo(csvOutput).Length > 0;

                if (fileNotEmpty)
                {
                    int confirmOption = ShowMenu("Utdatafilen är inte tom. Vill du skriva över den? ", new[]
                    {
                        "Ja",
                        "Nej",
                    });

                    if (confirmOption == 0)
                    {
                        // Användaren har godkänt, fortsätt med att skapa prioritetsordningen.
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
                        break; // Användaren har avbrutit överskrivningen
                    }
                }
                else if (!fileNotEmpty)
                {
                    // Filen finns men är tom, skapa prioritetsordningen direkt.
                    string[] outputLines = CreateVaccinationOrder(input, doses, vaccinateChildren);
                    File.WriteAllLines(csvOutput, outputLines);
                    Console.Clear();
                    Console.WriteLine("Din prioritetsordning har skapats i " + csvOutput);
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
                    Console.Clear();
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

                    // Kontrollera om mappen existerar
                    if (!Directory.Exists(directoryPath))
                    {
                        throw new DirectoryNotFoundException();
                    }

                    // Om filen inte finns, skapa den.
                    if (!File.Exists(filePath))
                    {
                        File.Create(filePath).Close();
                        Console.WriteLine("Din fil har skapats. ");
                    }

                    csvOutput = filePath;
                    break; // Avsluta loopen om allt är korrekt.
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine("Mappen du har angivit existerar inte. Ange en giltig mapp.");
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
            var sortedPatients = patients;

            var worksInHealthcarePriority = patients
                .Where(p => p.WorksInHealthCare == 1)
                .OrderByDescending(p => CalculateExactAge(p.PersonNummer))
                .ToList();

            var patientsOver65 = patients
                .Where(p => CalculateExactAge(p.PersonNummer) >= 65 && p.WorksInHealthCare == 0)
                .OrderByDescending(p => CalculateExactAge(p.PersonNummer))
                .ToList();

            var riskGroupPriority = patients
                .Where(p => p.RiskGroup == 1 && p.WorksInHealthCare == 0 && CalculateExactAge(p.PersonNummer) < 65)
                .OrderByDescending(p => CalculateExactAge(p.PersonNummer))
                .ToList();

            var restOfPriority = patients
                .Where(p => p.WorksInHealthCare == 0 && p.RiskGroup == 0 && CalculateExactAge(p.PersonNummer) < 65)
                .OrderBy(p => p.PersonNummer)
                .ToList();

            sortedPatients = worksInHealthcarePriority
                .Concat(patientsOver65)
                .Concat(riskGroupPriority)
                .Concat(restOfPriority)
                .ToList();

            return sortedPatients;
        }
        public static int CalculateExactAge(string personNummer)
        {
            int year = int.Parse(personNummer.Substring(0, 4));
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

                if (!vaccinateChildren && CalculateExactAge(person.PersonNummer) < 18) 
                {
                    // Gå vidare till nästa person om VaccinateChildren är false
                    continue; 
                }

                int requiredDoses = (person.HasBeenInfected == 1) ? 1 : 2;

                if (requiredDoses > remainingDoses)
                {
                    //Om den nuvarande personen behöver 2 doser och det endast finns 1 kvar
                    //gå vidare till nästa person som behöver 1 dos
                    continue; 
                }

                if (requiredDoses == 2 && remainingDoses == 1) 
                {
                    bool oneDoseRequired = false;
                    // Loopa igenom resten av personerna för att hitta en som behöver 1 dos.
                    for (int j = i + 1; j < sortedPatients.Count; j++) 
                    {
                        Person nextPerson = sortedPatients[j];
                        if ((nextPerson.HasBeenInfected == 1 ? 1 : 2) == 1) 
                        {
                            requiredDoses = 1;
                            oneDoseRequired = true;
                            // Sätt 'i' till nästa person för att undvika dubbelräkning.
                            i = j; 
                            break;
                        }
                    }

                    if (!oneDoseRequired) 
                    {
                        // Om ingen person som behöver 1 dos hittas, avsluta loopen.
                        break;
                    }
                }
                string outputLine = $"{person.PersonNummer},{person.LastName},{person.FirstName},{requiredDoses}";
                outputLines.Add(outputLine); 

                remainingDoses -= requiredDoses; 
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
        public void VaccinateChildrenFalse()
        {
            string[] input =
            {
                "800731-7777, Potter, Harry, 0, 0, 0",
                "800605-6666, Malfoy, Draco, 0, 0, 0",
                "20060913-1313, Potter, Albus, 0, 0, 0",
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
        public void VaccinateChildrenTrue()
        {
            string[] input =
            {
                "800731-7777, Potter, Harry, 0, 0, 0",
                "800605-6666, Malfoy, Draco, 0, 0, 0",
                "20060913-1313, Potter, Albus, 0, 0, 0",
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
            Assert.AreEqual("20060913-1313, Potter, Albus,2", output[5]);
        }

        [TestMethod]
        public void PersonnummerWithoutCorrectFormat()
        {
            string[] input =
            {
                "8007317777, Potter, Harry, 0, 0, 0",
                "8006056666, Malfoy, Draco, 0, 0, 0",
                "060913-1313, Potter, Albus, 0, 0, 0",
                "198007309876, Weasley, Ronald, 0, 0, 0",
                "4910303030, Weasley, Molly, 0, 1, 1",
                "4909144040, Weasley, Arthur, 0, 1, 1"
            };
            int doses = 10;
            bool vaccinateChildren = true;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            Assert.AreEqual(output.Length, 6);
            Assert.AreEqual("19060913-1313, Potter, Albus,2", output[0]);
            Assert.AreEqual("19490914-4040, Weasley, Arthur,1", output[1]);
            Assert.AreEqual("19491030-3030, Weasley, Molly,1", output[2]);
            Assert.AreEqual("19800605-6666, Malfoy, Draco,2", output[3]);
            Assert.AreEqual("19800730-9876, Weasley, Ronald,2", output[4]);
            Assert.AreEqual("19800731-7777, Potter, Harry,2", output[5]);
        }

        [TestMethod]
        public void RequiredDosesIsLessThanRemainingDoses()
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
        [TestMethod]
        public void RemainingDosesIsLessThenRequiredDoses()
        {
            string[] input =
            {
            "800731-7777, Potter, Harry, 0, 0, 0",
            "800605-6666, Malfoy, Draco, 1, 0, 0",
            "20060913-1313, Potter, Albus, 0, 0, 0",
            "19800730-9876, Weasley, Ronald, 0, 0, 0",
            "491030-3030, Weasley, Molly, 0, 1, 1",
            "490914-4040, Weasley, Arthur, 0, 1, 1"
            };
            int doses = 20;
            bool vaccinateChildren = true;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            Assert.AreEqual(output.Length, 6);
            Assert.AreEqual("19800605-6666, Malfoy, Draco,2", output[0]);
            Assert.AreEqual("19490914-4040, Weasley, Arthur,1", output[1]);
            Assert.AreEqual("19491030-3030, Weasley, Molly,1", output[2]);
            Assert.AreEqual("19800730-9876, Weasley, Ronald,2", output[3]);
            Assert.AreEqual("19800731-7777, Potter, Harry,2", output[4]);
            Assert.AreEqual("20060913-1313, Potter, Albus,2", output[5]);

        }
        [TestMethod]
        public void OnlyOneDoseLeft()
        {
            string[] input =
            {
                "19580907-9999, White, Walter, 0, 1, 1",
                "19840924-8888, Pinkman, Jesse, 0, 1, 1",
                "19700811-7777, White, Skylar, 0, 1, 0",
                "19720730-6666, Schrader, Marie, 1, 0, 1",
                "19720102-5555, Schrader, Hank, 0, 0, 0",
                "201008174444, White, Holly, 0, 0, 1"
            };
            int doses = 6;
            bool vaccinateChildren = true;

            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            Assert.AreEqual(output.Length, 5);
            Assert.AreEqual("19720730-6666, Schrader, Marie,1", output[0]);
            Assert.AreEqual("19580907-9999, White, Walter,1", output[1]);
            Assert.AreEqual("19700811-7777, White, Skylar,2", output[2]);
            Assert.AreEqual("19840924-8888, Pinkman, Jesse,1", output[3]);
            Assert.AreEqual("20100817-4444, White, Holly,1", output[4]);
        }

    }
}
