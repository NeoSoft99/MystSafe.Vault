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

using System.Text;
using Org.BouncyCastle.Security;

namespace MystSafe.Client.Base;

public static class RandomPasswordGenerator
{
    const string lowercase = "abcdefghijklmnopqrstuvwxyz";
    const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const string numeric = "0123456789";
    const string special = "!@#$%^&*()_-+=[{]};:<>|./?";

    public static string GenerateRandomPassword(
        int length,
        bool includeLowercase,
        bool includeUppercase,
        bool includeNumeric,
        bool includeSpecial)
    {


        StringBuilder characterSet = new StringBuilder();

        if (includeLowercase) characterSet.Append(lowercase);
        if (includeUppercase) characterSet.Append(uppercase);
        if (includeNumeric) characterSet.Append(numeric);
        if (includeSpecial) characterSet.Append(special);

        if (characterSet.Length == 0)
            throw new ArgumentException("At least one character set must be included.");

        byte[] data = new byte[4 * length];


        //Random random = new Random();
        SecureRandom random = new SecureRandom();
        random.NextBytes(data);

        StringBuilder password = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            uint randomIndex = BitConverter.ToUInt32(data, i * 4);
            randomIndex = randomIndex % (uint)characterSet.Length;

            password.Append(characterSet[(int)randomIndex]);
        }

        return password.ToString();
    }

}

