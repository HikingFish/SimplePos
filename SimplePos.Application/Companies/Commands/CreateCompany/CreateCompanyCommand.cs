using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Companies.Commands.CreateCompany
{
    public record CreateCompanyCommand(
            string Name,
            string PhoneNumber,
            string Street,
            string City,
            string State,
            string PostalCode,
            string Country,
            string Email
        ) : ICommand<Result>;
}
