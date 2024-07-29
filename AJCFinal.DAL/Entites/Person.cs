namespace AJCFinal.DAL.Entites
{
    public sealed class Person : User
    {
       public ICollection<Person> Friends { get; set; } 
    }
}
