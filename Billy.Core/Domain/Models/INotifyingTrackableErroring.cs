using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Billy.Core.Domain.Models
{
    public interface INotifyingTrackableErroring :
        INotifyPropertyChanging, INotifyPropertyChanged,
        IChangeTracking, IRevertibleChangeTracking, //IEditableObject,
        INotifyDataErrorInfo, IDataErrorInfo
    {
    }
}
