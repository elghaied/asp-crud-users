﻿namespace AJCFinal.API.Models
{
    public abstract class UserInput
    {
        public long Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string HashedPassword { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
