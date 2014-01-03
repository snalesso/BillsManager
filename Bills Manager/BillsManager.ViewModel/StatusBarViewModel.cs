using BillsManager.ViewModels.Messages;
using Caliburn.Micro;

namespace BillsManager.ViewModels
{
    public partial class StatusBarViewModel : Screen,
        IHandle<ActiveDBChangedMessage>
    {
        #region fields

        private readonly IEventAggregator gloablEventAggregator;

        #endregion

        #region ctor

        public StatusBarViewModel(IEventAggregator gloablEventAggregator)
        {
            this.gloablEventAggregator = gloablEventAggregator;

            this.gloablEventAggregator.Subscribe(this);
        }

        #endregion

        #region properties
        
        public bool HasActiveDB
        {
            get { return this.ActiveDBViewModel != null; }
        }
                
        private DBViewModel activeDBViewModel;
        public DBViewModel ActiveDBViewModel
        {
            get { return this.activeDBViewModel; }
            set
            {
                if (this.activeDBViewModel != value)
                {
                    this.activeDBViewModel = value;
                    this.NotifyOfPropertyChange(() => this.ActiveDBViewModel);
                    this.NotifyOfPropertyChange(() => this.HasActiveDB);
                }
            }
        }

        #endregion

        #region methods

        #region message handlers

        public void Handle(ActiveDBChangedMessage message)
        {
            this.ActiveDBViewModel = message.ActiveDB;
        }

        #endregion

        #endregion
    }
}