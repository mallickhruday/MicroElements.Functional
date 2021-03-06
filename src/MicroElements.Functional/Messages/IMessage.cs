﻿using System;
using System.Collections.Generic;

namespace MicroElements.Functional
{
    /// <summary>
    /// Represents message.
    /// Can be used as simple log message, detailed or structured log message, validation message, diagnostic message.
    /// </summary>
    public interface IMessage : IReadOnlyList<KeyValuePair<string, object>>, IReadOnlyDictionary<string, object>
    {
        /// <summary>
        /// Date and time of message created.
        /// </summary>
        DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Message severity.
        /// </summary>
        MessageSeverity Severity { get; }

        /// <summary>
        /// Message text.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// Event name.
        /// </summary>
        string EventName { get; }

        /// <summary>
        /// Optional state.
        /// </summary>
        object State { get; }

        /// <summary>
        /// Message properties.
        /// </summary>
        IReadOnlyList<KeyValuePair<string, object>> Properties { get; }
    }
}
