using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //create the initial drinks order
            var barOrder = InitializeDictionary();
            WriteDictionary(barOrder);

            //someone cancels their drink
            Console.WriteLine("\nRemove drink by key ....");
            barOrder.Remove(145);

            //write the order again
            WriteDictionary(barOrder);

            //someone wants to add a couple of drinks to the order
            AddDrinks(barOrder);
            WriteDictionary(barOrder);

            //checks if the Dictionary contains certain keys, and gets the
            //value if it does
            CheckDrinks(barOrder);
            WaitForKeyPress();

            //converts the dictionary to json
            Console.WriteLine("\nConvert the order to json ....");
            var json = JsonConvert.SerializeObject(barOrder);
            Console.WriteLine(json);

            //now clear the order because we are finished
            Console.WriteLine("\nNow clear the order because is's complete....");
            barOrder.Clear();

            //write the order again
            WriteDictionary(barOrder);
        }

        private static void WaitForKeyPress()
        {
            Console.WriteLine("Press any key to continue....");
            Console.ReadKey();
        }

        private static Dictionary<int, string> InitializeDictionary()
        {
            Dictionary<int, string> drinksOrder = new Dictionary<int, string>()
            {
                { 100, "Lemonade"},
                { 203, "Gin and tonic" },
                { 145, "Small white wine"},
                { 701, "Coke" },
                { 361, "Orange juice" }
            };

            Console.WriteLine("Drinks order created - Dictionary initialized.");

            return drinksOrder;
        }

        private static void CheckDrinks(Dictionary<int, string> barOrder)
        {
            Console.WriteLine("\n\nCheck if certain drinks exist in the order, using the drinks name....\n");

            //someone wants to know if their Coke is in the order, so we need to check
            if (barOrder.ContainsValue("Coke"))
            {
                Console.WriteLine("There's a Coke in the order.");
            }
            else
            {
                Console.WriteLine("There's no Coke in the order.");
            }

            if (barOrder.ContainsValue("Wine"))
            {
                Console.WriteLine("There's a Wine in the order.");
            }
            else
            {
                Console.WriteLine("There's no Wine in the order.");
            }


            Console.WriteLine("\n\nNow check if a certain drink is in the order...");

            string selectedDrink = "No drink found";

            //check drink reference
            if (barOrder.ContainsKey(361))
            {
                selectedDrink = barOrder[361];
            }

            Console.WriteLine("Get Drink using key > " + selectedDrink);
            Console.WriteLine("\n");

        }

        private static void WriteDictionary(Dictionary<int, string> list)
        {
            Console.WriteLine("\n****** BAR ORDER ******");

            foreach (var item in list)
            {
                Console.WriteLine(item.Key + ": " + item.Value);
            }

            if (list.Count == 0)
            {
                Console.WriteLine("---This order is empty---");
            }

            Console.WriteLine("--------------------------\n");
            WaitForKeyPress();
        }

        private static void AddDrinks(Dictionary<int, string> bardrinks)
        {
            Console.WriteLine("Add two drinks to the order.....");

            bardrinks.Add(487, "Sparkling water");
            bardrinks.Add(925, "Prosecco");
        }

    }
}

