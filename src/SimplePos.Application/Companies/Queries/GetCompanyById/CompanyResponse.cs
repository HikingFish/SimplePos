using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Companies.Queries.GetCompanyById
{
    public record CompanyResponse(
        Guid CompanyId,
        string Name,
        string PhoneNumber,
        string Street,
        string City,
        string State,
        string PostalCode,
        string Country,
        string Email);
}
