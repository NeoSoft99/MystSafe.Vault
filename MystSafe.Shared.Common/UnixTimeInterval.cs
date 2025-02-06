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

using MystSafe.Shared.Crypto;

namespace MystSafe.Shared.Common;

public class UnixTimeInterval
{
    private readonly long intervalMillis;

    // value in milliseconds
    public long Value { get { return intervalMillis;  } }

    public UnixTimeInterval(long millis)
    {
        intervalMillis = millis;
    }

    public static UnixTimeInterval FromMilliseconds(int milliseconds)
    {
        return new UnixTimeInterval(milliseconds);
    }

    public static UnixTimeInterval FromSeconds(int seconds)
    {
        return new UnixTimeInterval(seconds * 1000L);
    }

    public static UnixTimeInterval FromMinutes(int minutes)
    {
        return new UnixTimeInterval(minutes * 1000L * 60);
    }

    public static UnixTimeInterval FromHours(int hours)
    {
        return new UnixTimeInterval(hours * 1000L * 60 * 60);
    }

    public static UnixTimeInterval FromDays(int days)
    {
        return new UnixTimeInterval(days * 1000L * 60 * 60 * 24);
    }

    public long BeforeNow()
    {
        return UnixDateTime.Now - intervalMillis;
    }

    public string ToLongString()
    {
        var timeRemaining = TimeSpan.FromMilliseconds(intervalMillis);
        var parts = new List<string>();
        if (timeRemaining.Days > 0) parts.Add($"{timeRemaining.Days} day{(timeRemaining.Days > 1 ? "s" : "")}");
        if (timeRemaining.Hours > 0) parts.Add($"{timeRemaining.Hours} hour{(timeRemaining.Hours > 1 ? "s" : "")}");
        if (timeRemaining.Minutes > 0) parts.Add($"{timeRemaining.Minutes} minute{(timeRemaining.Minutes > 1 ? "s" : "")}");
        if (timeRemaining.Seconds > 0) parts.Add($"{timeRemaining.Seconds} second{(timeRemaining.Seconds > 1 ? "s" : "")}");

        return parts.Count > 0 ? string.Join(" ", parts) : "0 seconds";
    }

    public string ToDisplayString()
    {
        var timeRemaining = TimeSpan.FromMilliseconds(intervalMillis);
        var parts = new List<string>();
        if (timeRemaining.Days > 0)
        {
            parts.Add($"{timeRemaining.Days} day{(timeRemaining.Days > 1 ? "s" : "")}");
            if (timeRemaining.Hours > 0) parts.Add($"{timeRemaining.Hours} hour{(timeRemaining.Hours > 1 ? "s" : "")}");
        }
        else
        if (timeRemaining.Hours > 0)
        {
            parts.Add($"{timeRemaining.Hours} hour{(timeRemaining.Hours > 1 ? "s" : "")}");
            if (timeRemaining.Minutes > 0) parts.Add($"{timeRemaining.Minutes} minute{(timeRemaining.Minutes > 1 ? "s" : "")}");
        }
        else
        {
            if (timeRemaining.Minutes > 0) parts.Add($"{timeRemaining.Minutes} minute{(timeRemaining.Minutes > 1 ? "s" : "")}");
            if (timeRemaining.Seconds > 0) parts.Add($"{timeRemaining.Seconds} second{(timeRemaining.Seconds > 1 ? "s" : "")}");
        }

        return parts.Count > 0 ? string.Join(" ", parts) : "0 seconds";
    }

    public string ToShortString()
    {
        var timeRemaining = TimeSpan.FromMilliseconds(intervalMillis);
        return $"{timeRemaining.Days}:{timeRemaining.Hours}:{timeRemaining.Minutes}:{timeRemaining.Seconds}";
    }

    public static UnixTimeInterval FromRetentionInterval(int network, Type blockType)
    {
        UnixTimeInterval retention_period;
        if (network == Networks.devnet)
        {
            if (blockType == typeof(InitBlock) || blockType == typeof(MsgBlock))
            {
                retention_period = UnixTimeInterval.FromMinutes(Constants.FREE_TRIAL_MESSAGE_RETENTION_PERIOD_DEV_MINUTES);
            }
            else if (blockType == typeof(SecretBlock) || blockType == typeof(ContactBlock))
            {
                retention_period = UnixTimeInterval.FromMinutes(Constants.FREE_TRIAL_SECRET_RETENTION_PERIOD_DEV_MINUTES);
            }
            else
            {
                throw new ApplicationException($"Unknown block data type {blockType.Name}");
            }
        }
        else
        {
            if (blockType == typeof(InitBlock) || blockType == typeof(MsgBlock))
            {
                retention_period = UnixTimeInterval.FromDays(Constants.FREE_TRIAL_MESSAGE_RETENTION_PERIOD_DAYS);
            }
            else if (blockType == typeof(SecretBlock) || blockType == typeof(ContactBlock))
            {
                retention_period = UnixTimeInterval.FromDays(Constants.FREE_TRIAL_SECRET_RETENTION_PERIOD_DAYS); 
            }
            else
            {
                throw new ApplicationException($"Unknown block data type {blockType.Name}");
            }
        }

        return retention_period;
    }

   
}
