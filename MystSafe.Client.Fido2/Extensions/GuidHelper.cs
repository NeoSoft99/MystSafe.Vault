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

namespace Fido2NetLib;

internal static class GuidHelper
{
    private static void SwapBytes(byte[] bytes, int index1, int index2)
    {
        byte temp = bytes[index1];
        bytes[index1] = bytes[index2];
        bytes[index2] = temp;
    }

    /// <summary>
    /// AAGUID is sent as big endian byte array, this converter is for little endian systems.
    /// </summary>
    public static Guid FromBigEndian(byte[] bytes)
    {
        if (!BitConverter.IsLittleEndian)
        {
            // we're already on a big-endian system, keep the bytes as is
            return new Guid(bytes);
        }

        // swap the bytes to little-endian

        SwapBytes(bytes, 0, 3);
        SwapBytes(bytes, 1, 2);
        SwapBytes(bytes, 4, 5);
        SwapBytes(bytes, 6, 7);

        return new Guid(bytes);
    }
}
