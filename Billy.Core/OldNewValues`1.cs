using System;
using System.Collections.Generic;
using System.Text;

namespace Billy
{
    [Obsolete]
    public class OldNewValues
    {
        public OldNewValues(
            object oldValue
            , object newValue = default)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public object OldValue { get; }
        public object NewValue { get; set; }
    }

    [Obsolete]
    public class OldNewValues<T> //: OldNewValues
    {
        public OldNewValues(
            T oldValue
            , T newValue = default)
        //: base(oldValue, newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public T OldValue { get; }
        public T NewValue { get; set; }
    }
}
