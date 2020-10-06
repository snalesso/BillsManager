namespace Billy.UI.Wpf.Presentation.Billing
{
    public interface IValueTracker
    {
        object Original { get; }
        object Current { get; set; }

        bool IsChanged => this.Original != this.Current;
    }
}