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

using MystSafe.Client.Engine;
using MudBlazor;

namespace MystSafe.Client.App;

public class AvatarColor
{

    // Enumerate the values of the Color enum
    private static List<Color> AvailableColors = Enum.GetValues(typeof(Color)).Cast<Color>().ToList();

    public static Color GetColorFromContact(Contact contact)
    {

        if (contact.ColorIndex == -1)
        {
            int hash = contact.Id.GetHashCode();
            int colorIndex = Math.Abs(hash) % AvailableColors.Count;
            while (ColorAlreadyExists(colorIndex, contact.Account) &&
                   contact.Account.Contacts.Count < AvailableColors.Count)
            {
                hash++;
                colorIndex = Math.Abs(hash) % AvailableColors.Count;
            }

            contact.ColorIndex = colorIndex;

        }

        return AvailableColors[contact.ColorIndex];
    }

    private static bool ColorAlreadyExists(int colorIndex, Account account)
    {

        foreach (var contact in account.Contacts)
            if (contact.ColorIndex == colorIndex)
                return true;
        return false;
    }
}


