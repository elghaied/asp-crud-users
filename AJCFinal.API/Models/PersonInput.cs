namespace AJCFinal.API.Models
{
    public class PersonInput
    {
        
        public IEnumerable<long> FriendIds { get; set; } = new List<long>();
    }
}
