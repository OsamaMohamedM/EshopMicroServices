namespace OrderingApplication.DTOs;

public record AddressDto(
    string FirstName,
    string LastName,
    string AddressLine,
    string EmailAddress,
    string Country,
    string State,
    string ZipCode
    );