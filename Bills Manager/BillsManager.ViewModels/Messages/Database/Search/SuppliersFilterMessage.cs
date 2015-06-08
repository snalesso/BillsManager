using System.Collections.Generic;

namespace BillsManager.ViewModels.Messages
{
    public class SuppliersFilterMessage
    {
        //public SuppliersFilterMessage(Func<SupplierDetailsViewModel, bool> filter)
        //{
        //    this.filter = filter;
        //}

        public SuppliersFilterMessage(IEnumerable<Filter<SupplierDetailsViewModel>> filters)
        {
            this.filters = filters;
        }

        //private readonly Func<SupplierDetailsViewModel, bool> filter;
        //public Func<SupplierDetailsViewModel, bool> Filter
        //{
        //    get { return this.filter; }
        //}

        private readonly IEnumerable<Filter<SupplierDetailsViewModel>> filters;
        public IEnumerable<Filter<SupplierDetailsViewModel>> Filters
        {
            get { return this.filters; }
        }
    }
}