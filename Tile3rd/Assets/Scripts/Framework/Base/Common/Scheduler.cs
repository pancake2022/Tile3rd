using System;
using System.Collections;
using System.Collections.Generic;

namespace CSFramework
{
    public class SchedulerFloat
    {
        public float EscapeValue;
        public float MaxValue;
    }

    public static class SchedulerFloatExtension
    {
        public static SchedulerFloat Init (this SchedulerFloat scheduler, float max_value)
        {
            scheduler.MaxValue = max_value;
            scheduler.EscapeValue = 0;
            return scheduler;
        }

        public static bool Tick (this SchedulerFloat scheduler, float dt, bool auto_reset = true)
        {
            if (scheduler == null || scheduler.MaxValue <= 0)
                return false;

            var escape_value = scheduler.EscapeValue;
            var max_value = scheduler.MaxValue;
            var end_tick = true;
            if (escape_value < max_value)
            {
                escape_value += dt;
                if (escape_value < max_value)
                    end_tick = false;

                scheduler.EscapeValue = escape_value;
            }
            if (end_tick && auto_reset)
                scheduler.Reset();
            return end_tick;
        }
        public static SchedulerFloat Reset (this SchedulerFloat scheduler)
        {
            scheduler.EscapeValue = 0;
            return scheduler;
        }
        public static SchedulerFloat SetArrived (this SchedulerFloat scheduler)
        {
            scheduler.EscapeValue = scheduler.MaxValue;
            return scheduler;
        }
        public static SchedulerFloat Sub (this SchedulerFloat scheduler, float value)
        {
            scheduler.EscapeValue = Math.Max(0, scheduler.EscapeValue - value);
            return scheduler;
        }
        public static float Percent (this SchedulerFloat scheduler)
        {
            var percent = (float)(scheduler.EscapeValue / scheduler.MaxValue);
            if (percent < 0)
                percent = 0;
            else if (percent > 1)
                percent = 1;
            return percent;
        }
        public static bool IsArrived (this SchedulerFloat scheduler)
        {
            return scheduler.EscapeValue >= scheduler.MaxValue;
        }
    }
}