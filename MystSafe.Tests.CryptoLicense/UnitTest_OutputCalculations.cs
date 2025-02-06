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

using MoneroRing.Crypto;
using MoneroSharp.Utils;

public class UnitTest_OutputCalculations
{
    const string pub_hex =            "e46b60ebfe610b8ba761032018471e5719bb77ea1cd945475c4a4abe7224bfd0";
    const string sec_hex =            "981d477fb18897fa1f784c89721a9d600bf283f06b89cb018a077f41dcefef0f";
    const string expected_image_hex = "a637203ec41eab772532d30420eac80612fce8e44f1758bc7e2cb1bdda815887";
    const string wrong_sec_hex =      "ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";
    
    byte[] pub = MoneroUtils.HexBytesToBinary(pub_hex);
    byte[] sec = MoneroUtils.HexBytesToBinary(sec_hex);
    byte[] expectedImage = MoneroUtils.HexBytesToBinary(expected_image_hex);
    byte[] wrongSec = MoneroUtils.HexBytesToBinary(wrong_sec_hex);

    [Fact]
    public void GenerateKeyImage_ThrowsException_WithInvalidPrivateKey()
    {
        byte[] image = new byte[32]; 
        
        var ex = Assert.Throws<Exception>(() => RingSig.generate_key_image(pub, wrongSec, image));
        Assert.Equal("invalid private key", ex.Message);
    }

    [Fact]
    public void GenerateKeyImage_CorrectlyGeneratesImage_WithValidKeys()
    {
        byte[] image = new byte[32];
        
        RingSig.generate_key_image(pub, sec, image);

        Assert.Equal(expectedImage, image);
    }

}
