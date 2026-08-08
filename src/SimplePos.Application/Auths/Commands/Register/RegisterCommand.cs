using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Auths.Commands.Register
{
    public record RegisterCommand(
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
        string CompanyPhoneNumber);
}
