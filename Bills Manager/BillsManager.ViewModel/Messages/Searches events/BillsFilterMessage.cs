using System;
using System.Collections.Generic;
using BillsManager.Model;

namespace BillsManager.ViewModel.Messages
{
    public class BillsFilterMessage
    {
        public BillsFilterMessage(Func<BillDetailsViewModel, bool> filter)
        {
            this.filter = filter;
        }

        public BillsFilterMessage(IEnumerable<Func<BillDetailsViewModel, bool>> filters)
        {
            this.filters = filters;
        }

        private readonly Func<BillDetailsViewModel, bool> filter;
        public Func<BillDetailsViewModel, bool> Filter
        {
            get { return this.filter; }
        }

        private readonly IEnumerable<Func<BillDetailsViewModel, bool>> filters;
        public IEnumerable<Func<BillDetailsViewModel, bool>> Filters
        {
            get { return this.filters; }
        }
    }
}