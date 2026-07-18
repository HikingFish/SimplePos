using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SimplePos.Domain.Common.DomainEvent;

namespace SimplePos.Application.Abstractions.Messaging
{
    public interface IDomainEventDispatcher
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IDomainEvent;
        Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken = default);
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

        public async Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
        {
            var eventType = @event.GetType();
            //IDomainEventHandler<IDomainEvent>
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType); 
            var handlers = _serviceProvider.GetServices(handlerType);
            MethodInfo? handleMethod = handlerType.GetMethod("Handle");
            foreach (object? handler in  handlers)
            {
                if (handleMethod != null)
                {
                    await (Task)handleMethod.Invoke(handler, [@event, cancellationToken])!;
                }
            }
        }
    }
}
