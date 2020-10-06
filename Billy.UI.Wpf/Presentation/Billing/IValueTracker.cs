namespace Billy.UI.Wpf.Presentation.Billing
{
    public interface IValueTracker<T> : IValueTracker
    {
        new T Original { get; }
        new T Current { get; set; }
    }
}