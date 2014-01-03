using System.Collections.Generic;

namespace BillsManager.ViewModels.Messages
{
    public class BillsFilterMessage
    {
        //public BillsFilterMessage(Filter<BillDetailsViewModel> filter)
        //{
        //    this.filter = filter;
        //}

        public BillsFilterMessage(IEnumerable<Filter<BillDetailsViewModel>> filters)
        {
            this.filters = filters;
        }

        //private readonly Filter<BillDetailsViewModel> filter;
        //public Filter<BillDetailsViewModel> Filter
        //{
        //    get { return this.filter; }
        //}

        private readonly IEnumerable<Filter<BillDetailsViewModel>> filters;
        public IEnumerable<Filter<BillDetailsViewModel>> Filters
        {
            get { return this.filters; }
        }
    }
}