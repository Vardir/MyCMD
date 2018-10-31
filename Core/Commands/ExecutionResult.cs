namespace Core.Commands
{
    public struct ExecutionResult
    {
        public readonly bool isEmpty;
        public readonly bool isSuccessfull;
        public readonly object result;
        public readonly string errorMessage;

        private ExecutionResult(string errorMessage)
        {
            isEmpty = false;
            isSuccessfull = false;
            result = null;
            this.errorMessage = errorMessage;
        }
        private ExecutionResult(object result)
        {
            isEmpty = result == null;
            isSuccessfull = result != null;
            this.result = result;
            errorMessage = string.Empty;
        }

        public static ExecutionResult Error(string errorMesssage) => new ExecutionResult(errorMessage: errorMesssage);
        public static ExecutionResult Success(object result) => new ExecutionResult(result: result);
        public static ExecutionResult Empty() => new ExecutionResult(result: null);
    }
}