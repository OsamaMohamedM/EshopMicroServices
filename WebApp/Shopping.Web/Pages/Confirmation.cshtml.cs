using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Shopping.Web.Pages
{
    public class ConfirmationModel(ILogger<ConfirmationModel> logger)
        : PageModel
    {
        public string Message { get; set; } = default!;

        public void OnGet(string? message = null)
        {
            logger.LogInformation("Confirmation page accessed.");
            Message = !string.IsNullOrEmpty(message) ? message : "Your order has been submitted successfully.";
        }

        public void OnGetOrderSubmitted()
        {
            logger.LogInformation("Order submitted confirmation displayed.");
            Message = "Your order submitted successfully.";
        }

        public void OnGetContact()
        {
            logger.LogInformation("Contact confirmation displayed.");
            Message = "Your email was sent.";
        }
    }
}
