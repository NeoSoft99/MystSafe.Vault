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

using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.Abstractions;
using System.Text;

namespace MystSafe.Shared.Crypto;


public abstract class Key
{
    protected const int KEY_SIZE = 32;

    protected byte[] _encryptedKeyBytes;
    private static readonly IDataProtector _protector = GetDataProtector();

    public Key(byte[] keyBytes)
    {
        if (keyBytes is null)
            throw new ArgumentNullException("Key array cannot be null");
        if (keyBytes.Length != KEY_SIZE)
            throw new ArgumentOutOfRangeException("Wrong key array length");

        _encryptedKeyBytes = Protect(keyBytes);
    }

    public byte[] ToBytes()
    {
        return Unprotect(_encryptedKeyBytes);
    }

    private static IDataProtector GetDataProtector()
    {
        var dataProtectionProvider = DataProtectionProvider.Create("MystSafe.Shared.Crypto.Key");
        return dataProtectionProvider.CreateProtector("KeyProtector");
    }

    private static byte[] Protect(byte[] data)
    {
        return _protector.Protect(data);
    }

    private static byte[] Unprotect(byte[] encryptedData)
    {
        return _protector.Unprotect(encryptedData);
    }
}


/*public abstract class Key
{
    protected const int KEY_SIZE = 32;
    
    protected byte[] _keyBytes;
    
    public Key(byte[] keyBytes)
    {
        if (keyBytes is null)
            throw new ArgumentNullException("Key array cannot be null");
        if (keyBytes.Length != KEY_SIZE)
            throw new ArgumentOutOfRangeException("Wrong key array length");
        _keyBytes = new byte[KEY_SIZE];
        Array.Copy(keyBytes, 0, _keyBytes, 0, KEY_SIZE);
    }
    
    public byte[] ToBytes()
    {
        var keyBytes = new byte[KEY_SIZE];
        Array.Copy(_keyBytes, 0, keyBytes, 0, KEY_SIZE);
        return keyBytes;
    }

}*/