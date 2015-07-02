﻿using BillsManager.Models;
using System.Collections.Generic;

namespace BillsManager.Services.Data
{
    public interface IBackupsProvider
    {
        string Location { get; }

        IEnumerable<Backup> GetAll();

        bool CreateNew();

        bool Rollback(Backup backup);

        bool Delete(Backup backup);
    }
}