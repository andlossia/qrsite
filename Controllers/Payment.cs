using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllPayments() => Ok("All Payments");

    [HttpPost]
    public IActionResult CreatePayment([FromBody] Payment payment) => Ok("Payment created");

    [HttpGet("{id}")]
    public IActionResult GetPaymentById(long id) => Ok($"Payment {id}");

    [HttpPut("{id}")]
    public IActionResult UpdatePayment(long id, [FromBody] Payment payment) => Ok($"Payment {id} updated");

    [HttpDelete("{id}")]
    public IActionResult DeletePayment(long id) => Ok($"Payment {id} deleted");
}
