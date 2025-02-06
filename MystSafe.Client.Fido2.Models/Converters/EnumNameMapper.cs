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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;

namespace Fido2NetLib;

public static class EnumNameMapper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] TEnum>
    where TEnum : struct, Enum
{
    private static readonly Dictionary<TEnum, string> s_valueToNames = GetIdToNameMap();
    private static readonly Dictionary<string, TEnum> s_namesToValues = Invert(s_valueToNames);

    private static Dictionary<string, TEnum> Invert(Dictionary<TEnum, string> map)
    {
        var result = new Dictionary<string, TEnum>(map.Count, StringComparer.OrdinalIgnoreCase);

        foreach (var item in map)
        {
            result[item.Value] = item.Key;
        }

        return result;
    }

    public static bool TryGetValue(string name, out TEnum value)
    {
        return s_namesToValues.TryGetValue(name, out value);
    }

    public static string GetName(TEnum value)
    {
        return s_valueToNames[value];
    }

    public static IEnumerable<string> GetNames()
    {
        return s_namesToValues.Keys;
    }

    private static Dictionary<TEnum, string> GetIdToNameMap()
    {
        var dic = new Dictionary<TEnum, string>();

        foreach (var field in typeof(TEnum).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var description = field.GetCustomAttribute<EnumMemberAttribute>(false);

            var value = (TEnum)field.GetValue(null);

            dic[value] = description is not null ? description.Value : value.ToString();
        }

        return dic;
    }
}
