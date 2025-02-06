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

using MoneroSharp.Utils;
using MystSafe.Shared.Crypto;

namespace MystSafe.Shared.Tests;

public class UserAddressTests
{
    private const string Mnemonic12TestString =
        "casino pear chat salon business farm put museum fee violin shield whale";

    private const string Mnemonic25TestString =
        "hookup aunt aphid bimonthly pawnshop ashtray ugly physics rogue strained frying fonts cement highway angled goldfish cake push ecstatic tether artistic moment pouch rodent ecstatic";

    private readonly byte[] Seed64Test = MoneroUtils.HexBytesToBinary(
        "2ad370a6975f06035e7132ab3feb4cd4cd685d9877ab0c0613b78a1eaf5176cd1ba0a603f32e9113dc4de8c27eefea53d6c3bc6d9e86d81982794c89fe891eb9");

    private readonly string Seed32Test = "7178d4fa3c6aa96c335c30133c0e95fefa1f1fac2905898c018b323888856a0d";
    
    // same as Monero secret spend key
    private readonly byte[] PrivateReadKeyTest = MoneroUtils.HexBytesToBinary(
        "7178d4fa3c6aa96c335c30133c0e95fefa1f1fac2905898c018b323888856a0d");
    
    // same as monero secret view key
    private readonly byte[] PrivateScanKeyTest = MoneroUtils.HexBytesToBinary(
        "5e2f2a1ef7c49078873fe0ecc930cec5610ac1ce843785f615865b5044ab7e03");
    
    // same as monero public spend key
    private readonly byte[] PublicReadKeyTest = MoneroUtils.HexBytesToBinary(
        "b10153329537111a62745dafbf138b2c5bc91547a7ac151577788518da902d12");

    // same as monero public view key
    private readonly byte[] PublicScanKeyTest = MoneroUtils.HexBytesToBinary(
        "80d6c7afc6565d00d80de8349e710ded1f0dfec60e8acf88f0c5699f3993e57d");
    
    private const string AddressStringTest =
        "48L9xrFJ3NY5QxtQCWktHQ8RLLUTrqtNQ4bFiVR5WMvL46WHiH1a94g19BsifMnVSkgfNgAWMABNWPuVgXPtV4hSFDDwGqZ";

    [Fact]
    public void GenerateFromMnemonicTest()
    {
        // Generate a new UserAddress
        UserAddress userAddress = UserAddress.GenerateFromMnemonic(Networks.mainnet);

        // Check that the mnemonic phrase is not null
        Assert.NotNull(userAddress.Mnemonic12);
        Assert.NotNull(userAddress.Mnemonic12String);
        Assert.NotNull(userAddress.Mnemonic25String);
    }

    [Fact]
    public void RestoreFromMnemonicTest()
    {
        // Generate a new UserAddress
        UserAddress userAddress1 = UserAddress.GenerateFromMnemonic(Networks.mainnet);

        // Get the mnemonic phrase
        string mnemonicPhrase = userAddress1.Mnemonic12.ToString();

        // Restore the UserAddress from the mnemonic phrase
        UserAddress userAddress2 = UserAddress.RestoreFromMnemonic(mnemonicPhrase, Networks.mainnet);

        // Check that the restored UserAddress has the same mnemonic phrase
        Assert.Equal(userAddress1.Seed32Hex, userAddress2.Seed32Hex);
        Assert.Equal(userAddress1.Mnemonic12.ToString(), userAddress2.Mnemonic12.ToString());
        Assert.Equal(userAddress1.Mnemonic25String, userAddress2.Mnemonic25String);
        Assert.Equal(userAddress1.ToString(), userAddress2.ToString());
        
        Assert.Equal(userAddress1.ReadKey.ToBytes(), userAddress2.ReadKey.ToBytes());
        Assert.Equal(userAddress1.ReadKey.ToBase58(), userAddress2.ReadKey.ToBase58());
        Assert.Equal(userAddress1.ReadPubKey.ToBytes(), userAddress2.ReadPubKey.ToBytes());
        Assert.Equal(userAddress1.ReadPubKey.ToBase58(), userAddress2.ReadPubKey.ToBase58());
        
        Assert.Equal(userAddress1.ScanKey.ToBytes(), userAddress2.ScanKey.ToBytes());
        Assert.Equal(userAddress1.ScanKey.ToBase58(), userAddress2.ScanKey.ToBase58());
        Assert.Equal(userAddress1.ScanPubKey.ToBytes(), userAddress2.ScanPubKey.ToBytes());
        Assert.Equal(userAddress1.ScanPubKey.ToBase58(), userAddress2.ScanPubKey.ToBase58());
        
        Assert.Equal(userAddress1.HiddenScanKey.ToBytes(), userAddress2.HiddenScanKey.ToBytes());
        Assert.Equal(userAddress1.HiddenScanKey.ToBase58(), userAddress2.HiddenScanKey.ToBase58());
        Assert.Equal(userAddress1.HiddenScanPubKey.ToBytes(), userAddress2.HiddenScanPubKey.ToBytes());
        Assert.Equal(userAddress1.HiddenScanPubKey.ToBase58(), userAddress2.HiddenScanPubKey.ToBase58());
    }

    [Fact]
    public void GenerateFromMnemonic_ShouldGenerateValidAddress()
    {
        // Arrange
        int network = Networks.mainnet; // Replace with actual network identifier

        // Act
        var userAddress = UserAddress.GenerateFromMnemonic(network);

        // Assert
        Assert.NotNull(userAddress);
        Assert.False(string.IsNullOrEmpty(userAddress.Mnemonic12String));
        Assert.False(string.IsNullOrEmpty(userAddress.Mnemonic25String));
        Assert.NotNull(userAddress.HiddenScanPubKey);
        Assert.False(string.IsNullOrEmpty(userAddress.HiddenScanPubKey.ToBase58()));
        Assert.NotNull(userAddress.HiddenScanKey);
        Assert.False(string.IsNullOrEmpty(userAddress.HiddenScanKey.ToBase58()));
    }

    [Fact]
    public void RestoreFromMnemonic_ShouldRestoreValidMnemonic()
    {
        // Arrange
        int network = Networks.mainnet; 

        // Act
        var userAddress = UserAddress.RestoreFromMnemonic(Mnemonic12TestString, network);

        // Assert
        Assert.NotNull(userAddress);
        Assert.Equal(Mnemonic12TestString, userAddress.Mnemonic12String);
        Assert.Equal(Mnemonic25TestString, userAddress.Mnemonic25String);
    }

    [Fact]
    public void DeriveKeys_UsingTestVectors_ShouldDeriveCorrectKeys()
    {
        // Arrange
        int network = Networks.mainnet; // Replace with actual network identifier
        var userAddress = UserAddress.RestoreFromMnemonic(Mnemonic12TestString, network);

        Assert.Equal(Seed32Test, userAddress.Seed32Hex);
        
        Assert.Equal(PrivateReadKeyTest, userAddress.ReadKey.ToBytes());
        Assert.Equal(PrivateScanKeyTest, userAddress.ScanKey.ToBytes());
        
        Assert.Equal(PublicScanKeyTest, userAddress.ScanPubKey.ToBytes());
        Assert.Equal(PublicReadKeyTest, userAddress.ReadPubKey.ToBytes());
        
        Assert.Equal(AddressStringTest, userAddress.ToString());
    }

}
