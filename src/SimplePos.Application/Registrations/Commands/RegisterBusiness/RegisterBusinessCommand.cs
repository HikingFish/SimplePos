using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;

namespace SimplePos.Application.Registrations.Commands.RegisterBusiness;
public record RegisterBusinessCommand(
    string Username,
    string Email,
    string Password,
    string? PhoneNumber,
    string CompanyName,
    string Street,
    string City,
    string State,
    string PostalCode,
    string Country,
    string CompanyEmail,
    string CompanyPhoneNumber) : ICommand<Result>;