using System;
using System.Collections.Generic;

namespace BillsManager.Model
{
    public class Backup
    {
        #region ctor

        public Backup(
            string path,
            DateTime creationTime,
            uint billsCount,
            uint suppliersCount,
            IEnumerable<DateTime> rollbackDates)
        {
            this.path = path;
            this.creationTime = creationTime;
            this.billsCount = billsCount;
            this.suppliersCount = suppliersCount;
            this.rollbackDates = rollbackDates;
        }

        #endregion

        #region properties

        private readonly string path;
        public string Path
        {
            get { return this.path; }
        }

        private readonly DateTime creationTime;
        public DateTime CreationTime
        {
            get { return this.creationTime; }
        }

        private IEnumerable<DateTime> rollbackDates;
        public IEnumerable<DateTime> RollbackDates
        {
            get { return rollbackDates; }
        }

        private readonly uint suppliersCount = 0;
        public uint SuppliersCount
        {
            get { return this.suppliersCount; }
        }

        private readonly uint billsCount = 0;
        public uint BillsCount
        {
            get { return this.billsCount; }
        }

        #endregion
    }
}
