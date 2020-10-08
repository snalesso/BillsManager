using Caliburn.Micro;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Billy.UI.Wpf.Core
{
    public class ReactiveObjectEx : ReactiveObject, INotifyPropertyChangedEx
    {
        /// <summary>
        /// Creates an instance of <see cref = "PropertyChangedBase" />.
        /// </summary>
        public ReactiveObjectEx()
        {
            this.IsNotifying = true;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Enables/Disables property change notification.
        /// Virtualized in order to help with document oriented view models.
        /// </summary>
        public virtual bool IsNotifying { get; set; }

        /// <summary>
        /// Raises a change notification indicating that all bindings should be refreshed.
        /// </summary>
        public virtual void Refresh()
        {
            this.NotifyOfPropertyChange(string.Empty);
        }

        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <param name = "propertyName">Name of the property.</param>
        public virtual void NotifyOfPropertyChange([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (this.IsNotifying && PropertyChanged != null)
            {
                if (PlatformProvider.Current.PropertyChangeNotificationsOnUIThread)
                {
                    this.OnUIThread(() => this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName)));
                }
                else
                {
                    this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        /// <summary>
        /// Notifies subscribers of the property change.
        /// </summary>
        /// <typeparam name = "TProperty">The type of the property.</typeparam>
        /// <param name = "property">The property expression.</param>
        public void NotifyOfPropertyChange<TProperty>(Expression<Func<TProperty>> property)
        {
            this.NotifyOfPropertyChange(ReactiveUI.ExpressionMixins.GetMemberInfo(property).Name);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event directly.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Executes the given action on the UI thread
        /// </summary>
        /// <remarks>An extension point for subclasses to customise how property change notifications are handled.</remarks>
        /// <param name="action"></param>
        protected virtual void OnUIThread(System.Action action) => action.OnUIThread();

        /// <summary>
        /// Sets a backing field value and if it's changed raise a notification.
        /// </summary>
        /// <typeparam name="T">The type of the value being set.</typeparam>
        /// <param name="oldValue">A reference to the field to update.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyName">The name of the property for change notifications.</param>
        /// <returns></returns>
        public virtual bool Set<T>(ref T oldValue, T newValue, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                return false;
            }

            oldValue = newValue;

            this.NotifyOfPropertyChange(propertyName ?? string.Empty);

            return true;
        }
    }
}
