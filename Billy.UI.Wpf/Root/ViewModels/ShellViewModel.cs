using Billy.Billing.ViewModels;
using Billy.UI.Wpf.Common.Services;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;

// TODO: change/remove .Presentation namespace name
namespace Billy.UI.Wpf.Root.ViewModels
{
    public class ShellViewModel : ReactiveConductor<IScreen>.Collection.AllActive, IDisposable
    {
        #region constancts & fields

        private readonly IDialogService _dialogService;

        #endregion

        #region ctor

        public ShellViewModel(
            IDialogService dialogService,
            SuppliersViewModel suppliersViewModel)
        {
            this._dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            this.SuppliersViewModel = suppliersViewModel ?? throw new ArgumentNullException(nameof(suppliersViewModel));

            //this._isEnabled_OAPH = Observable
            //    .Return(true)
            //    .ToProperty(this, nameof(this.IsEnabled))
            //    .DisposeWith(this._disposables);

            this.Items.Add(this.SuppliersViewModel);

            this.DisplayName = nameof(Billy) + Thread.CurrentThread.Name;
        }

        #endregion

        #region properties

        public SuppliersViewModel SuppliersViewModel { get; }

        //private readonly ObservableAsPropertyHelper<bool> _isEnabled_OAPH;
        //public bool IsEnabled => this._isEnabled_OAPH.Value;

        //private readonly ObservableAsPropertyHelper<TaskbarItemProgressState> _taskbarProgressState_OAPH;
        //public TaskbarItemProgressState TaskbarProgressState => this._taskbarProgressState_OAPH.Value;

        //private readonly ObservableAsPropertyHelper<double> _taskbarProgressValue_OAPH;
        //public double TaskbarProgressValue => this._taskbarProgressValue_OAPH.Value;

        #endregion

        #region methods

        protected override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            await base.OnActivateAsync(cancellationToken);
        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        //public override void CanClose(Action<bool> callback)
        //{
        //    //this._playbackService.StopAsync().Wait(); // TODO: handle special cases: playback stop/other actions before closing fail so can close should return false
        //    base.CanClose(callback);
        //}

        #endregion

        #region commands

        #endregion

        #region IDisposable

        // https://docs.microsoft.com/en-us/dotnet/api/system.idisposable?view=netframework-4.8
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private bool _isDisposed = false;

        // use this in derived class
        // protected override void Dispose(bool isDisposing)
        // use this in non-derived class
        protected virtual void Dispose(bool isDisposing)
        {
            if (this._isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                // free managed resources here
                this._disposables.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override a finalizer below.
            // set large fields to null.

            this._isDisposed = true;
        }

        // remove if in derived class
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool isDisposing) above.
            this.Dispose(true);
        }

        #endregion
    }
}