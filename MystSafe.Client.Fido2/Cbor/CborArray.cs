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

using System.Collections;
using System.Collections.Generic;

namespace Fido2NetLib.Cbor;

public sealed class CborArray : CborObject, IEnumerable<CborObject>
{
    public CborArray()
    {
        Values = new List<CborObject>();
    }

    public CborArray(List<CborObject> values)
    {
        Values = values;
    }

    public override CborType Type => CborType.Array;

    public int Length => Values.Count;

    public List<CborObject> Values { get; }

    public override CborObject this[int index] => Values[index];

    public void Add(CborObject value)
    {
        Values.Add(value);
    }

    public void Add(byte[] value)
    {
        Values.Add(new CborByteString(value));
    }

    public void Add(string value)
    {
        Values.Add(new CborTextString(value));
    }

    public IEnumerator<CborObject> GetEnumerator() => Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Values.GetEnumerator();
}
