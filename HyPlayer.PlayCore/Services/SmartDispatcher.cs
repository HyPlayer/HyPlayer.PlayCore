namespace HyPlayer.PlayCore.Services;

public interface ISmartDispatcher
{
    public void InitializeDispatcher(int threadId,  Func<Action, Task> runner);
    public Task InvokeAsync(Action action);
}

public class SmartDispatcher : ISmartDispatcher
{
    private int _uiThreadId = -1;
    private  Func<Action, Task>? _runner = null;

    public void InitializeDispatcher(int threadId, Func<Action, Task> runner)
    {
        _uiThreadId = threadId;
        _runner = runner;
    }

    public async Task InvokeAsync(Action action)
    {
        if (Thread.CurrentThread.ManagedThreadId != _uiThreadId)
        {
            var res = _runner?.Invoke(action);
            res ??= Task.CompletedTask;
            await res;
        }
        else
        {
            action.Invoke();
        }
    }
}