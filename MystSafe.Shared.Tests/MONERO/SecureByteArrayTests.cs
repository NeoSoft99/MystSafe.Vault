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
using System.Text;

namespace MystSafe.Shared.Tests;

public class SecureByteArrayTests
{
    [Fact]
    public void Constructor_NullInput_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SecureByteArray(null));
    }
    
    [Fact]
    public void Length_ReturnsSameData()
    {
        // Arrange
        byte[] inputData = Codecs.FromASCIIToBytes("SensitiveData");
        using (var secureByteArray = new SecureByteArray(inputData))
        {
            // Act
            byte[] outputData = secureByteArray.GetBytes();
            Console.WriteLine(secureByteArray.ToString());
            // Assert
            Assert.Equal(inputData.Length, outputData.Length);
        }
    }

    [Fact]
    public void GetBytes_InputData_ReturnsSameData()
    {
        // Arrange
        byte[] inputData = Codecs.FromASCIIToBytes("SensitiveData");
        using (var secureByteArray = new SecureByteArray(inputData))
        {
            // Act
            byte[] outputData = secureByteArray.GetBytes();

            // Assert
            Assert.Equal(inputData, outputData);
        }
    }

    [Fact]
    public void Dispose_ClearsSecureString()
    {
        // Arrange
        byte[] inputData = Codecs.FromASCIIToBytes("SensitiveData");
        var secureByteArray = new SecureByteArray(inputData);
        Console.WriteLine(secureByteArray.ToString());

        // Act
        secureByteArray.Dispose();

        // Assert
        Assert.Throws<ObjectDisposedException>(() => secureByteArray.GetBytes());
    }
}