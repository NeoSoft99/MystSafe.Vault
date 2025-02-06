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

using Org.BouncyCastle.Security;

namespace MystSafe.Shared.Crypto;

public static class RandomSeed
{
    public static byte[] GenerateSecureRandom32ByteValue()
    {
        SecureRandom secureRandom = new SecureRandom();
        byte[] randomValue = new byte[32]; 
        secureRandom.NextBytes(randomValue);
        return randomValue;
    }

    // returns random seed base58 string
    public static string GenerateRandomSeed()
    {
        byte[] randomValue = GenerateSecureRandom32ByteValue();
        string result = Codecs.FromBytesToBase58(randomValue);
        return result;
    }
}

