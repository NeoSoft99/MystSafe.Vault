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

using MoneroRing.Crypto;
using MoneroSharp.NaCl;

namespace MystSafe.Shared.Crypto;

public class PubKey
{
    protected byte[] _keyBytes;
    
    public PubKey(byte[] pubKeyBytes)//: base(pubKeyBytes)
    {
        if (pubKeyBytes is null)
            throw new ArgumentNullException("Key array cannot be null");
        if (pubKeyBytes.Length != 32)
            throw new ArgumentOutOfRangeException("Wrong key array length");
        if (!RingSig.check_key(pubKeyBytes))
            throw new ArgumentException("Invalid public key");
        _keyBytes = new byte[32];
        Array.Copy(pubKeyBytes, 0, _keyBytes, 0, 32);
    }

    public static PubKey FromPrivateKeyBytes(byte[] privateKey)
    {
        byte[] pub = new byte[32];
        if (!RingSig.secret_key_to_public_key(privateKey, pub))
            throw new ArgumentException("Invalid private key");
        return new PubKey(pub);
    }
    
    public static PubKey FromPrivateKey(SecKey privateKey)
    {
        byte[] sec = privateKey.ToBytes();
        try
        {
            return FromPrivateKeyBytes(sec);
        }
        finally
        {
            CryptoBytes.Wipe(sec);
        }
    }
    
    public static PubKey FromBase58String(string publicKeyBase58String)
    {
        byte[] bytes = Codecs.FromBase58ToBytes(publicKeyBase58String);
        return new PubKey(bytes);
    }
    
    public byte[] ToBytes()
    {
        var keyBytes = new byte[32];
        Array.Copy(_keyBytes, 0, keyBytes, 0, 32);
        return keyBytes;
    }
    
    public string ToBase58()
    {
        return Codecs.FromBytesToBase58(_keyBytes);
    }
    
    public override string ToString()
    {
        return ToBase58();
    }
}