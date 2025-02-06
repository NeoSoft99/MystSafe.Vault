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

using MystSafe.Shared.CryptoLicense;

public class AmountDecomposerTests
{
    [Fact]
    public void DecomposeAmount_SimpleCase()
    {
        // Arrange
        decimal amount = 156.3m;
        // Act
        List<Output> result = Transaction.DecomposeAmount(amount);

        // Assert
        Assert.Equal(4, result.Count);
        Assert.Equal(100, result[0].Amount);
        Assert.Equal(50, result[1].Amount);
        Assert.Equal(6, result[2].Amount);
        Assert.Equal(0.3m, result[3].Amount);
    }

    [Fact]
    public void DecomposeAmount_ZeroAmount()
    {
        // Arrange
        decimal amount = 0;

        // Act
        var result = Record.Exception(() => Transaction.DecomposeAmount(amount));

        // Assert
        Assert.NotNull(result);
        Assert.IsType<System.ArgumentException>(result);
    }
    
    
    [Fact]
    public void DecomposeAmount_SmallAmount()
    {
        // Arrange
        decimal amount = 0.05m;
        // Act
        List<Output> result = Transaction.DecomposeAmount(amount);

        // Assert
        Assert.Single(result);
        Assert.Equal(0.05m, result[0].Amount);
    }

    [Fact]
    public void DecomposeAmount_LargeAmount()
    {
        // Arrange
        decimal amount = 123456.78m;
        // Act
        List<Output> result = Transaction.DecomposeAmount(amount);

        // Assert
        Assert.Equal(8, result.Count);
        Assert.Equal(100000, result[0].Amount);
        Assert.Equal(20000, result[1].Amount);
        Assert.Equal(3000, result[2].Amount);
        Assert.Equal(400, result[3].Amount);
        Assert.Equal(50, result[4].Amount);
        Assert.Equal(6, result[5].Amount);
        Assert.Equal(0.7m, result[6].Amount);
        Assert.Equal(0.08m, result[7].Amount);
    }

    [Fact]
    public void DecomposeAmount_NegativeAmount()
    {
        // Arrange
        decimal amount = -123.45m;
        // Act
        var result = Record.Exception(() => Transaction.DecomposeAmount(amount));

        // Assert
        Assert.NotNull(result);
        Assert.IsType<System.ArgumentException>(result);
    }
    
    [Theory]
    [InlineData(12345678.12345678)] // Precisely at limit
    [InlineData(0.00000001)]             // Smallest amount above zero valid under the precision
    [InlineData(1)]
    [InlineData(100.12345678)]
    public void DecomposeAmount_ValidAmounts_ReturnsCorrectDecomposition(decimal amount)
    {
        // Act
        var result = Transaction.DecomposeAmount(amount);

        // Assert
        Assert.NotEmpty(result);
        decimal total = 0;
        foreach (var output in result)
        {
            total += output.Amount;
        }
        Assert.Equal(amount, total);
    }

    [Theory]
    [InlineData(0)] // Zero amount
    [InlineData(-1)] // Negative amount
    public void DecomposeAmount_NegativeOrZero_ThrowsArgumentException(decimal amount)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Transaction.DecomposeAmount(amount));
        Assert.Equal("Amount must be greater than zero.", exception.Message);
    }

    [Theory]
    [InlineData(0.000000001)] // More precision than allowed
    [InlineData(123.123456789)] // Exceeds precision
    public void DecomposeAmount_ExcessivePrecision_ThrowsArgumentException(decimal amount)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => Transaction.DecomposeAmount(amount));
        Assert.Equal($"Amount cannot have more than {GlobalConstants.AMOUNT_PRECISION} decimal places.", exception.Message);
    }
}
