using Billy.Billing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billy.Billing.Persistence.EFCore.Entities
{
    public class EFCoreBill : Bill
    {
        public EFCoreBill(long id,
                          long supplierId,
                          DateTime releaseDate,
                          DateTime dueDate,
                          DateTime? paymentDate,
                          double amount,
                          double agio,
                          double additionalCosts,
                          string code,
                          string notes)
            : base(id, supplierId, releaseDate, dueDate, paymentDate, amount, agio, additionalCosts, code, notes)
        {
        }

        public EFCoreBill(long id,
                          long supplierId,
                          DateTime releaseDate,
                          DateTime dueDate,
                          DateTime? paymentDate,
                          DateTime registrationDate,
                          double amount,
                          double agio,
                          double additionalCosts,
                          string code,
                          string notes)
            : base(id, supplierId, releaseDate, dueDate, paymentDate, registrationDate, amount, agio, additionalCosts, code, notes)
        {
        }

        public virtual EFCoreSupplier Supplier { get; }
    }

    public class EFCoreSupplier : Supplier
    {
        public EFCoreSupplier(long id,
                              string name,
                              string eMail = null,
                              string webSite = null,
                              string phone = null,
                              string fax = null,
                              string notes = null,
                              Address address = null,
                              Agent agent = null)
            : base(id, name, eMail, webSite, phone, fax, notes, address, agent)
        {
        }

        public virtual ICollection<EFCoreBill> Bills { get; set; }
    }
}
