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

using System;
using System.Diagnostics.CodeAnalysis;

namespace Fido2NetLib;

public static class EnumExtensions
{
    /// <summary>
    /// Gets the enum value from EnumMemberAttribute's value.
    /// </summary>
    /// <typeparam name="TEnum">The type of enum.</typeparam>
    /// <param name="value">The EnumMemberAttribute's value.</param>
    /// <returns>TEnum.</returns>
    /// <exception cref="ArgumentException">No XmlEnumAttribute code exists for type " + typeof(TEnum).ToString() + " corresponding to value of " + value</exception>
    public static TEnum ToEnum<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>(this string value) where TEnum : struct, Enum
    {
        // Try with value from EnumMemberAttribute
        if (EnumNameMapper<TEnum>.TryGetValue(value, out var result))
        {
            return result;
        }

        // Then check the enum
        if (Enum.TryParse(value, out result))
            return result;

        throw new ArgumentException($"Value '{value}' is not a valid enum name of '{typeof(TEnum)}'. Valid values are: {string.Join(", ", EnumNameMapper<TEnum>.GetNames())}.");
    }

    /// <summary>
    /// Gets the EnumMemberAttribute's value from the enum's value.
    /// </summary>
    /// <typeparam name="TEnum">The type of enum.</typeparam>
    /// <param name="value">The enum's value</param>
    /// <returns>string.</returns>
    public static string ToEnumMemberValue<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>(this TEnum value) where TEnum : struct, Enum
    {
        return EnumNameMapper<TEnum>.GetName(value);
    }

}
