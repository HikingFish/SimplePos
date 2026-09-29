using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Outlets.Commands.CreateOutlet
{
    public record CreateOutletCommand(
        Guid CompanyId,
        string Name,
        string Street,
        string City,
        string State,
        string PostalCode,
        string Country,
        string phoneNumber
        ) : ICommand;
}
