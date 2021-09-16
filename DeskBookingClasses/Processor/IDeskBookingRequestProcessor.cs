using DeskBooking.Domain;

namespace DeskBooking.Processor
{
    public interface IDeskBookingRequestProcessor
    {
        DeskBookingResult BookDesk(DeskBookingRequest request);
    }
}