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
using System.Buffers.Binary;

namespace Fido2NetLib;

/// <summary>
/// Helper functions that implements https://w3c.github.io/webauthn/#authenticator-data
/// </summary>
internal static class AuthDataHelper
{
    public static byte[] GetSizedByteArray(ReadOnlySpan<byte> ab, ref int offset, ushort len = 0)
    {
        if (len is 0 && ((offset + 2) <= ab.Length))
        {
            len = BinaryPrimitives.ReadUInt16BigEndian(ab.Slice(offset, 2));
            offset += 2;
        }
        byte[] result = null!;
        if ((0 < len) && ((offset + len) <= ab.Length))
        {
            result = ab.Slice(offset, len).ToArray();
            offset += len;
        }
        return result;
    }
}
