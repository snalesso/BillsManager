using System;

namespace Billy
{
    public class Range<T>
        where T : struct, IComparable<T>
    {
        public Range(T? start, T? end)
        {
            this.Start = start;
            this.End = end;
        }
        public Range(T? singleValue)
        {
            this.Start = this.End = singleValue;
        }

        public T? Start { get; } //set; }
        public T? End { get; } //set; }

        public bool IsSingleValue =>
            this.Start.HasValue
            && this.End.HasValue
            && this.Start.Value.CompareTo(this.End.Value) == 0;
    }
}
