using DeskBooking.Domain;
using DeskBooking.Intefaces;
using System;
using System.Linq;

namespace DeskBooking.Processor
{
    public class DeskBookingRequestProcessor : IDeskBookingRequestProcessor
    {
        private readonly IDeskBookingRepository deskBookingRepository;
        private readonly IDeskRepositoy deskRepository;

        public DeskBookingRequestProcessor() { }

        public DeskBookingRequestProcessor(IDeskBookingRepository deskBookingRepository, IDeskRepositoy deskRepository)
        {
            this.deskBookingRepository = deskBookingRepository;
            this.deskRepository = deskRepository;
        }

        public DeskBookingResult BookDesk(DeskBookingRequest request)
        {
            if (request == null) throw new ArgumentNullException();

            var availableDesks = deskRepository.GetAvailableDesks(request.DateTime);

            var result = Create<DeskBookingResult>(request);

            if (availableDesks.FirstOrDefault() is Desk availableDesk)
            {
                var deskBooking = Create<DeskBook>(request);
                deskBooking.DeskId = availableDesk.Id;
                deskBookingRepository.Save(deskBooking);

                result.DeskBookingId = deskBooking.Id;
                result.Code = DeskBookingResultCode.Success;
            }
            else
            {
                result.Code = DeskBookingResultCode.NoDesk;
            }

            return result;
        }

        private static T Create<T>(DeskBookingRequest request) where T : DeskBookBase, new()
        {
            return new T
            {
                FirestName = request.FirestName,
                LastName = request.LastName,
                Email = request.Email,
                DateTime = request.DateTime
            };
        }
    }
}