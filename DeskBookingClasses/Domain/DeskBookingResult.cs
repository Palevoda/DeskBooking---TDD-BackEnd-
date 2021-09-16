using DeskBooking.Domain;
using System;
using System.Collections.Generic;

namespace DeskBooking.Domain
{
    public class DeskBookingResult : DeskBookBase
    {
        public DeskBookingResultCode Code { get; set; }
        public int? DeskBookingId { get; set; }
    }
}