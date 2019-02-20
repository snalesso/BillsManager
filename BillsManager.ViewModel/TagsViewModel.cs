using BillsManager.Models;
using BillsManager.Services.Providers;
using Caliburn.Micro;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace BillsManager.ViewModels
{
    public class TagsViewModel : Screen
    {
        #region fields

        private readonly IWindowManager windowManager;
        private readonly IEventAggregator globalEventAggregator;
        private readonly ITagsProvider tagsProvider;

        // factories
        private readonly Func<Tag, TagViewModel> tagViewModelFactory;

        #endregion

        #region ctor

        public TagsViewModel(
            IWindowManager windowManager,
            IEventAggregator globalEventAggregator,
            ITagsProvider tagsProvider,
            Func<Tag, TagViewModel> tagViewModelFactory)
        {
            this.windowManager = windowManager;
            this.globalEventAggregator = globalEventAggregator;
            this.tagsProvider = tagsProvider;

            this.tagViewModelFactory = tagViewModelFactory;

            this.globalEventAggregator.Subscribe(this);

            this.Deactivated +=
                (s, e) =>
                {
                    if (e.WasClosed)
                        this.globalEventAggregator.Unsubscribe(this);
                };

            this.LoadTags();

            // UI
            this.DisplayName = "Tags";
        }

        #endregion

        #region properties

        private ObservableCollection<TagViewModel> tagViewModels;
        public ObservableCollection<TagViewModel> TagViewModels
        {
            get { return this.tagViewModels; }
            set
            {
                if (this.tagViewModels == value) return;

                this.tagViewModels = value;
                this.NotifyOfPropertyChange(() => this.TagViewModels);
            }
        }

        #endregion

        #region methods

        private void LoadTags()
        {
            var tags = this.tagsProvider.GetAll();
            var tagViewModels = tags.Select(tag => this.tagViewModelFactory.Invoke(tag));
            this.TagViewModels = new ObservableCollection<TagViewModel>(tagViewModels);
        }

        #endregion

        #region commands
        #endregion
    }
}