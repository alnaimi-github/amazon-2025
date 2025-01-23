namespace API.DTOs;

public record CreateProductDto(
    [Required] string Name,
    string Description,
    [Required, Range(0.01, double.MaxValue, ErrorMessage = "The price must be greater than 0")] 
    decimal Price,
    [Required] 
    string PictureUrl,
    [Required] 
    string Type,
    [Required] 
    string Brand,
    [Range(1,int.MaxValue,ErrorMessage ="Quantity in stock must be al least 1")]
    int QuantityInStock
);