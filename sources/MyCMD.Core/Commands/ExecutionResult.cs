namespace Vardirsoft.MyCmd.Core.Commands
{
    /// <summary>
    /// Execution result of the command
    /// </summary>
    public readonly struct ExecutionResult
    {
        /// <summary>
        /// Indicates that result is empty
        /// </summary>
        public readonly bool isEmpty;
        
        /// <summary>
        /// Indicates that command execution was successful
        /// </summary>
        public readonly bool isSuccessful;
        
        /// <summary>
        /// The actual result of execution
        /// </summary>
        public readonly object result;
        
        /// <summary>
        /// The error message provided by the command if execution fails
        /// </summary>
        public readonly string errorMessage;

        private ExecutionResult(bool empty)
        {
            isEmpty = empty;
            isSuccessful = true;
            result = null;
            errorMessage = string.Empty;
        }
        
        private ExecutionResult(string errorMessage)
        {
            isEmpty = false;
            isSuccessful = false;
            result = null;
            this.errorMessage = errorMessage;
        }
        
        private ExecutionResult(object result)
        {
            isEmpty = false;
            isSuccessful = result != null;
            this.result = result;
            errorMessage = string.Empty;
        }

        public static ExecutionResult Error(string errorMesssage) => new ExecutionResult(errorMessage: errorMesssage);
        
        public static ExecutionResult Success(object result) => new ExecutionResult(result: result);
        
        public static ExecutionResult Empty() => new ExecutionResult(true);
    }
}