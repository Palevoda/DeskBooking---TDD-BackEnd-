using DeskBooking.Domain;
using DeskBooking.Processor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeskBookingWeb.Pages
{
    public class BookDeskModel : PageModel
    {
        private IDeskBookingRequestProcessor _deskBookingRequestProcessor;

        public BookDeskModel(IDeskBookingRequestProcessor deskBookingRequestProcessor)
        {
            _deskBookingRequestProcessor = deskBookingRequestProcessor;
        }

        [BindProperty]
        public DeskBookingRequest deskBookingRequest { get; set; }

        public void OnGet()
        {
        }
        public IActionResult OnPost()
        {
            IActionResult actionResult = Page();

            if (ModelState.IsValid)
            {
                var result = _deskBookingRequestProcessor.BookDesk(deskBookingRequest);
                if (result.Code == DeskBookingResultCode.Success)
                {
                    actionResult = RedirectToPage("BookDeskConfirmation", new { 
                        result.DeskBookingId,
                        result.FirestName,
                        result.DateTime
                    });
                }
                else if (result.Code == DeskBookingResultCode.NoDesk)
                {
                    ModelState.AddModelError("DeskBookingRequest.Date", "No desk available for selected date");
                }
            }

            return actionResult;
        }
    }
}
