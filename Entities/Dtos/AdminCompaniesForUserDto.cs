using Core.Entities;
using Entities.Concrete;

namespace Entities.Dtos
{
    public class AdminCompaniesForUserDto : Company , IDto 
    {
        public bool IsTrue { get; set; }
    }
}