namespace Billy.UI.Wpf.Services
{
    public interface ITaskbarService
    {
        // show playback progress on taskbar: playing green, pause yellow
        // jumplist buttons
        // thumb buttons
        // possible dependencies: IVisualizerService -> show animation in thumbnail
        void SetTaskbarProgressValue(double? progressPercentValue);

        void SetTaskbarProgressStatus(TaskbarProgressStatus status);
    }

    public enum TaskbarProgressStatus
    {
        None,
        Advancing,
        Stopped,
        Failed
    }
}