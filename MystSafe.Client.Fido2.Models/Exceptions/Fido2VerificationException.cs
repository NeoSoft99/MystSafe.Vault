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
using System.Runtime.Serialization;

using Fido2NetLib.Exceptions;

namespace Fido2NetLib;

[Serializable]
public class Fido2VerificationException : Exception
{
    public Fido2VerificationException()
    {
    }

    public Fido2VerificationException(string message) : base(message)
    {
    }

    public Fido2VerificationException(Fido2ErrorCode code, string message) : base(message)
    {
        Code = code;
    }

    public Fido2VerificationException(Fido2ErrorCode code, string message, Exception innerException) : base(message, innerException)
    {
        Code = code;
    }

    public Fido2VerificationException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected Fido2VerificationException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public Fido2ErrorCode Code { get; }
}
