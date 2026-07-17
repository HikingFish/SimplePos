using SimplePos.Domain.Common;
using SimplePos.Domain.Common.DomainEvent;

namespace SimplePos.Domain.Companies.Events;

public record CompanyCreatedDomainEvent(Guid CompanyId, string Name, Address Address, string PhoneNumber) : IDomainEvent;
