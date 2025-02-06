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
using System.Buffers;
using System.Buffers.Text;
using System.Security.Cryptography.X509Certificates;

namespace Fido2NetLib;

internal static class X509CertificateHelper
{
    public static X509Certificate2 CreateFromBase64String(ReadOnlySpan<byte> base64String)
    {
        var rentedBuffer = ArrayPool<byte>.Shared.Rent(Base64.GetMaxDecodedFromUtf8Length(base64String.Length));

        if (Base64.DecodeFromUtf8(base64String, rentedBuffer, out _, out int bytesWritten) != OperationStatus.Done)
        {
            ArrayPool<byte>.Shared.Return(rentedBuffer, true);

            throw new Exception("Invalid base64 data found parsing X509 certificate");
        }

        try
        {
            return CreateFromRawData(rentedBuffer.AsSpan(0, bytesWritten));
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rentedBuffer, true);
        }
    }

    public static X509Certificate2 CreateFromRawData(ReadOnlySpan<byte> rawData)
    {
        try
        {
            return new X509Certificate2(rawData);
        }
        catch (Exception ex)
        {
            throw new Exception("Could not parse X509 certificate", ex);
        }
    }
}
