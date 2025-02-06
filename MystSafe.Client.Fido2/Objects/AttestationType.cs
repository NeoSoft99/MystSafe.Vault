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

using Fido2NetLib.Serialization;

namespace Fido2NetLib.Objects;

[JsonConverter(typeof(AttestationTypeConverter))]
public sealed class AttestationType : IEquatable<AttestationType>
{
    public static readonly AttestationType None = new("none");
    public static readonly AttestationType Basic = new("basic");
    public static readonly AttestationType Self = new("self");
    public static readonly AttestationType AttCa = new("attca");
    public static readonly AttestationType ECDAA = new("ecdaa");

    private readonly string _value;

    internal AttestationType(string value)
    {
        _value = value;
    }

    public string Value => _value;

    public static implicit operator string(AttestationType op) { return op.Value; }

    public static bool operator ==(AttestationType e1, AttestationType e2)
    {
        if (e1 is null)
            return e2 is null;

        return e1.Equals(e2);
    }

    public static bool operator !=(AttestationType e1, AttestationType e2)
    {
        return !(e1 == e2);
    }

    public override bool Equals(object? obj)
    {
        return obj is AttestationType other && Equals(other);
    }

    public bool Equals(AttestationType? other)
    {
        if (ReferenceEquals(this, other))
            return true;

        if (other is null)
            return false;

        return string.Equals(Value, other.Value, StringComparison.Ordinal);
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    internal static AttestationType Get(string value)
    {
        return value switch
        {
            "none" => None,
            "basic" => Basic,
            "self" => Self,
            "attca" => AttCa,
            "ecdaa" => ECDAA,
            _ => new AttestationType(value)
        };
    }
}
