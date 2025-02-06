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

namespace MystSafe.Shared.Crypto;

public static class Networks
{
    public const int unknown = -1;

    public const int devnet = 0;
    public const int testnet = 1;
    public const int mainnet = 2;
    public const int stage = 3;
    public const int custom = 4;

    public static string ToString(int value)
    {
        switch (value)
        {
            case devnet: return "devnet";
            case testnet: return "testnet";
            case mainnet: return "mainnet";
            case custom: return "custom";
            case stage: return "stage";
            default: return "unknown";
        }
    }

    public static int FromString(string value)
    {
        switch (value)
        {
            case "devnet": return devnet;
            case "testnet": return testnet;
            case "mainnet": return mainnet;
            case "stage": return stage;
            case "custom": return custom;
            default: return unknown;
        }
    }
}

