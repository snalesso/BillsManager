using System;

namespace BillsManager.ViewModels
{
    public class Filter<T>
          where T : class
    {
        #region ctor

        public Filter(
            Predicate<T> filterPredicate,
            Func<string> description)
        {
            this.execute = filterPredicate;
            this.description = description;
        }

        #endregion

        #region properties

        private readonly Predicate<T> execute;
        public Predicate<T> Execute
        {
            get { return this.execute; }
        }

        private readonly Func<string> description;
        public Func<string> Description
        {
            get { return this.description; }
        }

        #endregion
    }
}