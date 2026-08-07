using Microsoft.Extensions.DependencyInjection;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.DomainEvent;
using SimplePos.Domain.Companies.Events;
using SimplePos.Application.Outlets.Subscribers;
using SimplePos.Application.Companies.Commands.CreateCompany;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Application.Abstractions;
using SimplePos.Application.Companies.Queries;

namespace SimplePos.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICqrsDispatcher, CqrsDispatcher>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IDomainEventHandler<CompanyCreatedDomainEvent>, SetupDefaultOutlet>();

        //Company Commands and Queries
        services.AddScoped<ICommandHandler<CreateCompanyCommand, Result>, CreateCompanyCommandHandler>();

        services.AddScoped<IQueryHandler<GetCompanyByIdQuery, Result<CompanyResponse>>, GetCompanyByIdQueryHandler>();

        return services;
    }
}