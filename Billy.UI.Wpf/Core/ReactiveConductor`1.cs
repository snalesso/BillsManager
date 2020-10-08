using Caliburn.Micro;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Billy.UI.Wpf.Core
{
    /// <summary>
    /// An implementation of <see cref="IConductor"/> that holds on to and activates only one item at a time.
    /// </summary>
    public partial class ReactiveConductor<T> : ReactiveConductorBaseWithActiveItem<T> where T : class
    {
        /// <inheritdoc />
        public override async Task ActivateItemAsync(T item, CancellationToken cancellationToken = default)
        {
            if (item != null && item.Equals(this.ActiveItem))
            {
                if (this.IsActive)
                {
                    await ScreenExtensions.TryActivateAsync(item, cancellationToken);
                    this.OnActivationProcessed(item, true);
                }
                return;
            }

            var closeResult = await this.CloseStrategy.ExecuteAsync(new[] { this.ActiveItem }, cancellationToken);

            if (closeResult.CloseCanOccur)
            {
                await this.ChangeActiveItemAsync(item, true, cancellationToken);
            }
            else
            {
                this.OnActivationProcessed(item, false);
            }
        }

        /// <summary>
        /// Deactivates the specified item.
        /// </summary>
        /// <param name="item">The item to close.</param>
        /// <param name="close">Indicates whether or not to close the item after deactivating it.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override async Task DeactivateItemAsync(T item, bool close, CancellationToken cancellationToken = default)
        {
            if (item == null || !item.Equals(this.ActiveItem))
            {
                return;
            }

            var closeResult = await this.CloseStrategy.ExecuteAsync(new[] { this.ActiveItem }, CancellationToken.None);

            if (closeResult.CloseCanOccur)
            {
                await this.ChangeActiveItemAsync(default, close);
            }
        }

        /// <summary>
        /// Called to check whether or not this instance can close.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override async Task<bool> CanCloseAsync(CancellationToken cancellationToken = default)
        {
            var closeResult = await this.CloseStrategy.ExecuteAsync(new[] { this.ActiveItem }, cancellationToken);

            return closeResult.CloseCanOccur;
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            return ScreenExtensions.TryActivateAsync(this.ActiveItem, cancellationToken);
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Indicates whether this instance will be closed.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            return ScreenExtensions.TryDeactivateAsync(this.ActiveItem, close, cancellationToken);
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <returns>The collection of children.</returns>
        public override IEnumerable<T> GetChildren()
        {
            return new[] { this.ActiveItem };
        }
    }
}
