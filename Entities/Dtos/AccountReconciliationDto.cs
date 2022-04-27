using Core.Entities;
using Entities.Concrete;

namespace Entities.Dtos
{
    public class AccountReconciliationDto : AccountReconciliation, IDto
    {
        public string CompanyName { get; set; }
        public string CompanyTaxDepartment { get; set; }
        public string CompanyTaxIdNumber { get; set; }
        public string CompanyIdentityNumber { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string AccountTaxDepartment { get; set; }
        public string AccountTaxIdNumber { get; set; }
        public string AccountIdentityNumber { get; set; }        
        public string AccountEmail { get; set; }        
        public string CurrencyCode { get; set; }

    }
}