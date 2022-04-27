using Core.DataAccess;
using Entities.Concrete;
using Entities.Dtos;

namespace DataAccess.Abstract
{
    public interface IUserRelationshipDal : IEntityRepository<UserRelationship>
    {
        List<UserRelationshipDto> GetListDto(int adminUserId);
        UserRelationshipDto GetById(int userUserId);
    }
}