using Caliburn.Micro;
using BillsManager.Model;
using System;

namespace BillsManager.ViewModel
{
    public partial class BackupViewModel : PropertyChangedBase
    {
        #region fields
        #endregion

        #region ctor

        public BackupViewModel(
            Backup backup)
        {
            this.exposedBackup = backup;
        }

        #endregion

        #region properties

        private Backup exposedBackup; // TODO: replace every exposed property with a bare field. WHY???
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

        public ushort TimesUsedForRollback
        {
            get { return this.ExposedBackup.TimesUsedForRollback; }
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

        #endregion

        #region methods
        #endregion
    }
}
