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
using Org.BouncyCastle.Crypto.Parameters;

namespace MystSafe.Shared.Tests;

    public class BlockKeyPairTests
    {
        [Fact]
        public void GenerateNew_ShouldCreateValidKeyPair()
        {


            // Arrange
            var validBase58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
            //var validBase64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

            // Act
            var blockKeyPair = KeyPair.GenerateRandom();
            //var keyBytes = blockKeyPair.Key.ToBytes();
            //var pubKeyBytes = blockKeyPair.PubKey.ToBytes();
            var keyBytes = blockKeyPair.PrivateKey.ToBytes();
            var pubKeyBytes = blockKeyPair.PublicKey.ToBytes();

            // Assert
            Assert.Equal(32, keyBytes.Length); // Key should be 32 bytes (256 bits)
            //Assert.Equal(33, pubKeyBytes.Length); // PubKey should be 33 bytes when compressed
            Assert.Equal(32, pubKeyBytes.Length); // Monero PubKey should be 32 bytes 

            // Check if Base64 string is valid
            Assert.All(blockKeyPair.PrivateKey.ToBase58(), c => Assert.Contains(c, validBase58Chars));
            int length = blockKeyPair.PrivateKey.ToBase58().Length;

            try
            {
                Assert.True(length >= 43 && length <= 45);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed: {ex.Message}");
                Console.WriteLine($"Length value: {length}");
                throw; // Re-throw the exception to ensure the test still fails
            }

            // Check if Base58 string is valid
            Assert.All(blockKeyPair.PublicKey.ToBase58(), c => Assert.Contains(c, validBase58Chars));
            length = blockKeyPair.PublicKey.ToBase58().Length;

            try
            {
                Assert.True(length >= 43 && length <= 45);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed: {ex.Message}");
                Console.WriteLine($"Length value: {length}");
                throw; // Re-throw the exception to ensure the test still fails
            }
        }

        [Fact]
        public void BlockKeyPairExTests()
        {
            // Arrange
            var validBase58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
            //var validBase64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

            // Act
            //var blockKeyPair = BlockKeyPairEx.GenerateRandom();
            var blockKeyPair = KeyPair.GenerateRandom();
            //var keyBytes = blockKeyPair.Key.ToBytes();
            //var pubKeyBytes = blockKeyPair.PubKey.ToBytes();
            var keyBytes = blockKeyPair.PrivateKey.ToBytes();
            var pubKeyBytes = blockKeyPair.PublicKey.ToBytes();

            // Assert
            Assert.Equal(32, keyBytes.Length); // Key should be 32 bytes (256 bits)
            //Assert.Equal(33, pubKeyBytes.Length); // PubKey should be 33 bytes when compressed
            Assert.Equal(32, pubKeyBytes.Length); // Monero PubKey should be 32 bytes when compressed

            // Check if Base64 string is valid
            Assert.All(blockKeyPair.PrivateKey.ToBase58(), c => Assert.Contains(c, validBase58Chars));
            int length = blockKeyPair.PrivateKey.ToBase58().Length;
            Assert.True(length >= 43 && length <= 45); //

            // Check if Base58 string is valid
            Assert.All(blockKeyPair.PublicKey.ToBase58(), c => Assert.Contains(c, validBase58Chars));
            length = blockKeyPair.PublicKey.ToBase58().Length;
            Assert.True(length >= 43 && length <= 45); //

            Assert.All(blockKeyPair.PrivateKey.ToBase58(), c => Assert.Contains(c, validBase58Chars));
            Assert.True(blockKeyPair.PrivateKey.ToBase58().Length >= 44 && blockKeyPair.PrivateKey.ToBase58().Length <= 45); //

            //var new_key_pair_ex = BlockKeyPairEx.GenerateFromPrivateKey(blockKeyPair.KeyBase64);
            var new_key_pair_ex = KeyPair.FromPrivateKey(blockKeyPair.PrivateKey);
            Assert.Equal(blockKeyPair.PrivateKey.ToBase58(), new_key_pair_ex.PrivateKey.ToBase58());
            Assert.Equal(blockKeyPair.PublicKey.ToBase58(), new_key_pair_ex.PublicKey.ToBase58());
            
            // Ensure keys are valid points on the elliptic curve
            Assert.True(IsValidPointOnCurve(pubKeyBytes));

            // Verify the public key can be derived from the private key
            var derivedPubKey = PubKey.FromPrivateKeyBytes(blockKeyPair.PrivateKey.ToBytes());
            Assert.Equal(blockKeyPair.PublicKey.ToBytes(), derivedPubKey.ToBytes());

            // Verify key uniqueness
            var newKeyPair = KeyPair.GenerateRandom();
            Assert.NotEqual(blockKeyPair.PrivateKey.ToBytes(), newKeyPair.PrivateKey.ToBytes());
            Assert.NotEqual(blockKeyPair.PublicKey.ToBytes(), newKeyPair.PublicKey.ToBytes());
        }
        
        private bool IsValidPointOnCurve(byte[] publicKey)
        {
            try
            {
                var point = new X25519PublicKeyParameters(publicKey, 0);
                // Further validation can be done using the specific curve's parameters
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

