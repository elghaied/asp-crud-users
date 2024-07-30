using AJCFinal.DataTransfertObjects;

namespace AJCFinal.Services.Abstractions
{
    public interface IPersonService
    {
        Task<IEnumerable<PersonDto>> GetPersonsAsync();
        Task<PersonDto> GetPersonAsync(int id);
        Task<string> GetPersonImageAsync(string imageName);
        Task<bool> AddFriendAsync(long userId, long friendId);
    }
}
