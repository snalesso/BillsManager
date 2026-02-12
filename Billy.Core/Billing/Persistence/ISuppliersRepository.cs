using Billy.Billing.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence
{
    public interface ISuppliersRepository : IReadSuppliersRepository, IWriteSuppliersRepository
    {
    }

    public interface IReadWriteBillingModel : IReadBillingModel, IWriteBillingModel
    {
    }
}