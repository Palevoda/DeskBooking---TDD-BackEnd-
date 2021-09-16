using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskBooking.Domain
{
    public class DeskBookBase
    {
        public string FirestName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateTime { get; set; }
    }
}
