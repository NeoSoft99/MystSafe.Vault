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
using MystSafe.Shared.Crypto;

namespace MystSafe.Shared.Tests;

    public class DiffieHellmanTests
    {
        [Fact]
        public void KeyDerivationTest()
        {
            byte[] sec1 = new byte[32];
            byte[] pub1 = new byte[32];
            RingSig.generate_keys(pub1, sec1);
            byte[] sec2 = new byte[32];
            byte[] pub2 = new byte[32];
            RingSig.generate_keys(pub2, sec2);

            bool check = RingSig.check_key(pub1);
            Assert.True(check);
            
            check = RingSig.check_key(pub2);
            Assert.True(check);
            
            byte[] shared_secret_1 = new byte[32];
            byte[] shared_secret_2 = new byte[32];
            bool result1 = RingSig.generate_key_derivation(pub2, sec1, shared_secret_1);
            
            bool result2 = RingSig.generate_key_derivation(pub1, sec2, shared_secret_2);
            Assert.True(result1);
            Assert.True(result2);
            Assert.NotEmpty(shared_secret_1);
            Assert.NotEmpty(shared_secret_2);
            Assert.Equal(shared_secret_1, shared_secret_2);
        }
        
        [Fact]
        public void SharedSecretTestBytes()
        {
            byte[] sec1 = new byte[32];
            byte[] pub1 = new byte[32];
            RingSig.generate_keys(pub1, sec1);
            byte[] sec2 = new byte[32];
            byte[] pub2 = new byte[32];
            RingSig.generate_keys(pub2, sec2);
            
            byte[] shared_secret_1 = DiffieHellman.GetSharedSecret(sec1,  pub2);
            
            byte[] shared_secret_2 = DiffieHellman.GetSharedSecret(sec2,  pub1);
            
            Assert.NotEmpty(shared_secret_1);
            Assert.NotEmpty(shared_secret_2);
            Assert.Equal(shared_secret_1, shared_secret_2);
        }
        
        [Fact]
        public void SharedSecretTestBasic()
        {
            byte[] sec1 = new byte[32];
            byte[] pub1 = new byte[32];
            RingSig.generate_keys(pub1, sec1);
            byte[] sec2 = new byte[32];
            byte[] pub2 = new byte[32];
            RingSig.generate_keys(pub2, sec2);
            
            // if (Debugger.IsAttached)
            // {
            //     Console.WriteLine("keyPair1 private: " + BitConverter.ToString(keyPair1.PrivateKey.ToBytes()));
            //     Console.WriteLine("keyPair1 public: " + BitConverter.ToString(keyPair1.PublicKey.ToBytes()));
            //     Console.WriteLine("keyPair2 private: " + BitConverter.ToString(keyPair2.PrivateKey.ToBytes()));
            //     Console.WriteLine("keyPair2 public: " + BitConverter.ToString(keyPair2.PublicKey.ToBytes()));
            //
            // }

            byte[] shared_secret_1 = DiffieHellman.GetSharedSecret(Codecs.FromBytesToBase58(sec1),  Codecs.FromBytesToBase58(pub2));
            
            byte[] shared_secret_2 = DiffieHellman.GetSharedSecret(Codecs.FromBytesToBase58(sec2),  Codecs.FromBytesToBase58(pub1));
            
            Assert.NotEmpty(shared_secret_1);
            Assert.NotEmpty(shared_secret_2);
            Assert.Equal(shared_secret_1, shared_secret_2);
        }
        
        [Fact]
        public void SharedSecretTest()
        {
            var keyPair1 = KeyPair.GenerateRandom();
          
            var keyPair2 = KeyPair.GenerateRandom();
            
            if (Debugger.IsAttached)
            {
                Console.WriteLine("keyPair1 private: " + BitConverter.ToString(keyPair1.PrivateKey.ToBytes()));
                Console.WriteLine("keyPair1 public: " + BitConverter.ToString(keyPair1.PublicKey.ToBytes()));
                Console.WriteLine("keyPair2 private: " + BitConverter.ToString(keyPair2.PrivateKey.ToBytes()));
                Console.WriteLine("keyPair2 public: " + BitConverter.ToString(keyPair2.PublicKey.ToBytes()));

            }

            byte[] shared_secret_1 = DiffieHellman.GetSharedSecret(keyPair1.PrivateKey.ToBase58(), keyPair2.PublicKey.ToBase58());
            
            byte[] shared_secret_2 = DiffieHellman.GetSharedSecret(keyPair2.PrivateKey.ToBase58(), keyPair1.PublicKey.ToBase58());
            
            Assert.NotEmpty(shared_secret_1);
            Assert.NotEmpty(shared_secret_2);
            Assert.Equal(shared_secret_1, shared_secret_2);
        }
        
        [Fact]
        public void SharedSecretWithAddressTest()
        {
            var address1 = UserAddress.GenerateFromMnemonic(Networks.devnet);
            var address2 = UserAddress.GenerateFromMnemonic(Networks.devnet);
            
            /*if (Debugger.IsAttached)
            {
                Console.WriteLine("keyPair1 private: " + BitConverter.ToString(keyPair1.PrivateKey.ToBytes()));
                Console.WriteLine("keyPair1 public: " + BitConverter.ToString(keyPair1.PublicKey.ToBytes()));
                Console.WriteLine("keyPair2 private: " + BitConverter.ToString(keyPair2.PrivateKey.ToBytes()));
                Console.WriteLine("keyPair2 public: " + BitConverter.ToString(keyPair2.PublicKey.ToBytes()));

            }*/

            byte[] shared_secret_1 = DiffieHellman.GetSharedSecret(address1.ScanKey.ToBase58(), address2.ScanPubKey.ToBase58());
            
            byte[] shared_secret_2 = DiffieHellman.GetSharedSecret(address2.ScanKey.ToBase58(), address1.ScanPubKey.ToBase58());
            
            Assert.NotEmpty(shared_secret_1);
            Assert.NotEmpty(shared_secret_2);
            Assert.Equal(shared_secret_1, shared_secret_2);
        }
        
        [Fact]
        public void EncryptDecryptTest()
        {
            var keyPair1 = KeyPair.GenerateRandom();
            var keyPair2 = KeyPair.GenerateRandom();
           
            string salt = "the_salt";
            string plainText = "Hello, World!";

            string encryptedText = DiffieHellman.Encrypt(keyPair1.PrivateKey.ToBase58(), keyPair2.PublicKey.ToBase58(), salt, plainText);
            string decryptedText = DiffieHellman.Decrypt(keyPair2.PrivateKey.ToBase58(), keyPair1.PublicKey.ToBase58(), salt, encryptedText);

            Assert.Equal(plainText, decryptedText);
        }
        
        private static Random _random = new Random();

        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] stringChars = new char[length];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[_random.Next(chars.Length)];
            }

            return new string(stringChars);
        }

        [Fact]
        public void EncryptDecryptWithRandomSaltAndPlainTextTest()
        {
            var keyPair1 = KeyPair.GenerateRandom();
            var keyPair2 = KeyPair.GenerateRandom();
        
            int saltLength = _random.Next(1, 256); // Random length between 8 and 16
            int plainTextLength = _random.Next(1, 256); // Random length between 8 and 64

            string salt = GenerateRandomString(saltLength);
            string plainText = GenerateRandomString(plainTextLength);
            
            string encryptedText = DiffieHellman.Encrypt(keyPair1.PrivateKey.ToBase58(), keyPair2.PublicKey.ToBase58(), salt, plainText);
            string decryptedText = DiffieHellman.Decrypt(keyPair2.PrivateKey.ToBase58(), keyPair1.PublicKey.ToBase58(), salt, encryptedText);
            
            if (Debugger.IsAttached)
            {
                Console.WriteLine("salt: " + salt);
                Console.WriteLine("plainText: " + plainText);
                Console.WriteLine("encryptedText: " + encryptedText);
            }

            Assert.Equal(plainText, decryptedText);
        }
    }


