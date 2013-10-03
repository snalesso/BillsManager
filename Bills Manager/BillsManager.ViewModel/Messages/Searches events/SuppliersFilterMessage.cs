using System;
using System.Collections.Generic;

namespace BillsManager.ViewModel.Messages
{
    // TODO: make generic ?
    public class SuppliersFilterMessage
    {
        public SuppliersFilterMessage(Func<SupplierDetailsViewModel, bool> filter)
        {
            this.filter = filter;
        }

        public SuppliersFilterMessage(IEnumerable<Func<SupplierDetailsViewModel, bool>> filters)
        {
            this.filters = filters;
        }

        private readonly Func<SupplierDetailsViewModel, bool> filter;
        public Func<SupplierDetailsViewModel, bool> Filter
        {
            get { return this.filter; }
        }

        private readonly IEnumerable<Func<SupplierDetailsViewModel, bool>> filters;
        public IEnumerable<Func<SupplierDetailsViewModel, bool>> Filters
        {
            get { return this.filters; }
        }
    }
}