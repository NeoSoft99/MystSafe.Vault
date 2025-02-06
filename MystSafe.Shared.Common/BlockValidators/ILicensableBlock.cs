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

namespace MystSafe.Shared.Common;

public interface ILicensableBlock
{
    string Hash { get; set; }
    int Height { get; set; }
    long TimeStamp { get; set; }
    string BlockPubKey { get; set; }
    int Network { get; set; }
    string BlockData { get; set; }

    string License { get; set; }

    //bool ProLicense { get; set; }
    int LicenseType { get; set; }

    decimal CalculateFeeAmount();

    int DataSizeInBytes();
}