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

using System.Globalization;
using MoneroSharp.Utils;
using MystSafe.Shared.Crypto;

namespace MystSafe.Tests.Crypto;

    public class StealthAddressTests
    {
        // Sample test data (replace with real or mocked data in production)
        private string RemoteScanPubKeyBase58 = Codecs.FromBytesToBase58(MoneroUtils.HexBytesToBinary(
            "80d6c7afc6565d00d80de8349e710ded1f0dfec60e8acf88f0c5699f3993e57d")); 
        private string ChatPubKey = Codecs.FromBytesToBase58(MoneroUtils.HexBytesToBinary(
            "80d6c7afc6565d00d80de8349e710ded1f0dfec60e8acf88f0c5699f3993e57d")); 
        private string LocalPrivateKeyBase58= Codecs.FromBytesToBase58(MoneroUtils.HexBytesToBinary(
            "7178d4fa3c6aa96c335c30133c0e95fefa1f1fac2905898c018b323888856a0d"));
        private string ScanKey = Codecs.FromBytesToBase58(MoneroUtils.HexBytesToBinary(
            "7178d4fa3c6aa96c335c30133c0e95fefa1f1fac2905898c018b323888856a0d"));

        //private string ScanKey = LocalPrivateKeyBase58;
        //private string ChatPubKey = RemoteScanPubKeyBase58;
        private const string Salt = "SampleSaltValue1234";

        [Fact]
        public void GenerateNew_ValidInputs_ReturnsNonNullStealthAddress()
        {
            // Arrange
            var localPrivateKey = SecKey.FromBase58(LocalPrivateKeyBase58);

            // Act
            var stealthAddress = StealthAddress.GenerateNew(
                RemoteScanPubKeyBase58,
                localPrivateKey,
                Salt
            );

            // Assert
            Assert.NotNull(stealthAddress);
            Assert.False(
                string.IsNullOrEmpty(stealthAddress.ToString()),
                "StealthAddress.ToString() should not be null or empty"
            );
        }

        [Fact]
        public void Restore_ValidInputs_ReturnsNonNullStealthAddress()
        {
            // Act
            var stealthAddress = StealthAddress.Restore(ScanKey, ChatPubKey, Salt);

            // Assert
            Assert.NotNull(stealthAddress);
            Assert.False(
                string.IsNullOrEmpty(stealthAddress.ToString()),
                "StealthAddress.ToString() should not be null or empty"
            );
        }

        [Fact]
        public void IsMatch_WhenAddressesAreEqual_ReturnsTrue()
        {
            // Arrange
            var stealthAddress = StealthAddress.Restore(ScanKey, ChatPubKey, Salt);
            var messageStealthAddress = stealthAddress.ToString(); // same as stealthAddress

            // Act
            var result = stealthAddress.IsMatch(messageStealthAddress);

            // Assert
            Assert.True(result, "IsMatch should return true for identical addresses");
        }

        [Fact]
        public void IsMatch_WhenAddressesAreNotEqual_ReturnsFalse()
        {
            // Arrange
            var stealthAddress = StealthAddress.Restore(ScanKey, ChatPubKey, Salt);
            var differentStealthAddress = "CompletelyDifferentStealthAddress";

            // Act
            var result = stealthAddress.IsMatch(differentStealthAddress);

            // Assert
            Assert.False(result, "IsMatch should return false for different addresses");
        }

        /*[Theory]
        [InlineData("TestScanKey1", "TestChatPubKey1", "TestSalt1")]
        [InlineData("TestScanKey2", "TestChatPubKey2", "TestSalt2")]
        public void Restore_WithMultipleTestData_ReturnsValidAddress(string testScanKey, string testChatPubKey, string testSalt)
        {
            // Act
            var stealthAddress = StealthAddress.Restore(testScanKey, testChatPubKey, testSalt);

            // Assert
            Assert.NotNull(stealthAddress);
            Assert.False(
                string.IsNullOrEmpty(stealthAddress.ToString()),
                "StealthAddress.ToString() should not be null or empty"
            );
        }*/
    }
