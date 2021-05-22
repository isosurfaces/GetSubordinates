using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace Employees
{
    class Program
    {
        static void Main(string[] args) {
            var currentWD = AppDomain.CurrentDomain.BaseDirectory;
            var fileName = "userHeirarchy.json";
            var output = "subordinates.json";
            var defaultPath = Path.Combine(currentWD, fileName);
            var outputPath = Path.Combine(currentWD, output);

            // Load json file
            // Default json input
            Console.WriteLine($"Loading default user-role file: {defaultPath}");
            Console.WriteLine("Please enter a file location or leave blank to use default: ");

            // Custom json input
            var path = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(path)) path = defaultPath;

            var data = new UserRoleManager();
            string errorMessage = null;

            while (!data.LoadUserRoleFromFile(path, ref errorMessage))
            {
                Console.WriteLine($"Error: {errorMessage}");
                Console.WriteLine("Please enter a file location or leave blank to use default: ");
                path = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(path)) path = defaultPath;
            }

            // Get SubOrdinates
            while (true)
            {
                Console.WriteLine("Please enter an Employee ID: ");
                if (int.TryParse(Console.ReadLine(), out var idValue))
                {
                    var result = data.GetSubOrdinates(idValue);
                    
                    if (result != null)
                    {
                        var ids = result.Select(x => x.Id);

                        // Return SubOrdinates
                        Console.WriteLine($"Found {result.Count} subordinates: {string.Join(", ", ids)}");
                        Console.WriteLine($"Writing result to file '{output}'.");
                        using (StreamWriter file = File.CreateText(outputPath))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            serializer.Serialize(file, result);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid Employee ID.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Employee ID.");
                }
            }
        }
    }
}
