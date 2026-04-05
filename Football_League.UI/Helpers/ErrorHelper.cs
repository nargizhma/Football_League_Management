using System;
using System.Collections.Generic;

namespace Football_League.UI.Helpers
{
    public static class ErrorHelper
    {
        public static string GetFullErrorMessage(Exception ex)
        {
            var messages = new List<string>();
            var current = ex;

            while (current != null)
            {
                messages.Add($"• {current.GetType().Name}: {current.Message}");
                current = current.InnerException;
            }

            return string.Join("\n", messages);
        }

        public static void DisplayError(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║                          ERROR OCCURRED                             ║");
            Console.WriteLine("╠════════════════════════════════════════════════════════════════════╣");
            Console.WriteLine(GetFullErrorMessage(ex));
            Console.WriteLine("╚════════════════════════════════════════════════════════════════════╝\n");
            Console.ResetColor();
        }
    }
}
