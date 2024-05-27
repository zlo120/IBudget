namespace IBudget.ConsoleUI.Utils
{
    public static class ConsoleStyler
    {        public static void PrintTitle(string message)
        {
            var border = new String('=', message.Length);
            Console.WriteLine(border);
            Console.WriteLine(message);
            Console.WriteLine(border);
            Console.WriteLine();
        }
    }
}
