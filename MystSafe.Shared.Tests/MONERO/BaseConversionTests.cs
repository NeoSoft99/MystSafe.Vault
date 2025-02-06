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
using MoneroSharp;
using MystSafe.Shared.Crypto;

namespace MystSafe.Shared.Tests;

public class BaseConversionTests
{
    [Fact]
    public void TestMoneroBase58VariableLengthConversions()
    {
        for (int i = 1; i <= 1024; i++)
        {
            int length = i;
            byte[] input = new byte[length];
            RingSig.generate_random_bytes(input, length);
            string input_string = BitConverter.ToString(input);
            if (Debugger.IsAttached)
            {
                Console.WriteLine("input_string: " + input_string);
                Console.WriteLine("input length: " + length);
            }

            byte[] encoded_bytes = Base58.Encode(input);
            
            if (Debugger.IsAttached)
            {
                Console.WriteLine("base58 string: " + Codecs.FromBytesToASCII(encoded_bytes));
                Console.WriteLine("base58 length: " + encoded_bytes.Length);
            }
            
            byte[] output = Base58.Decode(encoded_bytes);
            string output_string = BitConverter.ToString(output);
            
            if (Debugger.IsAttached)
            {
                Console.WriteLine("output_string: " + output_string);
                Console.WriteLine("output length: " + output.Length);
            }
            
            Assert.Equal(input_string, output_string);
        }
    }
    
    [Fact]
    public void TestMoneroBase58Conversions()
    {
        byte[] input = new byte[32];
        RingSig.random_scalar(input);
        string input_string = Codecs.FromBytesToHex(input);

        byte[] input_bytes = Base58.Encode(input);
       
        
        byte[] output = Base58.Decode(input_bytes);
        string output_string = Codecs.FromBytesToHex(output);
        
        Assert.Equal(input_string, output_string);
    }
    
    [Theory]
    [InlineData("12b66991d7d7c685", "48Y3H2eSZ6C")]
    [InlineData("13533d0560f820d7", "4EUjY1B5viS")]
    public void TestCodecsBase58Conversions(string ascii, string expectedBase58)
    {
        byte[] bytes = Codecs.FromHexToBytes(ascii);
        //byte[] bytes = NBitcoin.DataEncoders.Encoders.ASCII.DecodeData(ascii);

        //string nbitcoinBase58 = NBitcoin.DataEncoders.Encoders.Base58.EncodeData(bytes);
        //Assert.Equal(expectedBase58, nbitcoinBase58);
        
        // Convert ASCII to Base58
        string base58Result = Codecs.FromBytesToBase58(bytes);
        Assert.Equal(expectedBase58, base58Result);

        byte[] back_converstion = Codecs.FromBase58ToBytes(base58Result);
        string back_hex = Codecs.FromBytesToHex(back_converstion);
        Assert.Equal(ascii, back_hex);
    }
    
    /*[Theory]
    [InlineData("a",  "YQ==")]
    [InlineData("bbb", "YmJi")]
    [InlineData("ccc", "Y2Nj")]
    [InlineData("simply a long string", "c2ltcGx5IGEgbG9uZyBzdHJpbmc=")]
    public void TestBase64Conversions(string ascii, string expectedBase64)
    {
        //byte[] bytes = System.Text.Encoding.UTF8.GetBytes(ascii);
        byte[] bytes = NBitcoin.DataEncoders.Encoders.ASCII.DecodeData(ascii);
        
        string nbitcoinBase64 = NBitcoin.DataEncoders.Encoders.Base64.EncodeData(bytes);
        Assert.Equal(expectedBase64, nbitcoinBase64);

        // Convert ASCII to Base64
        string base64Result = Codecs.FromBytesToBase64(bytes);
        Assert.Equal(expectedBase64, base64Result);
    }*/
    
   
}