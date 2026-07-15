using System;
using System.Collections.Generic;
using System.Text;

namespace SimplePos.Application.Abstractions.Messaging;
public interface IEvent { }

public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    Task Handle(TEvent @event, CancellationToken cancellationToken = default);
}
