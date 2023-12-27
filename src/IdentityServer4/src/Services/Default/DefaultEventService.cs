// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Configuration;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace IdentityServer4.Events;

/// <summary>
/// The default event service
/// </summary>
/// <seealso cref="IEventService" />
public class DefaultEventService : IEventService
{
    /// <summary>
    /// The options
    /// </summary>
    protected readonly IdentityServerOptions Options;

    /// <summary>
    /// The context
    /// </summary>
    protected readonly IHttpContextAccessor Context;

    /// <summary>
    /// The sink
    /// </summary>
    protected readonly IEventSink Sink;

    /// <summary>
    /// The clock
    /// </summary>
    protected readonly ISystemClock Clock;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultEventService"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="context">The context.</param>
    /// <param name="sink">The sink.</param>
    /// <param name="clock">The clock.</param>
    public DefaultEventService(IdentityServerOptions options, IHttpContextAccessor context, IEventSink sink, ISystemClock clock)
    {
        Options = options;
        Context = context;
        Sink = sink;
        Clock = clock;
    }

    /// <summary>
    /// Raises the specified event.
    /// </summary>
    /// <param name="evt">The event.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">evt</exception>
    public async Task RaiseAsync(Event evt)
    {
        if (evt == null) throw new ArgumentNullException(nameof(evt));

        if (CanRaiseEvent(evt))
        {
            await PrepareEventAsync(evt);
            await Sink.PersistAsync(evt);
        }
    }

    /// <summary>
    /// Indicates if the type of event will be persisted.
    /// </summary>
    /// <param name="evtType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public bool CanRaiseEventType(EventTypes evtType)
    {
        return evtType switch
        {
            EventTypes.Failure => Options.Events.RaiseFailureEvents,
            EventTypes.Information => Options.Events.RaiseInformationEvents,
            EventTypes.Success => Options.Events.RaiseSuccessEvents,
            EventTypes.Error => Options.Events.RaiseErrorEvents,
            _ => throw new ArgumentOutOfRangeException(nameof(evtType)),
        };
    }

    /// <summary>
    /// Determines whether this event would be persisted.
    /// </summary>
    /// <param name="evt">The evt.</param>
    /// <returns>
    ///   <c>true</c> if this event would be persisted; otherwise, <c>false</c>.
    /// </returns>
    protected virtual bool CanRaiseEvent(Event evt)
    {
        return CanRaiseEventType(evt.EventType);
    }

    /// <summary>
    /// Prepares the event.
    /// </summary>
    /// <param name="evt">The evt.</param>
    /// <returns></returns>
    protected virtual async Task PrepareEventAsync(Event evt)
    {
        evt.ActivityId = Context.HttpContext.TraceIdentifier;
        evt.TimeStamp = Clock.UtcNow.DateTime;
        evt.ProcessId = Environment.ProcessId;

        if (Context.HttpContext?.Connection.LocalIpAddress != null)
        {
            evt.LocalIpAddress = Context.HttpContext.Connection.LocalIpAddress.ToString() + ":" + Context.HttpContext.Connection.LocalPort;
        }
        else
        {
            evt.LocalIpAddress = "unknown";
        }

        if (Context.HttpContext?.Connection.RemoteIpAddress != null)
        {
            evt.RemoteIpAddress = Context.HttpContext.Connection.RemoteIpAddress.ToString();
        }
        else
        {
            evt.RemoteIpAddress = "unknown";
        }

        await evt.PrepareAsync();
    }
}