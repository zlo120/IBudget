using Core.Exceptions;
using Core.Model;

namespace MyBudgetter_Prototype.Utils
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
        public static DateTime GetDate()
        {
            Console.Write("Please insert the date (format: dd/MM/yyyy): ");
            string userInput = Console.ReadLine();

            // Try to parse the user input into a DateTime
            if (DateTime.TryParseExact(userInput, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime userDate))
            {
                // Successfully parsed
                return userDate;
            }
            else
            {
                throw new InvalidInputException("Invalid input. Please enter a valid amount.");
            }
        }
        public static List<Tag> GetTags()
        {
            var tags = new List<Tag>();
            Console.WriteLine("Enter a tag associated with this expense (optional, enter nothing to continue): ");
            while (true)
            {

                Console.Write(" > ");
                var input = Console.ReadLine().ToLower();
                if (input == "")
                {
                    break;
                }
                var tag = new Tag()
                {
                    Name = input
                };

                tags.Add(tag);
            }

            return tags;
        }
        public static string Prompt(string message, bool optional = false)
        {
            if (message.Contains(":")) 
                Console.Write(message);
            else
                Console.Write(message + ": ");

            var userInput = Console.ReadLine();

            if (string.IsNullOrEmpty(userInput) && optional is false)
            {
                throw new InvalidInputException("Invalid input. Please enter a valid amount.");
            }

            return userInput;
        }
        public static int NumberPrompt(string message)
        {
            var userInput = Prompt(message);
            if (int.TryParse(userInput, out int amount))
            {
                return amount;
            }
            else
            {
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
        public static void PrintTitle(string message)
        {
            var border = new String('=', message.Length);
            Console.WriteLine(border);
            Console.WriteLine(message);
            Console.WriteLine(border);
            Console.WriteLine();
        }
    }
}