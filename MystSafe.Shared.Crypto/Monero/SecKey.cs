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

using System.Security;
using MoneroRing.Crypto;
using MoneroSharp.NaCl.Internal.Ed25519Ref10;
using Microsoft.AspNetCore.DataProtection;
using MoneroSharp.NaCl;

namespace MystSafe.Shared.Crypto;

public class SecKey: IDisposable
{
    private byte[]? _encryptedKeyBytes;
    private static readonly IDataProtector _protector = GetDataProtector();  

    public SecKey(byte[] privateKeyBytes)
    {
        if (privateKeyBytes is null)
            throw new ArgumentNullException("Key array cannot be null");
        if (privateKeyBytes.Length != 32)
            throw new ArgumentOutOfRangeException("Wrong key array length");
        if (RingSig.sc_check(privateKeyBytes) != 0)
            throw new ArgumentException("Invalid private key");
        _encryptedKeyBytes = Protect(privateKeyBytes);
    }

    // create empty key object for incoming contacts
    private SecKey()
    {
        
    }
    
    public static SecKey CreateEmpty()
    {
        return new SecKey();
    }

    public static SecKey GenerateRandom()
    {
        byte[] sec = new byte[32];
        RingSig.random_scalar(sec);
        RingSig.sc_reduce32(sec);
        try
        {
            return new SecKey(sec);
        }
        finally
        {
            CryptoBytes.Wipe(sec);
        }
    }

    public static SecKey FromSeed(byte[] seed)
    {
        byte[] pkey_padded = new byte[64];
        Buffer.BlockCopy(seed, 0, pkey_padded, 0, seed.Length);
        //ScalarOperations.sc_reduce(pkey_padded);
        byte[] sec = pkey_padded.ToArray();
        try
        {
            ScalarOperations.sc_reduce(sec);
            sec = sec.Take(32).ToArray();
            return new SecKey(sec);
        }
        finally
        {
            CryptoBytes.Wipe(sec);
        }
    }

    //public static SecKey FromBase64(string secKeyBase64)
    public static SecKey FromBase58(string secKeyBase58)
    {
        byte[] sec = Codecs.FromBase58ToBytes(secKeyBase58);
        try
        {
            return new SecKey(sec);
        }
        finally
        {
            CryptoBytes.Wipe(sec);
        }
    }
    
    //public string ToBase64()
    public string ToBase58()
    {
        if (_encryptedKeyBytes is null)
            return string.Empty;
        
        byte[] sec = ToBytes();
        try
        {
            return Codecs.FromBytesToBase58(sec);
        }
        finally
        {
            CryptoBytes.Wipe(sec);
        }
    }
    
    public override string ToString()
    {
        return ToBase58();
    }
    
    public byte[]? ToBytes()
    {
        if (_encryptedKeyBytes is null)
            return null;
        return Unprotect(_encryptedKeyBytes);
    }

    private static IDataProtector GetDataProtector()
    {
        return new CustomDataProtector("MystSafe.Shared.Crypto.Key");
    }

    private static byte[] Protect(byte[] data)
    {
        return _protector.Protect(data);
    }

    private static byte[] Unprotect(byte[] encryptedData)
    {
        return _protector.Unprotect(encryptedData);
    }
    
    public void Dispose()
    {
        (_protector as IDisposable)?.Dispose();
        if (_encryptedKeyBytes is not null)
            CryptoBytes.Wipe(_encryptedKeyBytes);
    }
}

