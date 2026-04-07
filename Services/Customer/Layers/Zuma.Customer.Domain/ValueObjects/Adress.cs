namespace ZUMA.CustomerService.ValueObjects;

public class Address
{
    public required string Street { get; set; } = string.Empty;
    public required string City { get; set; } = string.Empty;
    public required string ZipCode { get; set; } = string.Empty;
    public required string Country { get; set; } = string.Empty;

    public string GetFullAddress() => $"{Street}, {ZipCode} {City}, {Country}";
}