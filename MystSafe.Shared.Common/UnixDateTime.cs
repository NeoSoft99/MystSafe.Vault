// MystSafe is a secret vault with anonymous access and zero activity tracking protected by cryptocurrency-grade tech.
// 
//     Copyright (C) 2024-2025 MystSafe, NeoSoft99
// 
//     MystSafe: The Only Privacy-Preserving Password Manager.
//     https://mystsafe.com
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU Affero General Public License as
//     published by the Free Software Foundation, either version 3 of the
//     License, or (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
//     See the GNU Affero General Public License for more details.
// 
//     You should have received a copy of the GNU Affero General Public License
//     along with this program.  If not, see <https://www.gnu.org/licenses/>.

namespace MystSafe.Shared.Common;

public static class UnixDateTime
{
    public static long Now
    {
        get { return FromDateTime(DateTime.UtcNow); }
    }

    public static long FromDateTime(DateTime dateTime)
    {
        long epochTicks = new DateTime(1970, 1, 1).Ticks;
        long nowTicks = dateTime.ToUniversalTime().Ticks;
        // Using TimeSpan.TicksPerMillisecond for millisecond precision
        return (nowTicks - epochTicks) / TimeSpan.TicksPerMillisecond;
    }

    public static DateTime ToDateTime(long unixTimeMillis)
    {
        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        // Using AddMilliseconds for millisecond precision
        return dtDateTime.AddMilliseconds(unixTimeMillis).ToLocalTime();
    }

    public static long AddSeconds(long unixDateTime, int intervalSeconds)
    {
        // Convert seconds to milliseconds
        long intervalMillis = intervalSeconds * 1000;

        // Add the interval to the Unix time
        return unixDateTime + intervalMillis;
    }

    public static long AddMinutes(long unixDateTime, int intervalMinutes)
    {
        // Convert seconds to milliseconds      1s     1m   
        long intervalMillis = intervalMinutes * 1000 * 60;

        // Add the interval to the Unix time
        return unixDateTime + intervalMillis;
    }

    public static long AddDays(long unixDateTime, int intervalDays)
    {
        // Convert seconds to milliseconds    1s    1m    1h   1d
        long intervalMillis = intervalDays * 1000 * 60 * 60 * 24;

        // Add the interval to the Unix time
        return unixDateTime + intervalMillis;
    }

    public static long AddInterval(long unixDateTime, UnixTimeInterval interval)
    {
        // Add the interval to the Unix time
        return unixDateTime + interval.Value;
    }

    // point of time in the past when deletion stops, in UnixDateTime (ms)
    public static long DeletionThreshold(int network, Type blockType)
    {
        UnixTimeInterval retention_period = UnixTimeInterval.FromRetentionInterval(network, blockType);
        return retention_period.BeforeNow();
    }

    // point of time in the past when deletion stops, in UnixDateTime (ms)
    public static long DeletionThreshold(UnixTimeInterval retentionInterval)
    {
        return retentionInterval.BeforeNow();
    }

    public static string ToLongString(long unixDateTime)
    {
        return string.Format("{0} {1}", UnixDateTime.ToDateTime(unixDateTime).ToShortDateString(), UnixDateTime.ToDateTime(unixDateTime).ToLongTimeString());
    }
}
