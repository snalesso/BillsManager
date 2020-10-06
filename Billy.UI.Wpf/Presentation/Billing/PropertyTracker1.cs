using System;

namespace Billy.UI.Wpf.Presentation.Billing
{
    public class PropertyTracker
    {
        public PropertyTracker(object originalValue)
            : this(originalValue.GetType())
        {
            this.Original = originalValue;
        }

        public PropertyTracker(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; }

        object Original { get; }
        private object _current;
        object Current
        {
            get => this._current;
            set
            {
                if (value.GetType() != this.Type)
                    throw new InvalidOperationException();

                this._current = value;
            }
        }
    }
}