using BillsManager.Models;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BillsManager.ViewModels
{
    public partial class BackupViewModel : Screen
    {
        #region ctor

        public BackupViewModel(
            Backup backup)
        {
            if (backup == null)
                throw new ArgumentNullException("backup cannot be null.");

            this.exposedBackup = backup;

            this.DisplayName = "Backup: " + this.CreationTime.ToShortDateString() + " - " + this.CreationTime.ToShortTimeString();
        }

        #endregion

        #region properties

        private Backup exposedBackup;
        public Backup ExposedBackup
        {
            get { return this.exposedBackup; }
            protected set
            {
                if (this.exposedBackup != value)
                {
                    this.exposedBackup = value;
                    this.NotifyOfPropertyChange(() => this.ExposedBackup);
                }
            }
        }

        #region wrapped from backup

        public string Path
        {
            get { return this.ExposedBackup.Path; }
        }

        public DateTime CreationTime
        {
            get { return this.ExposedBackup.CreationTime; }
        }

        public IEnumerable<DateTime> RollbackDates
        {
            get { return this.ExposedBackup.RollbackDates; }
        }

        public uint SuppliersCount
        {
            get { return this.ExposedBackup.SuppliersCount; }
        }

        public uint BillsCount
        {
            get { return this.ExposedBackup.BillsCount; }
        }

        #endregion

        #region added

        public bool HasRollbacks
        {
            get { return this.TimesUsedForRollback > 0; }
        }

        public bool HasNoRollbacks
        {
            get { return !this.HasRollbacks; }
        }

        public ushort TimesUsedForRollback
        {
            get { return (ushort)this.RollbackDates.Count(); }
        }

        #endregion

        #endregion
    }
}