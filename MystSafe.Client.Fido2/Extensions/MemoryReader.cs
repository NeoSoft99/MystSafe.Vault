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

internal ref struct MemoryReader
{
    public int _position;
    public readonly ReadOnlySpan<byte> _buffer;

    public MemoryReader(ReadOnlySpan<byte> buffer)
    {
        _buffer = buffer;
        _position = 0;
    }

    public int Position => _position;

    public void Advance(int count)
    {
        _position += count;
    }

    public uint ReadUInt32BigEndian()
    {
        var result = BinaryPrimitives.ReadUInt32BigEndian(_buffer.Slice(_position, 4));

        _position += 4;

        return result;
    }

    public byte[] ReadBytes(int count)
    {
        byte[] result = _buffer.Slice(_position, count).ToArray();

        _position += count;

        return result;
    }

    public byte ReadByte()
    {
        byte result = _buffer.Slice(_position)[0];

        _position += 1;

        return result;
    }

    public int RemainingBytes => _buffer.Length - _position;
}
