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
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;

namespace MystSafe.Shared.Crypto;

public static class KDF
{
    //public KDF()
    //{
    //}


    public static string GetStealthKey(byte[] shared_secret, string salt)
    {
        var key_bytes = GenerateKey(shared_secret, salt, 10);
        //return Codecs.FromBytesToBase64(key_bytes);
        return Codecs.FromBytesToBase58(key_bytes);
    }

    public static byte[] GetPasswordKey(byte[] shared_secret, string salt)
    {
        return GenerateKey(shared_secret, salt, 10000);
    }

    private static byte[] GenerateKey(byte[] shared_secret, string salt, int iterations)
    {
        //string hashed_salt = Hashing.SHA256Base58(salt); // to stretch salt bytes if necessary
        string hashed_salt = Hashing.KeccakBase58(salt); // to stretch salt bytes if necessary
        byte[] salt_bytes = Encoding.Unicode.GetBytes(hashed_salt.ToCharArray(), 0, 8);

        const int keyLength = 32;

        var generator = new Pkcs5S2ParametersGenerator();
        generator.Init(
            //PbeParametersGenerator.Pkcs5PasswordToBytes(shared_secret.ToCharArray()),
            shared_secret,
            salt_bytes,
            iterations);

        KeyParameter key = (KeyParameter)generator.GenerateDerivedMacParameters(keyLength * 8);
        var key_bytes = key.GetKey();
        return key_bytes;
        //return Codecs.FromBytesToBase64(key_bytes);
    }
}


