using IBudget.Core.Exceptions;
using IBudget.Core.Model;
using Microsoft.Extensions.Options;

namespace IBudget.ConsoleUI.Utils
{
    public static class UserInput
    {
        public static int GetUserDecision(int lowerBound, int upperBound, bool optional = false)
        {
            if (optional is true)
            {
                Console.Write("\nPlease enter your choice (optional): ");
            } 
            else
            {
                Console.Write("\nPlease enter your choice: ");
            }

            string userInput = Console.ReadLine();

            if (string.IsNullOrEmpty(userInput) && optional is true)
            {
                return -1;
            }

            // Try to parse the user input into an integer
            if (int.TryParse(userInput, out int userChoice))
            {
                // Successfully parsed

                // check if the user input is within the specified range
                if (userChoice < lowerBound || userChoice > upperBound)
                {
                    throw new InvalidInputException("Invalid input. Please enter a valid amount.");
                } 

                return userChoice;
            }
            else
            {
                if (optional is true) return -1;
                throw new InvalidInputException("Invalid input. Please enter a valid amount.");
            }
        }
        public static int MultipleChoicePrompt(string[] choices, bool optional = false)
        {
            for (int i = 0; i < choices.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {choices[i]}");
            }

            return GetUserDecision(1, choices.Length, optional);
        }
    }
}