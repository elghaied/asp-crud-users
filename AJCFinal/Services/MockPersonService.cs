using AJCFinal.DataTransfertObjects;
using AJCFinal.Services.Abstractions;

namespace AJCFinal.Services
{
    public class MockPersonService : IPersonService
    {
        public Task<IEnumerable<PersonDto>> GetPersonsAsync()
        {
            var persons = new List<PersonDto>
        {
            new PersonDto { Id = 1, FirstName = "John", LastName = "Doe" },
            new PersonDto { Id = 2, FirstName = "Jane", LastName = "Smith" }
        };
            return Task.FromResult((IEnumerable<PersonDto>)persons);
        }

        public Task<PersonDto> GetPersonAsync(int id)
        {
            var person = new PersonDto { Id = id, FirstName = "John", LastName = "Doe" };
            return Task.FromResult(person);
        }

        public Task<string> GetPersonImageAsync(string imageName)
        {
            return Task.FromResult("/images/Default_pfp.jpg");
        }

        public Task<bool> AddFriendAsync(long userId, long friendId)
        {
            return Task.FromResult(true);
        }
    }
}
