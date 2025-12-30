using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceNowAutomation.Helpers
{
    public static class ReadInstructions
    {
        public static string Read(string filePath)
        {
            string instructions = "Brak instrukcji";
            try
            {
                using StreamReader reader = new(filePath);
                instructions = reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file not found, access denied)
                Console.WriteLine($"Error reading file: {ex.Message}");
            }
            return instructions;
        }
    }
}
