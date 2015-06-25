using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillsManager.Models
{
    public interface ISettings
    {
        CultureInfo Language { get; set; }
    }
}