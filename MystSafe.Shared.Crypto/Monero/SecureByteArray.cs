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

using System.Text;

namespace MystSafe.Shared.Crypto;

using System;
using System.Runtime.InteropServices;
using System.Security;

public class SecureByteArray : IDisposable
{
    private readonly SecureString secureString;

    public SecureByteArray(byte[] input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        secureString = ConvertToSecureString(input);
        secureString.MakeReadOnly();
    }

    public byte[] GetBytes()
    {
        return ConvertToByteArray(secureString);
    }
    
    private SecureString ConvertToSecureString(byte[] input)
    {
        SecureString secureStr = new SecureString();
        string base58 = Codecs.FromBytesToBase58(input);
        foreach (char b in base58)
        {
            secureStr.AppendChar(b);
        }
        secureStr.MakeReadOnly();
        return secureStr;
    }

    private byte[] ConvertToByteArray(SecureString secureStr)
    {

        if (secureStr == null)
            throw new ArgumentNullException(nameof(secureStr));
        

        IntPtr unmanagedString = IntPtr.Zero;
        try
        {
            unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureStr);
            string s = Marshal.PtrToStringUni(unmanagedString);
            return Codecs.FromBase58ToBytes(s);
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
        }
        
        
    }

    public void Dispose()
    {
        secureString?.Dispose();
    }
}