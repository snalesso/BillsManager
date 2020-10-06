using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billy.UI.Wpf.Services
{
    public class DialogResult<TResult>
    {
        public DialogResult(bool? isConfirmed, TResult content)
        {
            this.IsConfirmed = isConfirmed;
            this.Content = content;
        }

        public bool? IsConfirmed { get; }

        public TResult Content { get; }
    }
}