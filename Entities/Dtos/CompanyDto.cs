using Core.Entities;
using Entities.Concrete;

namespace Entities.Dtos
{
    public class CompanyDto : Company, IDto
    {        
        public int UserId { get; set; }
    }
}