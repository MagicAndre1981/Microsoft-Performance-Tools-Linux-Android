﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Microsoft.Performance.SDK;
using System.Collections.Generic;

namespace PerfettoCds.Pipeline.DataOutput
{
    /// <summary>
    /// A event that represents the frequency and state of a CPU at a point in time
    /// </summary>
    public readonly struct PerfettoCpuFrequencyEvent
    {
        // The current frequency of this CPU
        public double CpuFrequency { get; }
        // The specific CPU core
        public int CpuNum { get; }
        public Timestamp StartTimestamp { get; }
        // Type of CPU frequency event. Whether it's an idle change or frequency change event
        public string Name { get; }
        public TimestampDelta Duration { get; }
        public bool IsIdle { get; }

        public PerfettoCpuFrequencyEvent(double cpuFrequency, int cpuNum, Timestamp startTimestamp, TimestampDelta duration, string name, bool isIdle)
        {
            this.CpuFrequency = cpuFrequency;
            this.CpuNum = cpuNum;
            this.StartTimestamp = startTimestamp;
            this.Duration = duration;
            this.Name = name;
            this.IsIdle = isIdle;
        }
    }
}
