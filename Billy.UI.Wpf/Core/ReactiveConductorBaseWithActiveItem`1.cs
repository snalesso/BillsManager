using Caliburn.Micro;
using System.Threading;
using System.Threading.Tasks;

namespace Billy.UI.Wpf.Core
{
    /// <summary>
    /// A base class for various implementations of <see cref="IConductor"/> that maintain an active item.
    /// </summary>
    /// <typeparam name="T">The type that is being conducted.</typeparam>
    public abstract class ReactiveConductorBaseWithActiveItem<T> : ReactiveConductorBase<T>, IConductActiveItem where T : class
    {
        private T _activeItem;

        /// <summary>
        /// The currently active item.
        /// </summary>
        public T ActiveItem
        {
            get => this._activeItem;
            set => this.ActivateItemAsync(value, CancellationToken.None);
        }

        /// <summary>
        /// The currently active item.
        /// </summary>
        /// <value></value>
        object IHaveActiveItem.ActiveItem
        {
            get => this.ActiveItem;
            set => this.ActiveItem = (T)value;
        }

        /// <summary>
        /// Changes the active item.
        /// </summary>
        /// <param name="newItem">The new item to activate.</param>
        /// <param name="closePrevious">Indicates whether or not to close the previous active item.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual async Task ChangeActiveItemAsync(T newItem, bool closePrevious, CancellationToken cancellationToken)
        {
            await ScreenExtensions.TryDeactivateAsync(this._activeItem, closePrevious, cancellationToken);

            newItem = this.EnsureItem(newItem);

            this._activeItem = newItem;
            this.NotifyOfPropertyChange(nameof(this.ActiveItem));

            if (this.IsActive)
                await ScreenExtensions.TryActivateAsync(newItem, cancellationToken);

            this.OnActivationProcessed(this._activeItem, true);
        }

        /// <summary>
        /// Changes the active item.
        /// </summary>
        /// <param name="newItem">The new item to activate.</param>
        /// <param name="closePrevious">Indicates whether or not to close the previous active item.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected Task ChangeActiveItemAsync(T newItem, bool closePrevious) => this.ChangeActiveItemAsync(newItem, closePrevious, default);
    }
}