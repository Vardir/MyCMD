using System;

using Vardirsoft.MyCmd.Core.Commands;

using Enumerable = System.Linq.Enumerable;

namespace Vardirsoft.MyCmd.ConsoleApp
{
    internal static class Program
    {
        private static ExecutionService ExecutionService;
        
        private static readonly string Header;

        static Program()
        {
            var assembly = typeof(ExecutionService).Assembly;
            var version = assembly.GetName().Version;
            
            Header = $"MyCMD v{version!.Major}.{version.Minor}.{version.Build}";
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

        private static void Print(string str, Message msgType = Message.Default, bool breakLine = true)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = GetForeground(msgType);
            Console.Write($"{str}{(breakLine ? "\n\r" : "")}");
            Console.ForegroundColor = oldColor;
        }
        
        private static void Main(string[] args)
        {
            Initialize();

            CleanScreen();
            var times = 0;
            do
            {
                Print("> ", Message.Placeholder, false);
                
                var query = Console.ReadLine();
                Console.WriteLine();
                
                var result = ExecutionService.Execute(query);
                Interpret(result);
                
                if (!result.isEmpty)
                {
                    Console.WriteLine();
                }

                times++;
            }
            while (times < 3000);

            Console.ReadKey();
        }

        private static void Initialize()
        {
            ExecutionService = new ExecutionService();

            //activate parser
            ExecutionService.Execute("");
        }
        private static void Interpret(ExecutionResult executionResult)
        {
            if (executionResult.isEmpty)
            {
                //do nothing
            }
            else if (executionResult.isSuccessful)
            {
                var valueType = executionResult.result?.GetType();
                if (valueType != null)
                {
                    if (valueType.IsArray)
                    {
                        Print($"[{string.Join(", ", executionResult.result as object[] ?? Enumerable.Empty<object>())}]");
                        
                        return;
                    }
                }
                
                Print(executionResult.result?.ToString());
            }
            else
            {
                Print(executionResult.errorMessage, Message.Error);
            }
        }

        private static ConsoleColor GetForeground(Message msgType) =>
            msgType switch
            {
                Message.Error => ConsoleColor.DarkRed,
                Message.Info => ConsoleColor.DarkCyan,
                Message.Placeholder => ConsoleColor.Gray,
                Message.Default => ConsoleColor.White,
                _ => ConsoleColor.White
            };
    }
}