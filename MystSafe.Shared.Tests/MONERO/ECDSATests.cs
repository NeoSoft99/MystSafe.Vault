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

using MystSafe.Shared.Crypto;

namespace MystSafe.Shared.Tests;

    public class ECDSATests
    {
        private const string Mnemonic12TestString =
            "casino pear chat salon business farm put museum fee violin shield whale";
        
        [Fact]
        public void SignAndVerifySignatureBinaryTest()
        {
            // Generate a new private key and get the corresponding public key
            // SecKey privateKey = SecKey.GenerateRandom();
            // PubKey publicKey = PubKey.FromPrivateKey(privateKey);
            //UserAddress userAddress = UserAddress.GenerateFromMnemonic(Networks.mainnet);
            UserAddress userAddress = UserAddress.RestoreFromMnemonic(Mnemonic12TestString, Networks.mainnet);

            // Convert the private and public keys to base64 and base58 respectively
            // string privateKeyBase64 = Codecs.FromBytesToBase64(privateKey.ToBytes());
            // string publicKeyBase58 = Codecs.FromBytesToBase58(publicKey.ToBytes());

            // Create a hash for testing
            
            //var keccak256 = new Nethereum.Util.Sha3Keccack();
            //byte[] hash = keccak256.CalculateHash(Codecs.FromUTF8ToBytes("Hello, World!"));
            string hash_str = Hashing.KeccakBase58("Hello, World!"); //Hashing.SHA256Base58("Hello, World!");
            byte[] hash = Codecs.FromBase58ToBytes(hash_str);
            Console.WriteLine("hash1: " + hash_str);
            //Console.WriteLine("ScanPubKey: " + userAddress.ScanPubKey.ToBase58());
            //Console.WriteLine("ScanKey: " + userAddress.ScanKey.ToBase64());
            // Sign the hash using the private key
            byte[] signature = ECDSA.Sign(hash, userAddress.ScanPubKey.ToBytes(), userAddress.ScanKey.ToBytes());
            //Console.WriteLine("signature1: " + Codecs.FromBytesToBase58(signature));
            // Verify the signature using the public key
            bool isVerified = ECDSA.VerifySignature(hash, userAddress.ScanPubKey.ToBytes(), signature);

            Assert.True(isVerified);
        }
        
        [Fact]
        public void SignAndVerifySignatureTest()
        {
            // Generate a new private key and get the corresponding public key
            // SecKey privateKey = SecKey.GenerateRandom();
            // PubKey publicKey = PubKey.FromPrivateKey(privateKey);
            //UserAddress userAddress = UserAddress.GenerateFromMnemonic(Networks.mainnet);
            UserAddress userAddress = UserAddress.RestoreFromMnemonic(Mnemonic12TestString, Networks.mainnet);

            // Convert the private and public keys to base64 and base58 respectively
            // string privateKeyBase64 = Codecs.FromBytesToBase64(privateKey.ToBytes());
            // string publicKeyBase58 = Codecs.FromBytesToBase58(publicKey.ToBytes());

            // Create a hash for testing
            //string hash = Hashing.SHA256Base58("Hello, World!");
            string hash = Hashing.KeccakBase58("Hello, World!");
            Console.WriteLine("hash2: " + hash);
            //Console.WriteLine("ScanPubKey: " + userAddress.ScanPubKey.ToBase58());
            //Console.WriteLine("ScanKey: " + userAddress.ScanKey.ToBase64());
            // Sign the hash using the private key
            string signature = ECDSA.Sign(hash, userAddress.ScanPubKey.ToBase58(), userAddress.ScanKey.ToBase58());
            //Console.WriteLine("signature2: " + signature);
            // Verify the signature using the public key
            bool isVerified = ECDSA.VerifySignature(hash, userAddress.ScanPubKey.ToBase58(), signature);

            Assert.True(isVerified);
        }
        
        [Fact]
        public void SignAndVerifySignatureBinaryKeysTest()
        {
            // Generate a new private key and get the corresponding public key
            SecKey privateKey = SecKey.GenerateRandom();
            PubKey publicKey = PubKey.FromPrivateKey(privateKey);

            // Convert the private and public keys to base64 and base58 respectively
            // string privateKeyBase64 = Codecs.FromBytesToBase64(privateKey.ToBytes());
            // string publicKeyBase58 = Codecs.FromBytesToBase58(publicKey.ToBytes());

            // Create a hash for testing
            
            //var keccak256 = new Nethereum.Util.Sha3Keccack();
            //byte[] hash = keccak256.CalculateHash(Codecs.FromUTF8ToBytes("Hello, World!"));
            //string hash_str = Hashing.SHA256Base58("Hello, World!");
            string hash_str = Hashing.KeccakBase58("Hello, World!");
            byte[] hash = Codecs.FromBase58ToBytes(hash_str);
            Console.WriteLine("hash1: " + hash_str);
            //Console.WriteLine("ScanPubKey: " + userAddress.ScanPubKey.ToBase58());
            //Console.WriteLine("ScanKey: " + userAddress.ScanKey.ToBase64());
            // Sign the hash using the private key
            byte[] signature = ECDSA.Sign(hash, publicKey.ToBytes(), privateKey.ToBytes());
            //Console.WriteLine("signature1: " + Codecs.FromBytesToBase58(signature));
            // Verify the signature using the public key
            bool isVerified = ECDSA.VerifySignature(hash, publicKey.ToBytes(), signature);

            Assert.True(isVerified);
        }
        
        [Fact]
        public void SignAndVerifySignatureKeysTest()
        {
            // Generate a new private key and get the corresponding public key
            SecKey privateKey = SecKey.GenerateRandom();
            PubKey publicKey = PubKey.FromPrivateKey(privateKey);

            // Convert the private and public keys to base64 and base58 respectively
            //string privateKeyBase64 = privateKey.ToString();
            //string publicKeyBase58 = publicKey.ToString();

            // Create a hash for testing
            
            //var keccak256 = new Nethereum.Util.Sha3Keccack();
            //byte[] hash = keccak256.CalculateHash(Codecs.FromUTF8ToBytes("Hello, World!"));
            //string hash_str = Codecs.FromBytesToBase58(hash);
            string hash_str = Hashing.KeccakBase58("Hello, World!");

            // Sign the hash using the private key
            string signature = ECDSA.Sign(hash_str, publicKey.ToString(), privateKey.ToString());
            //Console.WriteLine("signature1: " + Codecs.FromBytesToBase58(signature));
            // Verify the signature using the public key
            bool isVerified = ECDSA.VerifySignature(hash_str, publicKey.ToString(), signature);

            Assert.True(isVerified);
        }
    }

