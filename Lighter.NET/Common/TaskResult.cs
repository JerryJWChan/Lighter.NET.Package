namespace Lighter.NET.Common
{
    public class TaskResult<TResult> : System.Threading.Tasks.Task<TResult>
    {
        public TaskResult(Func<object?, TResult> function, object? state) : base(function, state)
        {
        }
    }
}
