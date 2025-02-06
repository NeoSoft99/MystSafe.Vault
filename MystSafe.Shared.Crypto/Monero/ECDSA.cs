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
using MoneroSharp.Utils;

namespace MystSafe.Shared.Crypto;

    // Edwards-Curve Digital Signature Algorithm (EdDSA)
    // Test vectors: https://datatracker.ietf.org/doc/html/rfc8032#page-24
	public static class ECDSA // it's actually replaced with EdDSA - to do: rename
	{
        public static string Sign(string hash_base58, string public_key_base58, SecKey privateKey)
        {
            byte[] hash = Codecs.FromBase58ToBytes(hash_base58);
            byte[] public_key = Codecs.FromBase58ToBytes(public_key_base58);
            
            byte[] signature = Sign(hash, public_key, privateKey.ToBytes());
      
            //string signature_base64 = Codecs.FromBytesToBase64(signature);
            string signature_base58 = Codecs.FromBytesToBase58(signature);
            return signature_base58;
        }
        
        // returns base64-encoded digital signature
        public static string Sign(string hash_base58, string public_key_base58, string private_key_base58)
        {

            byte[] hash = Codecs.FromBase58ToBytes(hash_base58);
            byte[] public_key = Codecs.FromBase58ToBytes(public_key_base58);
            //byte[] private_key = Codecs.FromBase64ToBytes(private_key_base64);
            byte[] private_key = Codecs.FromBase58ToBytes(private_key_base58);
            try
            {
                byte[] signature = Sign(hash, public_key, private_key);
                //return Codecs.FromBytesToBase64(signature);
                return Codecs.FromBytesToBase58(signature);
            }
            finally
            {

                CryptoBytes.Wipe(private_key);
            }
        }

        public static byte[] Sign(byte[] hash, byte[] public_key, byte[] private_key)
        {

                if (Debugger.IsAttached)
                {
                    //Console.WriteLine("hash: " + MoneroUtils.BytesToHex(hash));
                    //Console.WriteLine("public_key: " + MoneroUtils.BytesToHex(public_key));
                    //Console.WriteLine("private_key: " + MoneroUtils.BytesToHex(private_key));
                }

                byte[] signature = RingSig.generate_signature(hash, public_key, private_key);
                if (Debugger.IsAttached)
                {
                    //Console.WriteLine("signature: " + MoneroUtils.BytesToHex(signature));
                }

                return signature;

        }

        public static bool VerifySignature(string hash_base58, string public_key_base58, string signature_base58)
        { 
            byte[] hash_bytes = Codecs.FromBase58ToBytes(hash_base58);
            byte[] public_key_bytes = Codecs.FromBase58ToBytes(public_key_base58);
            //byte[] signature_bytes = Codecs.FromBase64ToBytes(signature_base64);
            byte[] signature_bytes = Codecs.FromBase58ToBytes(signature_base58);
            return VerifySignature(hash_bytes, public_key_bytes, signature_bytes);
        }
        
        public static bool VerifySignature(byte[] hash, byte[] public_key, byte[] signature)
        {
            return RingSig.check_signature(hash, public_key, signature);
        }
    }


