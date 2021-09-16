using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskBooking.Domain
{
    public class DeskBook : DeskBookBase
    {
        public int DeskId { get; set; }
        public int Id { get; set; }
    }
}
