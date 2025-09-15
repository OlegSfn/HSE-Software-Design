namespace PaymentsService.DTOs;

public class CreateAccountRequest
{
    public string UserId { get; set; }
    public decimal InitialBalance { get; set; } = 0;
}