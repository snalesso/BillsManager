using System;
using System.Windows;

namespace Billy.Services.Reporting
{
    public sealed class TextAlignmentAttribute : Attribute
    {
        public TextAlignmentAttribute(TextAlignment alignment)
        {
            this.alignment = alignment;
        }
        
        private readonly TextAlignment alignment;
        public TextAlignment Alignment
        {
            get { return this.alignment; }
        }
    }
}