using SimplePos.Domain.Common;

namespace SimplePos.Domain.Companies.Events;

public record CompanyCreatedDomainEvent(Guid CompanyId, string Name) : IDomainEvent;
