using CrimsonClans.Structs;
using ProjectM;
using System;
using System.Collections.Generic;

namespace CrimsonClans.Utilities;

public static class RaidTime
{
    private static readonly List<DayOfWeek> WeekendDays = [DayOfWeek.Saturday, DayOfWeek.Sunday];

    public static bool IsRaidTimeNow()
    {
        return IsRaidTime(DateTime.Now);
    }

    public static bool IsRaidTime(DateTime dateTime)
    {
        switch (Core.ServerGameSettingsSystem._Settings.CastleDamageMode)
        {
            case CastleDamageMode.Never:
                return false;
            case CastleDamageMode.Always:
                return true;
            case CastleDamageMode.TimeRestricted:
                var settings = Core.ServerGameSettingsSystem._Settings.PlayerInteractionSettings;
                var window = IsWeekend(dateTime) ? settings.VSCastleWeekendTime : settings.VSCastleWeekdayTime;
                return IsInWindow(dateTime, window);
            default:
                return false;
        }
    }

    private static bool IsWeekend(DateTime dateTime)
    {
        return WeekendDays.Contains(dateTime.DayOfWeek);
    }
    
    private static bool IsInWindow(DateTime dateTime, StartEndTimeData window)
    {
        var currentTime = dateTime.TimeOfDay;

        var startTime = new TimeSpan(window.StartHour, window.StartMinute, 0)
            .Subtract(TimeSpan.FromMinutes(Settings.PreRaidBuffer.Value));

        var endTime = new TimeSpan(window.EndHour, window.EndMinute, 0)
            .Add(TimeSpan.FromMinutes(Settings.PostRaidBuffer.Value));

        return currentTime >= startTime && currentTime < endTime;
    }
}