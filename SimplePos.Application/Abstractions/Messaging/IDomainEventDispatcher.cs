using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SimplePos.Domain.Common.DomainEvent;

namespace SimplePos.Application.Abstractions.Messaging
{
    public interface IDomainEventDispatcher
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IDomainEvent;
    }

    public  class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IDomainEvent
        {
            var handlers = _serviceProvider.GetServices<IDomainEventHandler<TEvent>>();

            var tasks = handlers.Select(handler => handler.Handle(@event, cancellationToken));
            await Task.WhenAll(tasks);
        }
    }
}
