using BillsManager.ViewModels.Messages;
using Caliburn.Micro;

namespace BillsManager.ViewModels
{
    public partial class StatusBarViewModel : Screen,
        IHandle<DBConnectionStateChangedMessage>,
        IHandle<SuppliersListChangedMessage>,
        IHandle<BillsListChangedMessage>
    {
        #region fields

        private readonly IEventAggregator gloablEventAggregator;

        #endregion

        #region ctor

        public StatusBarViewModel(IEventAggregator gloablEventAggregator)
        {
            this.gloablEventAggregator = gloablEventAggregator;
            this.gloablEventAggregator.Subscribe(this);
            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                        this.gloablEventAggregator.Unsubscribe(this);
                };
        }

        #endregion

        #region properties

        private DBConnectionState connectionState = DBConnectionState.Disconnected;
        public DBConnectionState ConnectionState
        {
            get { return this.connectionState; }
            set
            {
                if (this.connectionState == value) return;

                this.connectionState = value;
                this.NotifyOfPropertyChange(() => this.ConnectionState);

                this.NotifyOfPropertyChange(() => this.IsConnected);
                this.NotifyOfPropertyChange(() => this.IsDirty);
            }
        }

        public bool IsConnected
        {
            get { return this.ConnectionState != DBConnectionState.Disconnected; }
        }

        public bool IsDirty
        {
            get { return this.ConnectionState != DBConnectionState.Dirty; }
        }

        private ulong suppliersCount;
        public ulong SuppliersCount
        {
            get { return this.suppliersCount; }
            set
            {
                if (this.suppliersCount == value) return;

                this.suppliersCount = value;
                this.NotifyOfPropertyChange(() => this.SuppliersCount);
            }
        }

        private ulong billsCount;
        public ulong BillsCount
        {
            get { return this.billsCount; }
            set
            {
                if (this.billsCount == value) return;

                this.billsCount = value;
                this.NotifyOfPropertyChange(() => this.BillsCount);
            }
        }

        #endregion

        #region methods

        #region message handlers

        public void Handle(DBConnectionStateChangedMessage message)
        {
            this.ConnectionState = message.DBState;
        }

        public void Handle(SuppliersListChangedMessage message)
        {
            this.SuppliersCount = message.Suppliers.ULongCount();
        }

        public void Handle(BillsListChangedMessage message)
        {
            this.BillsCount = message.Bills.ULongCount();
        }

        #endregion

        #endregion
    }
}