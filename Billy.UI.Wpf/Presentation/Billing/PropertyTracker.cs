namespace Billy.UI.Wpf.Presentation.Billing
{
    public class PropertyTracker<TProperty> : IValueTracker<TProperty>
    {
        public PropertyTracker(TProperty originalValue = default)
        {
            this.Original = originalValue;
        }

        public TProperty Original { get; }
        public TProperty Current { get; set; }

        object IValueTracker.Original => this.Original;
        object IValueTracker.Current
        {
            get => this.Current;
            set => this.Current = (TProperty)value;
        }
    }
}