namespace Core.Commands
{
    public struct ExecutionResult
    {
        public readonly bool successfull;
        public readonly object result;
        public readonly string errorMessage;

        private ExecutionResult(string errorMessage)
        {
            successfull = false;
            result = null;
            this.errorMessage = errorMessage;
        }
        private ExecutionResult(object result)
        {
            successfull = true;
            this.result = result;
            errorMessage = string.Empty;
        }

        public static ExecutionResult Error(string errorMesssage) => new ExecutionResult(errorMessage: errorMesssage);
        public static ExecutionResult Success(object result) => new ExecutionResult(result: result);
    }
}