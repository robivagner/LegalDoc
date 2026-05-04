namespace LegalDoc.Web.State;

public class AppState
{
    public event Action? OnChange;
    
    public int PendingTasksCount { get; private set; } = 0;
    public int PendingAiAnalysesCount { get; private set; } = 0;
    public bool NeedsProfileCompletion { get; private set; }
    
    public void UpdateProfileStatus(bool needsCompletion)
    {
        NeedsProfileCompletion = needsCompletion;
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