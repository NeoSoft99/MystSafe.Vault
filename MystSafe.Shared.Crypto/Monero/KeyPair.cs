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

using MoneroSharp.NaCl;

namespace MystSafe.Shared.Crypto;

public class KeyPair
{
    public SecKey PrivateKey { get; private set; }
    public PubKey PublicKey { get; private set; }

    public KeyPair(byte[] privateKey, byte[] publicKey)
    {
        PrivateKey = new SecKey(privateKey);
        PublicKey = new PubKey(publicKey);
    }
    
    public KeyPair(SecKey privateKey, PubKey publicKey)
    {
        PrivateKey = privateKey;
        PublicKey = publicKey;
    }
    
    /*
    public static BlockKeyPair RestoreFromExistingKey(string private_key_base_64)
    {
        var sec = Codecs.FromBase64ToBytes(private_key_base_64);
        try
        {
            return GenerateFromData(sec);
        }
        finally
        {
            CryptoBytes.Wipe(sec);
        }
    }
    */
        
   
    // public static KeyPair GenerateRandom()
    // {
    //     SecKey sec = SecKey.GenerateRandom();
    //     return FromPrivateKeyBytes(sec.ToBytes());
    // }
    //
    public static KeyPair GenerateRandom()
    {
        SecKey sec_key = SecKey.GenerateRandom();
        PubKey pub_key = PubKey.FromPrivateKey(sec_key);
        return new KeyPair(sec_key, pub_key);
    }

    // public static BlockKeyPair GenerateFromData(byte[] key_data)
    // {
    //     var sec_key = new SecKey(key_data);
    //     var pub_key = PubKey.FromPrivateKeyBytes(sec_key.ToBytes());
    //     return new BlockKeyPair(sec_key, pub_key);
    // }
    //
    public static KeyPair FromPrivateKey(SecKey private_key)
    {
        PubKey pub = PubKey.FromPrivateKey(private_key);
        return new KeyPair(private_key, pub);
    }
    
    public static KeyPair FromPrivateKeyBytes(byte[] privateKeyBytes)
    {
        SecKey sec_key = new SecKey(privateKeyBytes);
        return FromPrivateKey(sec_key);
    }
    
    public static KeyPair FromPrivateKeyBase58(string sec_key_base58)
    {
        var sec = Codecs.FromBase58ToBytes(sec_key_base58);
        try
        {
            return FromPrivateKeyBytes(sec);
        }
        finally
        {
            CryptoBytes.Wipe(sec);
        }
    }
    
    /*public static KeyPair FromPrivateKeyBase64(string sec_key_base64)
    {
        var sec = Codecs.FromBase64ToBytes(sec_key_base64);
        try
        {
            return FromPrivateKeyBytes(sec);
        }
        finally
        {
            CryptoBytes.Wipe(sec);
        }
    }*/
    
}
