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

using System;
using System.Text.Json.Serialization;

namespace Fido2NetLib;

/// <summary>
/// Represents a generic version with major and minor fields.
/// </summary>
/// <remarks>
/// https://fidoalliance.org/specs/fido-uaf-v1.2-rd-20171128/fido-uaf-protocol-v1.2-rd-20171128.html#version-interface
/// </remarks>
public readonly struct UafVersion : IEquatable<UafVersion>
{
    [JsonConstructor]
    public UafVersion(ushort major, ushort minor)
    {
        Major = major;
        Minor = minor;
    }

    /// <summary>
    /// Major version
    /// </summary>
    [JsonPropertyName("major")]
    public ushort Major { get; }

    /// <summary>
    /// Minor version
    /// </summary>
    [JsonPropertyName("minor")]
    public ushort Minor { get; }

    public bool Equals(UafVersion other)
    {
        return Major == other.Major
            && Minor == other.Minor;
    }

    public override bool Equals(object obj)
    {
        return obj is UafVersion other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Major, Minor);
    }

    public static bool operator ==(UafVersion left, UafVersion right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(UafVersion left, UafVersion right)
    {
        return !left.Equals(right);
    }
}
