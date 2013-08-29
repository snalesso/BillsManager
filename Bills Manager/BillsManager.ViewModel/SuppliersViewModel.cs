using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using BillsManager.Model;
using BillsManager.Service;
using BillsManager.Service.Providers;
using BillsManager.ViewModel.Commanding;
using BillsManager.ViewModel.Messages;
using Caliburn.Micro;

namespace BillsManager.ViewModel
{
    public partial class SuppliersViewModel :
        Screen,
        IHandle<AskForAvailableSuppliersMessage>,
        IHandle<SuppliersNeedRefreshMessage>,
        IHandle<SuppliersFilterMessage>,
        IHandle<SuppliersFilterNeedsRefreshMessage>,
        IHandle<AddNewSupplierMessage>
    {
        #region fields

        private readonly ISuppliersProvider suppliersProvider;
        private readonly IWindowManager windowManager;
        //private readonly IDialogService dialogService;
        private readonly IEventAggregator eventAggregator;

        #endregion

        #region ctor

        public SuppliersViewModel(
            ISuppliersProvider suppliersProvider,
            IWindowManager windowManager,
            //IDialogService dialogService,
            IEventAggregator eventAggregator)
        {
            this.suppliersProvider = suppliersProvider;
            this.windowManager = windowManager;
            //this.dialogService = dialogService;
            this.eventAggregator = eventAggregator;

            this.eventAggregator.Subscribe(this);
        }

        #endregion

        #region properties

        private SearchSuppliersViewModel searchSuppliersViewModel;
        public SearchSuppliersViewModel SearchSuppliersViewModel
        {
            get
            {
                if (this.searchSuppliersViewModel == null)
                    this.searchSuppliersViewModel = new SearchSuppliersViewModel(this.eventAggregator);

                return this.searchSuppliersViewModel;
            }
            set
            {
                if (this.searchSuppliersViewModel != value)
                {
                    this.searchSuppliersViewModel = value;
                    this.NotifyOfPropertyChange(() => this.SearchSuppliersViewModel);
                }
            }
        }

        private ObservableCollection<SupplierViewModel> supplierViewModels;
        public ObservableCollection<SupplierViewModel> SupplierViewModels
        {
            get
            {
                if (this.supplierViewModels == null)
                {
                    if (!Execute.InDesignMode)
                    {
                        this.supplierViewModels = new ObservableCollection<SupplierViewModel>(this.suppliersProvider.GetAll().Select(s => new SupplierViewModel(s, this.windowManager, /*this.dialogService,*/ this.eventAggregator)));
                        this.PublishAvailableSuppliersChanged();
                    }
                    // DESIGN TIME
                    else
                        this.SupplierViewModels = new ObservableCollection<SupplierViewModel>();
                }

                return this.supplierViewModels;
            }
            protected set
            {
                if (this.supplierViewModels != value)
                {
                    this.supplierViewModels = value;
                    this.NotifyOfPropertyChange(() => this.SupplierViewModels);
                }
            }
        }

        public ReadOnlyObservableCollection<SupplierViewModel> FilteredSupplierViewModels
        {
            get
            {
                if (this.Filters != null)

                    return new ReadOnlyObservableCollection<SupplierViewModel>(
                        new ObservableCollection<SupplierViewModel>(
                            this.SupplierViewModels.Where(this.Filters)));

                else
                    return new ReadOnlyObservableCollection<SupplierViewModel>(this.SupplierViewModels);
            }
        }

        private SupplierViewModel selectedSupplierViewModel;
        public SupplierViewModel SelectedSupplierViewModel
        {
            get { return this.selectedSupplierViewModel; }
            set
            {
                if (this.selectedSupplierViewModel != value)
                {
                    this.selectedSupplierViewModel = value;
                    this.NotifyOfPropertyChange(() => this.SelectedSupplierViewModel);
                }
            }
        }

        private IEnumerable<Func<SupplierViewModel, bool>> filters;
        public IEnumerable<Func<SupplierViewModel, bool>> Filters
        {
            get { return this.filters; }
            set
            {
                if (this.filters != value)
                {
                    this.filters = value;
                    this.NotifyOfPropertyChange(() => this.Filters);
                    this.NotifyOfPropertyChange(() => this.FilteredSupplierViewModels);
                }
            }
        }

        #endregion

        #region methods

        protected IEnumerable<Supplier> GetSuppliers()
        {
            List<Supplier> supps = new List<Supplier>();

            foreach (SupplierViewModel svm in this.SupplierViewModels)
                supps.Add(svm.ExposedSupplier);

            return supps;
        }

        protected void PublishAvailableSuppliersChanged()
        {
            this.eventAggregator.Publish(new AvailableSuppliersMessage(this.GetSuppliers()));
        }

        protected void RefreshSuppliersList()
        {
            this.SupplierViewModels = new ObservableCollection<SupplierViewModel>(this.suppliersProvider.GetAll().Select(s => new SupplierViewModel(s, this.windowManager, /*this.dialogService,*/ this.eventAggregator)));
        }

        #region message handlers

        public void Handle(AskForAvailableSuppliersMessage message)
        {
            message.AcquireSuppliersAction.Invoke(this.GetSuppliers());
        }

        public void Handle(SuppliersNeedRefreshMessage message)
        {
            this.RefreshSuppliersList();
        }

        public void Handle(SuppliersFilterMessage message)
        {
            this.Filters = message.Filters;
        }

        public void Handle(SuppliersFilterNeedsRefreshMessage message)
        {
            this.NotifyOfPropertyChange(() => this.FilteredSupplierViewModels);
        }

        public void Handle(AddNewSupplierMessage message)
        {
            this.AddNewSupplierCommand.Execute(null);
        }

        #endregion

        #endregion

        #region commands

        private RelayCommand addNewSupplierCommand;
        public RelayCommand AddNewSupplierCommand
        {
            get
            {
                if (this.addNewSupplierCommand == null) this.addNewSupplierCommand = new RelayCommand(
                    () =>
                    {
                        SupplierViewModel newSupp = new SupplierViewModel(new Supplier(this.suppliersProvider.GetLastID() + 1), this.windowManager, /*this.dialogService,*/ this.eventAggregator);

                        if (this.windowManager.ShowDialog(newSupp, null, settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } }) == true)
                        {
                            this.SupplierViewModels.Add(newSupp);
                            this.suppliersProvider.Add(newSupp.ExposedSupplier);
                            this.PublishAvailableSuppliersChanged();
                        }
                    });

                return this.addNewSupplierCommand;
            }
        }

        private RelayCommand<SupplierViewModel> editSupplierCommand;
        public RelayCommand<SupplierViewModel> EditSupplierCommand
        {
            get
            {
                if (this.editSupplierCommand == null)
                    this.editSupplierCommand = new RelayCommand<SupplierViewModel>(
                        p =>
                        {
                            string oldName = p.Name;

                            p.BeginEdit();

                            if (this.windowManager.ShowDialog(p, null, settings: new Dictionary<string, object> { { "ResizeMode", ResizeMode.NoResize } }) == true)
                            {
                                this.suppliersProvider.Edit(p.ExposedSupplier);

                                if (p.Name != oldName)
                                    this.eventAggregator.Publish(new SupplierNameChangedMessage(oldName, p.Name));
                            }
                        },
                        p => p != null);

                return this.editSupplierCommand;
            }
        }

        private RelayCommand<SupplierViewModel> deleteSupplierCommand;
        public RelayCommand<SupplierViewModel> DeleteSupplierCommand
        {
            get
            {
                if (this.deleteSupplierCommand == null)
                    this.deleteSupplierCommand = new RelayCommand<SupplierViewModel>(
                        p =>
                        {
                            //if (this.dialogService.ShowYesNoQuestion("Delete supplier", "Do you really want to delete this supplier?\n\n" +
                            //    p.Name + "\n" +
                            //    p.Street +
                            //    " " + p.Number +
                            //    ", " + p.Zip +
                            //    " " + p.City +
                            //    " (" + p.Province + ")" +
                            //    " - " + p.Country // TODO: fix (add Address support? can't remember what the problem is :| )
                            //    //+ "\n" + p.ExposedSupplier.Bills.Count + " bills"
                            //    ))
                            // TODO: use a custom dialog window containing a SupplierDetailsView
                            // TODO: language
                            // TODO: Supplier support

                            var question = new DialogViewModel(
                                "Delete supplier",
                                "Do you really want to delete this supplier?\n\n" +
                                p.Name + "\n" +
                                p.Street +
                                " " + p.Number +
                                ", " + p.Zip +
                                " " + p.City +
                                " (" + p.Province + ")" +
                                " - " + p.Country // TODO: fix (add Address support? can't remember what the problem is :| )
                                // + "\n" + p.ExposedSupplier.Bills.Count + " bills"
                                ,
                                new[]
                                {
                                    new DialogResponse(ResponseType.Yes, 5),
                                    new DialogResponse(ResponseType.No)
                                });

                            this.windowManager.ShowDialog(question);

                            if (question.Response == ResponseType.Yes)
                            {
                                this.suppliersProvider.Delete(p.ExposedSupplier);
                                this.SupplierViewModels.Remove(p);

                                this.NotifyOfPropertyChange(() => this.FilteredSupplierViewModels);

                                this.SelectedSupplierViewModel = null;
                                this.PublishAvailableSuppliersChanged(); // TODO: make that you can pass a parameter as edited, deleted, added in order to avoid multiple refreshes, or make single messages for each event
                                this.eventAggregator.Publish(new SupplierDeletedMessage(p.ExposedSupplier));
                            }
                        },
                        p => p != null);

                return this.deleteSupplierCommand;
            }
        }

        #endregion
    }
}
