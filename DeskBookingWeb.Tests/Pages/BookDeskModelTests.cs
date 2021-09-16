using DeskBooking.Processor;
using DeskBookingWeb.Pages;
using Moq;
using Xunit;
using DeskBooking.Domain;
using System;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace DeskBookingWeb.Tests.Pages
{
    public class BookDeskModelTests
    {
        private Mock<IDeskBookingRequestProcessor> _procesorMock;
        private BookDeskModel _bookDeskModel;
        private DeskBookingResult _deskBookingResult;

        public BookDeskModelTests()
        {
            _procesorMock = new Mock<IDeskBookingRequestProcessor>();

            _bookDeskModel = new BookDeskModel(_procesorMock.Object)
            {
                deskBookingRequest = new DeskBookingRequest()
            };

            _deskBookingResult = new DeskBookingResult()
            {
                Code = DeskBookingResultCode.Success
            };

            _procesorMock.Setup(x => x.BookDesk(_bookDeskModel.deskBookingRequest)).Returns(_deskBookingResult);

        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(0, false)]
        public void ShouldCallBookDeskMethodFromProcessor(int expectedBookDeskCalls, bool isModelValid)
        {
            //Arrange
            _deskBookingResult.Code = DeskBookingResultCode.Success;
            if (!isModelValid)
            {
                _bookDeskModel.ModelState.AddModelError("JustAKey", "AnErrorMessage");
            }

            //Act
            _bookDeskModel.OnPost();

            //Assert
            _procesorMock.Verify(x => x.BookDesk(_bookDeskModel.deskBookingRequest), Times.Exactly(expectedBookDeskCalls));

        }

        [Fact]
        public void ShouldAddModelErrorIfNoDeskAvailable()
        {
            //Arrange
            _deskBookingResult.Code = DeskBookingResultCode.NoDesk;

            //Act
            _bookDeskModel.OnPost();

            //Assert

            var ModelStateEntry = Assert.Contains("DeskBookingRequest.Date", _bookDeskModel.ModelState);

            var ModalError = Assert.Single(ModelStateEntry.Errors);

            Assert.Equal("No desk available for selected date", ModalError.ErrorMessage);
        }

        [Fact]
        public void ShouldNotAddModelErrorDeskAvailable()
        {
            //Arrange
            _deskBookingResult.Code = DeskBookingResultCode.Success;

            //Act
            _bookDeskModel.OnPost();

            //Assert
            Assert.DoesNotContain("DeskBookingRequest.Date", _bookDeskModel.ModelState);
        }

        [Theory]
        [InlineData(typeof(PageResult), false, null)]
        [InlineData(typeof(PageResult), true, DeskBookingResultCode.NoDesk)]
        [InlineData(typeof(RedirectToPageResult), true, DeskBookingResultCode.Success)]
        public void ShouldReturnExpectedActionResult(Type expectedActionResultType, bool isModelValid, DeskBookingResultCode? deskBookingResultCode)
        {
            if (!isModelValid)
            {
                _bookDeskModel.ModelState.AddModelError("JustAKey", "AnErrorMessage");
            }

            if (deskBookingResultCode.HasValue)
            {
                _deskBookingResult.Code = deskBookingResultCode.Value;
            }

            IActionResult actionResult = _bookDeskModel.OnPost();

            Assert.IsType(expectedActionResultType, actionResult);

        }

        [Fact]
        public void ShouldRedirectToBookDeskConfirmationPage()
        {
            //Arrange
            _deskBookingResult.Code = DeskBookingResultCode.Success;
            _deskBookingResult.DeskBookingId = 7;
            _deskBookingResult.FirestName = "Thomas";
            _deskBookingResult.DateTime = new DateTime(2021, 9, 12);

            //Act
            IActionResult actionResult = _bookDeskModel.OnPost();

            //Assert
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(actionResult);
            Assert.Equal("BookDeskConfirmation", redirectToPageResult.PageName);

            var routeValues = redirectToPageResult.RouteValues;
            Assert.Equal(3, routeValues.Count);

            //var deskBookingId = Assert.Contains("DeskBookingId", routeValues);
            //Assert.Equal(_deskBookingResult.DeskBookingId, deskBookingId);

            //var firestName = Assert.Contains("firstName", routeValues);
            //Assert.Equal(_deskBookingResult.DeskBookingId, deskBookingId);
        }

    }
}
