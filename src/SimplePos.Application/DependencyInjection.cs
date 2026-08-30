using Microsoft.Extensions.DependencyInjection;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Domain.Common.DomainEvent;
using SimplePos.Domain.Companies.Events;
using SimplePos.Application.Outlets.Subscribers;
using SimplePos.Application.Companies.Commands.CreateCompany;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Application.Abstractions;
using SimplePos.Application.Companies.Queries;
using SimplePos.Application.Registrations.Commands.RegisterBusiness;
using SimplePos.Application.Auths.Commands.Login;
using SimplePos.Application.Auths.Queries;

using SimplePos.Application.Users.Queries.GetOutletAccess;
using SimplePos.Application.Users.Commands.GrantOutletAccess;
using SimplePos.Application.Users.Commands.SetOutletAccess;
using SimplePos.Application.Users.Commands.RevokeOutletAccess;

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

        services.AddScoped<ICommandHandler<RegisterBusinessCommand, Result>, RegisterBusinessCommandHandler>();

        services.AddScoped<ICommandHandler<LoginCommand, Result<string>>, LoginCommandHandler>();
        services.AddScoped<IQueryHandler<GetCurrentUserQuery, Result<CurrentUserResponse>>, GetCurrentUserQueryHandler>();

        // User Outlet Access Commands and Queries
        services.AddScoped<IQueryHandler<GetUserOutletAccessesQuery, Result<List<UserOutletAccessResponse>>>, GetUserOutletAccessesQueryHandler>();
        services.AddScoped<ICommandHandler<GrantUserOutletAccessCommand, Result>, GrantUserOutletAccessCommandHandler>();
        services.AddScoped<ICommandHandler<SetUserOutletAccessesCommand, Result>, SetUserOutletAccessesCommandHandler>();
        services.AddScoped<ICommandHandler<RevokeUserOutletAccessCommand, Result>, RevokeUserOutletAccessCommandHandler>();

        return services;
    }
}