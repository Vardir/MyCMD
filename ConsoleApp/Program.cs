using System;
using Core.Commands;
using System.Reflection;
using ConsoleApp.Commands;

namespace ConsoleApp
{
    internal class Program
    {
        public static readonly string Header;

        public static ExecutionService executionService;

        static Program()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Header = $"MyCMD v{version.Major}.{version.Minor}";
        }

        public static void Close()
        {
            Print("Terminating processes...", Message.Info);
            Print("Closing console...", Message.Info);
            Environment.Exit(0);
        }
        public static void CleanScreen()
        {
            Console.Clear();
            Print(Header, Message.Info);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void Print(string str, Message msgType = Message.Default, bool breakLine = true)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = GetForeground(msgType);
            Console.Write($"{str}{(breakLine ? "\n\r" : "")}");
            Console.ForegroundColor = oldColor;
        }
        
        private static void Main(string[] args)
        {
            Initialize();

            CleanScreen();
            int times = 0;
            do
            {
                Print("> ", Message.Placeholder, false);
                string query = Console.ReadLine();
                Console.WriteLine();
                var result = executionService.Execute(query);
                Interpret(result);
                Console.WriteLine();
            }
            while (times < 3000);

            Console.ReadKey();
        }

        private static void Initialize()
        {
            executionService = new ExecutionService();

            executionService.AddCommands(new Command[]
            {
                new ExitCommand(), new CleanScreenCommand(), new HelpCommand()
            });

            //activate parser
            executionService.Execute("");
        }
        private static void Interpret(ExecutionResult executionResult)
        {
            if (executionResult.successfull)
                Print(executionResult.result?.ToString());
            else
                Print(executionResult.errorMessage, Message.Error);
        }

        private static ConsoleColor GetForeground(Message msgType)
        {
            switch (msgType)
            {
                case Message.Error:
                    return ConsoleColor.DarkRed;
                case Message.Info:
                    return ConsoleColor.DarkCyan;
                case Message.Placeholder:
                    return ConsoleColor.Gray;
                case Message.Default:
                default:
                    return ConsoleColor.White;
            }
        }
    }
}