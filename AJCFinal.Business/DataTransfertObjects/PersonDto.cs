using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJCFinal.Business.DataTransfertObjects
{
    public sealed class PersonDto : UserDto
    {
        
        public IEnumerable<long> FriendIds { get; set; }
    }
}
