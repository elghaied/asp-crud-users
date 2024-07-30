﻿using AJCFinal.DataTransfertObjects;
using System.ComponentModel.DataAnnotations;

namespace AJCFinal.Models.Person
{
    public class PersonBaseViewModel
    {
        public long Id { get; set; }
        [Display(Name ="Email")]
        public string Email { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Date of birth")]
        public DateTime DateOfBirth { get; set; }
        public IEnumerable<long>? FriendIds { get; set; } = new List<long>();

        public List<PersonBaseViewModel> Friends { get; set; } = new List<PersonBaseViewModel>();

        [Display(Name ="Profile Image")]
        public string Image {  get; set; } = string.Empty;

        public bool IsFriend { get; set; }  

        public static PersonBaseViewModel FromDto(PersonDto person)
        {
            return new PersonBaseViewModel
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                DateOfBirth = person.DateOfBirth,
                FriendIds = person.FriendIds,
                Friends = person.Friends
            };
        }
    }
}
