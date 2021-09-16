using DeskBooking.Domain;
using DeskBooking.Intefaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace DeskBooking.Processor
{
    public class DeskBookingRequestProcessorTests
    {
        private readonly DeskBookingRequest request;
        private readonly List<Desk> availableDesks;
        private readonly DeskBookingRequestProcessor processor;
        private readonly Mock<IDeskBookingRepository> deskBookingRepositoryMock;
        private readonly Mock<IDeskRepositoy> deskRepositoryMock;

        public DeskBookingRequestProcessorTests()
        {
            request = new DeskBookingRequest
            {
                FirestName = "Aliaksandr",
                LastName = "Palevoda",
                Email = "alex-pale@mail.ru",
                DateTime = new DateTime(2020, 2, 1)
            };


            availableDesks = new List<Desk> { new Desk { Id = 7} };

            deskBookingRepositoryMock = new Mock<IDeskBookingRepository>();

            deskRepositoryMock = new Mock<IDeskRepositoy>();

            deskRepositoryMock.Setup(x=> x.GetAvailableDesks(request.DateTime)).Returns(availableDesks);

            processor = new DeskBookingRequestProcessor(deskBookingRepositoryMock.Object, deskRepositoryMock.Object);
        }

        [Fact]
        public void ShouldReturnDeskBookingRequest()
        {
            //Arrange


            // Act
            DeskBookingResult result = processor.BookDesk(request);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(request.FirestName, result.FirestName);
            Assert.Equal(request.LastName, result.LastName);
            Assert.Equal(request.Email, result.Email);
            Assert.Equal(request.DateTime, result.DateTime);
        }

        [Fact]
        public void ShouldThrowExceptionIfRequestIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => processor.BookDesk(null));
        }

        [Fact]
        public void ShouldSaveDeskBooking()
        {
            DeskBook savedDeskBook = null;

            deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBook>())).Callback<DeskBook>(deskBooking =>
                savedDeskBook = deskBooking
            );

            processor.BookDesk(request);

            deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBook>()), Times.Once);

            Assert.NotNull(savedDeskBook);

            Assert.Equal(request.FirestName, savedDeskBook.FirestName);
            Assert.Equal(request.LastName, savedDeskBook.LastName);
            Assert.Equal(request.Email, savedDeskBook.Email);
            Assert.Equal(request.DateTime, savedDeskBook.DateTime);
            Assert.Equal(availableDesks.First().Id, savedDeskBook.DeskId);
        }

        [Fact]
        public void ShouldNotSaveDeskBookingIfNoDeskAvailable()
        {
            availableDesks.Clear();

            processor.BookDesk(request);

            deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBook>()), Times.Never);
        }
        public enum DeskBookingResultCode
        {
            Success,
            NoDesk
        }

        [Theory]
        [InlineData(DeskBookingResultCode.Success, true)]
        [InlineData(DeskBookingResultCode.NoDesk, false)]
        public void ShouldReturnExpectedResultCode(DeskBookingResultCode expectedResult, bool isDeskAvailable)
        {
            if (!isDeskAvailable)
            {
                availableDesks.Clear();
            }
            var result = processor.BookDesk(request);

            Assert.Equal(expectedResult, (DeskBookingResultCode)result.Code);
        }

        [Theory]
        [InlineData(5, true)]
        [InlineData(null, false)]
        public void ShouldReturnExpectedDescBookingId(int? expectedDescBookingId, bool isDeskAvailable)
        {
            if (!isDeskAvailable)
            {
                availableDesks.Clear();
            }
            else
            {
                deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBook>())).Callback<DeskBook>(deskbooking => {
                    deskbooking.Id = expectedDescBookingId.Value;
                });
            }
            var result = processor.BookDesk(request);

            Assert.Equal(expectedDescBookingId, result.DeskBookingId);

        }
    }
}
