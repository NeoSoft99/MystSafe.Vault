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


//using NBitcoin.DataEncoders;
using System.Text;
using MoneroSharp.NaCl;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;


namespace MystSafe.Shared.Crypto;

public static class AES
{
   // returns base58 encoded byte array
    public static string Encrypt(byte[] Key, string salt, string PlainText)
    { 
        byte[] iv = GetIVfromSalt(salt);
        byte[] plaintext_bytes = Encoding.UTF8.GetBytes(PlainText);
        var encrypted_bytes = Encrypt(plaintext_bytes, Key, iv);
        CryptoBytes.Wipe(plaintext_bytes);
        //return Encoders.Base64.EncodeData(encrypted_bytes);
        return Codecs.FromBytesToBase58(encrypted_bytes);
    }

    public static byte[] Encrypt(byte[] input, byte[] key, byte[] iv)
    {
        var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
        cipher.Init(true, new ParametersWithIV(new KeyParameter(key), iv));
        return cipher.DoFinal(input);
    }

    public static string Encrypt(string Key, string salt, string PlainText)
    {
        //var key_bytes = Codecs.FromBase64ToBytes(Key);
        var key_bytes = Codecs.FromBase58ToBytes(Key);
        byte[] iv = GetIVfromSalt(salt);
        byte[] plaintext_bytes = Encoding.UTF8.GetBytes(PlainText);
        var encrypted_bytes = Encrypt(plaintext_bytes, key_bytes, iv);
        CryptoBytes.Wipe(key_bytes);
        CryptoBytes.Wipe(plaintext_bytes);
        //return Encoders.Base64.EncodeData(encrypted_bytes);
        return Codecs.FromBytesToBase58(encrypted_bytes);
    }

    public static byte[] Encrypt(byte[] input, byte[] key, string salt)
    {
        byte[] iv = GetIVfromSalt(salt);
        return Encrypt(input, key, iv);
    }

  
    private static byte[] GetIVfromSalt(string salt)
    {
        //string hashed_salt = Hashing.SHA256Base58(salt); // to stretch salt bytes if necessary
        string hashed_salt = Hashing.KeccakBase58(salt); // to stretch salt bytes if necessary
        return Encoding.Unicode.GetBytes(hashed_salt.ToCharArray(), 0, 8);
    }

    public static string Decrypt(byte[] Key, string salt, string CipherText)
    {
        byte[] iv = GetIVfromSalt(salt);
        //var cipher_text_bytes = Encoders.Base64.DecodeData(CipherText);
        var cipher_text_bytes = Codecs.FromBase58ToBytes(CipherText);
        var decrypted_bytes = Decrypt(cipher_text_bytes, Key, iv);
        var result =  Encoding.UTF8.GetString(decrypted_bytes);
        CryptoBytes.Wipe(decrypted_bytes);
        return result;
    }

    public static byte[] Decrypt(byte[] input, byte[] key, byte[] iv)
    {
        var cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
        cipher.Init(false, new ParametersWithIV(new KeyParameter(key), iv));
        return cipher.DoFinal(input);
    }

    // base64-encoded key
    public static string Decrypt(string Key, string salt, string CipherText)
    {
        var key_bytes = Codecs.FromBase58ToBytes(Key);
        byte[] iv = GetIVfromSalt(salt);
        var cipher_text_bytes = Codecs.FromBase58ToBytes(CipherText);
        var decrypted_bytes = Decrypt(cipher_text_bytes, key_bytes, iv);
        var result =  Encoding.UTF8.GetString(decrypted_bytes);
        CryptoBytes.Wipe(decrypted_bytes);
        CryptoBytes.Wipe(key_bytes);
        return result;
    }

    public static byte[] Decrypt(byte[] input, byte[] key, string salt)
    {
        byte[] iv = GetIVfromSalt(salt);
        return Decrypt(input, key, iv);
    }

    // returns new AES 256 key base58 
    public static byte[] GenerateRandomAES256Key()
    {
        const int keySizeInBits = 256;
        SecureRandom random = new SecureRandom();
        byte[] keyBytes = new byte[keySizeInBits / 8];
        random.NextBytes(keyBytes);
        return keyBytes;
        //return Codecs.FromBytesToBase58(keyBytes);
    }

}



