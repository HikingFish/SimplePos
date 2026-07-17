using SimplePos.Application.Abstractions.Messaging;
using SimplePos.Application.Companies.Commands.CreateCompany;
using SimplePos.Domain.Common.ResultPattern;
using SimplePos.Domain.Companies.Events;
using Xunit;
using NSubstitute;

public class CreateCompanyCommandHandlerTests
{
    private readonly IDomainEventDispatcher _domainEventDispatcherMock;
    private readonly CreateCompanyCommandHandler _handler;
    public CreateCompanyCommandHandlerTests()
    {
        // 1. Mock only the Event Dispatcher using NSubstitute
        _domainEventDispatcherMock = Substitute.For<IDomainEventDispatcher>();
        // 2. Instantiate the Handler
        _handler = new CreateCompanyCommandHandler(_domainEventDispatcherMock);
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
            Arg.Is<CompanyCreatedDomainEvent>(e => e.Name == command.Name),
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
            Arg.Any<CompanyCreatedDomainEvent>(),
            Arg.Any<CancellationToken>()
        );
    }
}