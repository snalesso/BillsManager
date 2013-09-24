using System;
using System.Collections.Generic;
using BillsManager.Model;

namespace BillsManager.ViewModel.Messages
{
    public class BillsFilterMessage
    {
        public BillsFilterMessage(Func<BillViewModel, bool> filter)
        {
            this.filter = filter;
        }

        public BillsFilterMessage(IEnumerable<Func<BillViewModel, bool>> filters)
        {
            this.filters = filters;
        }

        private readonly Func<BillViewModel, bool> filter;
        public Func<BillViewModel, bool> Filter
        {
            get { return this.filter; }
        }

        private readonly IEnumerable<Func<BillViewModel, bool>> filters;
        public IEnumerable<Func<BillViewModel, bool>> Filters
        {
            get { return this.filters; }
        }
    }
}