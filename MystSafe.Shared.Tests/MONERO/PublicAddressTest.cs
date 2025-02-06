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

public class PublicAddressTests
{
    [Theory]
    [InlineData(
        "49krEa2UChucvK3H6zPEdiSP2hF554M2tdQ6Emea8qqNbgHDnvqjBppAgV7sMpoiijcNcXEfLbeVLfsET6JCCTDC5UjTKhA",
        "52575ee2154b2d39e0a78e397b7b58d37aa8083a53511fe85d72d0432480b327",
        "d6a0f4ca27ed78d6bf212c6084804597bc6a0923ee5cf5d99c2dc30826ce7dcf")]
    [InlineData(
        "49D8pyA2mhZLqS1znG3K171BZdbyBYcYu4bjb7kotSKa7R1hqpNNeg82YdFfXmdxM3DmBoxGBmMgcDphAnsVhHnj2Zd486w",
        "58c8de65789709093d0e8361301ae64c4c4b0e2136c2694ca8d0dbaea14f9c0d",
        "c83326c16f0d587695fa875acdbb2e0116bacb051a8e1e158426f2c84351f126")]
    [InlineData(
        "4AgrsLkekqZ289mtnS9iUaR7sEGB2LwEnbEnFTvrguJ1bpsRVsgzicJJyW4fXSVmS29MU81tMtkFi68wZamyzj8LLa9w6kV",
        "3502479c4c775b6b75e0d1f08b518b31f04c5345198ef91eb62996862eadb9ad",
        "ef33c13d3c4c6406b73bfd99c234b39031c2363c1ab33fccb1739d3d0781f2d0")]
    // Add more test vectors as needed
    public void CreateFromPublicKeys_ValidKeys_ReturnsCorrectAddress(
        string expectedAddress, string scanPubKeyHex, string readPubKeyHex)
    {
        // Arrange
        PubKey scanPubKey = new PubKey(Codecs.FromHexToBytes(scanPubKeyHex));
        PubKey readPubKey = new PubKey(Codecs.FromHexToBytes(readPubKeyHex));

        // Act
        var publicAddress = PublicAddress.CreateFromPublicKeys(scanPubKey, readPubKey, Networks.mainnet);

        // Assert
        Assert.Equal(expectedAddress, publicAddress.ToString());
    }


    [Theory]
    [InlineData(
        "49krEa2UChucvK3H6zPEdiSP2hF554M2tdQ6Emea8qqNbgHDnvqjBppAgV7sMpoiijcNcXEfLbeVLfsET6JCCTDC5UjTKhA",
        "52575ee2154b2d39e0a78e397b7b58d37aa8083a53511fe85d72d0432480b327",
        "d6a0f4ca27ed78d6bf212c6084804597bc6a0923ee5cf5d99c2dc30826ce7dcf")]
    [InlineData(
        "49D8pyA2mhZLqS1znG3K171BZdbyBYcYu4bjb7kotSKa7R1hqpNNeg82YdFfXmdxM3DmBoxGBmMgcDphAnsVhHnj2Zd486w",
        "58c8de65789709093d0e8361301ae64c4c4b0e2136c2694ca8d0dbaea14f9c0d",
        "c83326c16f0d587695fa875acdbb2e0116bacb051a8e1e158426f2c84351f126")]
    [InlineData(
        "4AgrsLkekqZ289mtnS9iUaR7sEGB2LwEnbEnFTvrguJ1bpsRVsgzicJJyW4fXSVmS29MU81tMtkFi68wZamyzj8LLa9w6kV",
        "3502479c4c775b6b75e0d1f08b518b31f04c5345198ef91eb62996862eadb9ad",
        "ef33c13d3c4c6406b73bfd99c234b39031c2363c1ab33fccb1739d3d0781f2d0")]
    // Add more test vectors as needed
    public void RecreateFromAddressString_ValidAddressString_ReturnsCorrectKeys(
        string addressString, string expectedScanPubKeyHex, string expectedReadPubKeyHex)
    {
        // Act
        var publicAddress = PublicAddress.RecreateFromAddressString(addressString, Networks.mainnet);
        var scanPubKey = publicAddress.ScanPubKey;
        var readPubKey = publicAddress.ReadPubKey;

        // Assert
        Assert.Equal(expectedScanPubKeyHex, Codecs.FromBytesToHex(scanPubKey.ToBytes()));
        Assert.Equal(expectedReadPubKeyHex, Codecs.FromBytesToHex(readPubKey.ToBytes()));
    }

    [Fact]
    public void AddressShort_ValidAddressString_ReturnsShortenedAddress()
    {
        // Arrange
        string addressString =
            "49krEa2UChucvK3H6zPEdiSP2hF554M2tdQ6Emea8qqNbgHDnvqjBppAgV7sMpoiijcNcXEfLbeVLfsET6JCCTDC5UjTKhA";
        string expectedShortAddress = "49kr....TKhA";

        // Act
        var shortAddress = PublicAddress.AddressShort(addressString);

        // Assert
        Assert.Equal(expectedShortAddress, shortAddress);
    }

}
