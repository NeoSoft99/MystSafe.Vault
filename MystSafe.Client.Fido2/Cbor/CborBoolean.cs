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

namespace Fido2NetLib.Cbor;

public sealed class CborBoolean : CborObject
{
    public static readonly CborBoolean True = new(true);
    public static readonly CborBoolean False = new(false);

    public CborBoolean(bool value)
    {
        Value = value;
    }

    public override CborType Type => CborType.Boolean;

    public bool Value { get; }

    public override int GetHashCode()
    {
        return HashCode.Combine(Type, Value);
    }

    public static explicit operator CborBoolean(bool value) => value ? True : False;

    public static implicit operator bool(CborBoolean value) => value.Value;
}
