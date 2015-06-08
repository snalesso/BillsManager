using System.Collections.Generic;

namespace BillsManager.ViewModels.Messages
{
    public partial class FilterMessage<T>
        where T : class
    {
        public FilterMessage(IEnumerable<Filter<T>> filters)
        {
            this.filters = filters;
        }

        private readonly IEnumerable<Filter<T>> filters;
        public IEnumerable<Filter<T>> Filters
        {
            get { return this.filters; }
        }
    }
}