namespace LegalDoc.Web.State;

public class AppState
{
    public event Action? OnChange;

    public string CurrentUser { get; private set; } = "Vizitator";
    public int PendingTasksCount { get; private set; } = 0;
    
    public int PendingAiAnalysesCount { get; private set; } = 0;

    public void SetCurrentUser(string userName)
    {
        CurrentUser = userName;
        NotifyStateChanged();
    }

    public void UpdatePendingTasks(int count)
    {
        PendingTasksCount = count;
        NotifyStateChanged();
    }
    
    public void UpdatePendingAiAnalyses(int count)
    {
        PendingAiAnalysesCount = count;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}