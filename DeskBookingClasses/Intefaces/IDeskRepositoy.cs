using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeskBooking.Domain;

namespace DeskBooking.Intefaces
{
    public interface IDeskRepositoy
    {
       List<Desk> GetAvailableDesks(DateTime date);
    }
}
