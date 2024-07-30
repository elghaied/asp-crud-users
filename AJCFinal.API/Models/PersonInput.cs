namespace AJCFinal.API.Models
{
    public class PersonInput : UserInput
    {
        
        public IEnumerable<long> FriendIds { get; set; } = new List<long>();
    }
}
