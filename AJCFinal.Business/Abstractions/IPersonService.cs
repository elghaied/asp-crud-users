using AJCFinal.Business.DataTransfertObjects;

namespace AJCFinal.Business.Abstractions
{
    public interface IPersonService
    {
        Task<PersonDto?> GetPersonByIdAsync(long id);
        Task<IEnumerable<PersonDto>> GetAllPersonsAsync();
        Task<long> UpdatePersonAsync(PersonDto personDto);
        Task<bool> AddFriendAsync(long personId, long friendId);
        Task<IEnumerable<PersonDto>> GetFriendsAsync(long personId);
        Task<bool> RemoveFriendAsync(long personId, long friendId);

    }
}
