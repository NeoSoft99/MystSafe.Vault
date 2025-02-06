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

using MoneroSharp;

namespace MystSafe.Shared.Crypto;

	public static class Codecs
	{
		/*public static string FromBytesToBase64(byte[] data)
		{
			return Convert.ToBase64String(data);  //Encoders.Base64.EncodeData(data);
		}

        public static byte[] FromBase64ToBytes(string data)
        {
	        return Convert.FromBase64String(data); //Encoders.Base64.DecodeData(data);
        }*/

		//public static byte[] FromUTF8ToBytes(string data)
		public static byte[] FromASCIIToBytes(string data)
		{
            return System.Text.Encoding.ASCII.GetBytes(data); //Encoders.ASCII.DecodeData(data); //System.Text.Encoding.UTF8.GetBytes(data);
        }
		
		public static string FromBytesToASCII(byte[] data)
		{
			return System.Text.Encoding.ASCII.GetString(data); 
		}

        public static string FromBytesToBase58(byte[] data)
        {
	        // byte[] base58Bytes = MoneroSharp.Base58.Encode(data);
	        // return System.Text.Encoding.ASCII.GetString(base58Bytes); //Encoders.Base58.EncodeData(data);
	        //return Encoders.Base58.EncodeData(data);
	        byte[] base58Bytes = Base58.Encode(data);
	        return System.Text.Encoding.ASCII.GetString(base58Bytes); //Encoders.Base58.EncodeData(data);
        }

        public static byte[] FromBase58ToBytes(string data)
        {
	        if (string.IsNullOrWhiteSpace(data))
		        throw new ArgumentException("Input string cannot be null, empty, or whitespace.", nameof(data));

	        // Valid Base58 characters
	        const string validBase58Chars = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

	        // Validate that the string only contains valid Base58 characters
	        foreach (char c in data)
	        {
		        if (!validBase58Chars.Contains(c))
		        {
			        throw new ArgumentException(
				        $"Invalid character '{c}' in the input string. " +
				        "The input can only contain valid Base58 characters.",
				        nameof(data));
		        }
	        }
	        
	        byte[] base58Bytes = System.Text.Encoding.ASCII.GetBytes(data);
	        return Base58.Decode(base58Bytes);  //Encoders.Base58.DecodeData(data);
        }
        
        public static string FromBytesToHex(byte[] data)
        {
	        return MoneroSharp.Utils.MoneroUtils.BytesToHex(data);
        }

        public static byte[] FromHexToBytes(string data)
        {
	        return MoneroSharp.Utils.MoneroUtils.HexBytesToBinary(data);
        }
    }


