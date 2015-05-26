//using BillsManager.ViewModels.Commanding;
//using BillsManager.ViewModels.Messages;
//using Caliburn.Micro;

//namespace BillsManager.ViewModels.Search
//{
//    public abstract partial class SearchViewModel<T> : Screen
//        where T : class
//    {
//        #region fields

//        private readonly IEventAggregator globalEventAggregator;

//        #endregion

//        #region ctor

//        public SearchViewModel(IEventAggregator globalEventAggregator, Filter<T> filter)
//        {
//            this.globalEventAggregator = globalEventAggregator;

//            this.globalEventAggregator.Subscribe(this);

//            // handlers
//            this.Deactivated +=
//                (s, e) =>
//                {
//                    if (e.WasClosed)
//                        this.globalEventAggregator.Unsubscribe(this);
//                };

//            this.Filter = filter;
//        }

//        #endregion

//        #region properties

//        private string searchText;
//        public string SearchText
//        {
//            get { return this.searchText; }
//            set
//            {
//                if (this.searchText == value) return;

//                this.searchText = value;
//                this.NotifyOfPropertyChange(() => this.SearchText);
//            }
//        }

//        public Filter<T> Filter { protected set; get; }

//        #endregion

//        #region methods

//        public void Search()
//        {
//            if (!string.IsNullOrEmpty(this.SearchText))
//                this.globalEventAggregator.PublishOnUIThread(new FilterMessage<T>(new Filter<T>[] { this.Filter }));
//            else
//                this.globalEventAggregator.PublishOnUIThread(new FilterMessage<T>(null));
//        }

//        #endregion

//        #region commands

//        private RelayCommand searchCommand;
//        public RelayCommand SearchCommand
//        {
//            get
//            {
//                return this.searchCommand ?? (this.searchCommand =
//                    new RelayCommand(
//                        () => this.Search()));
//            }
//        }

//        #endregion
//    }
//}