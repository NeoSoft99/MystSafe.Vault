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

using System.Diagnostics;
using MoneroRing.Crypto;
using MoneroSharp.NaCl;

namespace MystSafe.Shared.Crypto;

    public static class DiffieHellman // 
    {

        //public static string Encrypt(string localPrivateKeyBase64, string remotePublicKeyBase58, string salt, string plainText)
        public static string Encrypt(string localPrivateKeyBase58, string remotePublicKeyBase58, string salt, string plainText)
        {

            var secret = GetSharedSecret(localPrivateKeyBase58, remotePublicKeyBase58);
            return AES.Encrypt(secret, salt, plainText);
        }
        
        public static string Encrypt(SecKey localPrivateKey, string remotePublicKeyBase58, string salt, string plainText)
        {
            var secret = GetSharedSecret(localPrivateKey.ToBytes(), Codecs.FromBase58ToBytes(remotePublicKeyBase58));
            return AES.Encrypt(secret, salt, plainText);
        }

        //public static string Decrypt(string localPrivateKeyBase64, string remotePublicKeyBase58, string salt, string cipherText)
        public static string Decrypt(string localPrivateKeyBase58, string remotePublicKeyBase58, string salt, string cipherText)
        {
            var secret = GetSharedSecret(localPrivateKeyBase58, remotePublicKeyBase58);
            return AES.Decrypt(secret, salt, cipherText);
        }
        
        public static string Decrypt(SecKey localPrivateKey, string remotePublicKeyBase58, string salt, string cipherText)
        {
            var secret = GetSharedSecret(localPrivateKey.ToBytes(), Codecs.FromBase58ToBytes(remotePublicKeyBase58));
            return AES.Decrypt(secret, salt, cipherText);
        }

        // private static byte[] GetIV(string salt)
        // {
        //     var salt_bytes = Codecs.FromASCIIToBytes(salt);
        //     byte[] hashed_salt_bytes = Hashing.KeccakBytes(salt_bytes); //Hashing.SHA256Base58(Salt); // to stretch salt bytes if necessary
        //     byte[] iv = hashed_salt_bytes.Take(8).ToArray(); //Encoding.Unicode.GetBytes(hashed_salt.ToCharArray(), 0, 8);
        //     return iv;
        // }
        
         public static byte[] GetSharedSecret(byte[] privateKey, byte[] publicKey)
        {
            try
            {
                // if (Debugger.IsAttached)
                // {
                //     Console.WriteLine("GetSharedSecret - privateKey: " + BitConverter.ToString(privateKey));
                //     Console.WriteLine("GetSharedSecret - publicKey: " + BitConverter.ToString(publicKey));
                // }
                
                // Validate key lengths for Curve25519
                if (privateKey.Length != 32)
                    throw new ArgumentException("Invalid length for local private key.");
                if (privateKey.Length != 32)
                    throw new ArgumentException("Invalid length for remote public key.");

                
                // Convert rA to bytes
                byte[] rABytes = new byte[32];
                if (!RingSig.generate_key_derivation(publicKey, privateKey, rABytes))
                {
                    throw new Exception("generate_key_derivation failed");
                }

                // if (Debugger.IsAttached)
                // {
                //     Console.WriteLine("rABytes: " + BitConverter.ToString(rABytes));
                // }

                // Hash the rABytes to get the shared secret
                //byte[] sharedSecret = RingSig.hash_to_scalar(rABytes);
                byte[] sharedSecret = Hashing.KeccakBytes(rABytes);

                // if (Debugger.IsAttached)
                // {
                //     Console.WriteLine("GetSharedSecret - Shared Secret (hashed): " + BitConverter.ToString(sharedSecret));
                // }
                //CryptoBytes.Wipe(localPrivateKeyBytes);
                return sharedSecret;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }
    

        //public static byte[] GetSharedSecret(string localPrivateKeyBase64, string remotePublicKeyBase58)
        public static byte[] GetSharedSecret(string localPrivateKeyBase58, string remotePublicKeyBase58)
        {
            try
            {
                // Convert keys from base58 to byte arrays
                byte[] remotePublicKeyBytes = Codecs.FromBase58ToBytes(remotePublicKeyBase58);
                byte[] localPrivateKeyBytes = Codecs.FromBase58ToBytes(localPrivateKeyBase58);
                try
                {
                    return GetSharedSecret(localPrivateKeyBytes, remotePublicKeyBytes);
                }
                finally
                {
                    CryptoBytes.Wipe(localPrivateKeyBytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }
    }




