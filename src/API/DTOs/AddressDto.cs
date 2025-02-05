namespace API.DTOs;

public class AddressDto
{
    [Required]
     public string Line1 { get; set; } = default!;
    public string? Line2 { get; set; }
    [Required]
    public string City { get; set; } = default!;
    [Required]
    public string State { get; set; } = default!;
    [Required]
    public string PostalCode { get; set; } = default!;
    [Required]
    public string Country { get; set; } = default!;
}