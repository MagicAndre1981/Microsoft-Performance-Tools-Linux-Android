// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using CtfPlayback;
using CtfPlayback.EventStreams.Interfaces;
using CtfPlayback.FieldValues;
using LTTngCds.CtfExtensions.DescriptorInterfaces;
using Microsoft.Performance.SDK;
using Microsoft.Performance.SDK.Extensibility;

namespace LTTngCds.CookerData
{
    /// <summary>
    /// An LTTng event
    /// </summary>
    public class LTTngEvent
        : IKeyedDataType<string>
    {
        private readonly ICtfEvent ctfEvent;
        private readonly IEventDescriptor eventDescriptor;
        private readonly CtfTimestamp adjustedCtfTimestamp;

        internal LTTngEvent(ICtfEvent ctfEvent)
        {
            if (!(ctfEvent.EventDescriptor is IEventDescriptor eventDescriptor))
            {
                throw new CtfPlaybackException("Not a valid LTTng event.");
            }

            if (ctfEvent.Payload is null)
            {
                throw new CtfPlaybackException("LTTng event payload is null.");
            }

            if (!(ctfEvent.Payload is CtfStructValue payload))
            {
                throw new CtfPlaybackException("LTTng event payload is not a structure.");
            }

            this.ctfEvent = ctfEvent;
            this.eventDescriptor = eventDescriptor;
        }

        internal LTTngEvent(LTTngEvent lttngEvent, long timestampAdjustmentNano) : this(lttngEvent.ctfEvent)
        {
            long adjustedTimestamp = Math.Max(0, (long)lttngEvent.ctfEvent.Timestamp.NanosecondsFromClockBase + timestampAdjustmentNano);
            CtfTimestamp origCtfTimestamp = lttngEvent.ctfEvent.Timestamp;
            this.adjustedCtfTimestamp = new CtfTimestamp(origCtfTimestamp.BaseIntegerValue, adjustedTimestamp, origCtfTimestamp.ClockDescriptor, origCtfTimestamp.ClockName);
        }

        /// <summary>
        /// Event Id
        /// </summary>
        public uint Id => this.eventDescriptor.Id;

        /// <summary>
        /// Event name
        /// </summary>
        public string Name => this.eventDescriptor.Name;

        /// <summary>
        /// Event timestamp. Nanoseconds from the CTF clock the timestamp is associated with.
        /// </summary>
        public Timestamp Timestamp => new Timestamp((long)(adjustedCtfTimestamp?.NanosecondsFromClockBase ?? this.ctfEvent.Timestamp.NanosecondsFromClockBase));

        /// <summary>
        /// Event time as a wall clock.
        /// </summary>
        public DateTime WallClockTime => adjustedCtfTimestamp?.GetDateTime() ?? this.ctfEvent.Timestamp.GetDateTime();

        /// <summary>
        /// CTF timestamp
        /// </summary>
        public CtfTimestamp CtfTimestamp => adjustedCtfTimestamp ?? this.ctfEvent.Timestamp;

        /// <summary>
        /// Event header as defined in the stream for this event.
        /// </summary>
        public CtfFieldValue StreamDefinedEventHeader => this.ctfEvent.StreamDefinedEventHeader;

        /// <summary>
        /// Event context as defined in the stream for this event.
        /// </summary>
        public CtfStructValue StreamDefinedEventContext => this.ctfEvent.StreamDefinedEventContext as CtfStructValue;

        /// <summary>
        /// Event payload
        /// </summary>
        public CtfStructValue Payload => this.ctfEvent.Payload as CtfStructValue;

        /// <summary>
        /// Size of the event payload, in bits.
        /// </summary>
        public uint PayloadBitCount => this.ctfEvent.PayloadBitCount;

        /// <summary>
        /// Number of discarded events so far.
        /// </summary>
        public uint DiscardedEvents => this.ctfEvent.DiscardedEvents;

        /// <inheritdoc />
        public int CompareTo(string name)
        {
            return StringComparer.InvariantCulture.Compare(this.Name, name);
        }

        /// <summary>
        /// The key to this event, for distribution to registered source cookers.
        /// </summary>
        /// <returns>Event key</returns>
        public string GetKey()
        {
            return this.Name;
        }
    }
}
