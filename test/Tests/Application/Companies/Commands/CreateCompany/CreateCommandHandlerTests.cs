using SimplePos.Application.Abstractions;
using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Companies.Commands.CreateCompany;
using SimplePos.Domain.Common;
using SimplePos.Domain.Common.DomainEvent;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Companies;
using SimplePos.Domain.Companies.Events;
using Xunit;
using NSubstitute;

namespace Tests.Application.Companies.Commands.CreateCompany;

public class CreateCommandHandlerTests
{
    private readonly IDomainEventDispatcher _domainEventDispatcherMock;
    private readonly ICompanyRepository _companyRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CreateCompanyCommandHandler _handler;

    public CreateCommandHandlerTests()
    {
        _domainEventDispatcherMock = Substitute.For<IDomainEventDispatcher>();
        _companyRepositoryMock = Substitute.For<ICompanyRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _handler = new CreateCompanyCommandHandler(
            _domainEventDispatcherMock,
            _companyRepositoryMock,
            _unitOfWorkMock
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccess_AndPublishEvent_WhenCommandIsValid()
    {
        // Arrange
        var command = new CreateCompanyCommand(
            Name: "Acme Corp",
            PhoneNumber: "012345678",
            Street: "123 Main St",
            City: "Kuala Lumpur",
            State: "WP",
            PostalCode: "50450",
            Country: "Malaysia",
            Email: "info@acme.com"
        );
        // Act
        Result result = await _handler.HandleAsync(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsSuccess);
        // Verify: The event dispatcher received a call to publish the CompanyCreatedDomainEvent
        await _domainEventDispatcherMock.Received(1).PublishAsync(
            Arg.Is<IDomainEvent>(e => e is CompanyCreatedDomainEvent && ((CompanyCreatedDomainEvent)e).Name == command.Name),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new CreateCompanyCommand(
            Name: "Acme Corp",
            PhoneNumber: "012345678",
            Street: "123 Main St",
            City: "Kuala Lumpur",
            State: "WP",
            PostalCode: "50450",
            Country: "Malaysia",
            Email: "invalid-email" // Invalid email address format
        );
        // Act
        Result result = await _handler.HandleAsync(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsFailure);
        
        // Verify: Event dispatcher was NOT called because validation failed early
        await _domainEventDispatcherMock.DidNotReceive().PublishAsync(
            Arg.Any<IDomainEvent>(),
            Arg.Any<CancellationToken>()
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFailure_WhenPhoneNumberIsInvalid()
    {
        // Arrange
        var command = new CreateCompanyCommand(
            Name: "Acme Corp",
            PhoneNumber: "invalid-phone",
            Street: "123 Main St",
            City: "Kuala Lumpur",
            State: "WP",
            PostalCode: "50450",
            Country: "Malaysia",
            Email: "info@acme.com"
        );
        // Act
        Result result = await _handler.HandleAsync(command, CancellationToken.None);
        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(PhoneNumberError.InvalidFormat, result.Error);
        
        // Verify: Event dispatcher was NOT called because validation failed early
        await _domainEventDispatcherMock.DidNotReceive().PublishAsync(
            Arg.Any<IDomainEvent>(),
            Arg.Any<CancellationToken>()
        );
    }
}