using System;
using System.Collections.Generic;
using BillsManager.Model;

namespace BillsManager.ViewModel.Messages
{
    public class SuppliersFilterMessage
    {
        public SuppliersFilterMessage(Func<SupplierViewModel, bool> filter)
        {
            this.filter = filter;
        }

        public SuppliersFilterMessage(IEnumerable<Func<SupplierViewModel, bool>> filters)
        {
            this.filters = filters;
        }

        private readonly Func<SupplierViewModel, bool> filter;
        public Func<SupplierViewModel, bool> Filter
        {
            get { return this.filter; }
        }

        private readonly IEnumerable<Func<SupplierViewModel, bool>> filters;
        public IEnumerable<Func<SupplierViewModel, bool>> Filters
        {
            get { return this.filters; }
        }
    }
}