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

public class Constants
{
    public const int VERSION = 0;

    public const string MESSAGE_SEPARATOR = "||";

    public const int FREE_NETWORK_DIFFICULTY = 2;

    public const int LICENSED_NETWORK_DIFFICULTY = 1;

    //public const string DEVNET_INITIAL_MASTER_LICENSE_PRIVATE_KEY = "6sbC79e37gBFMpD4Q9zgjAYYiaFZ2cN3duCVAiBXa6av";

    //public const string DEVNET_INITIAL_MASTER_LICENSE_PUBLIC_KEY = "fA9qjquxNFkJ2S92sy4XpwMCSLszu9Vb4f61whJSux6X";

    //public const string MAINNET_INITIAL_MASTER_LICENSE_PUBLIC_KEY = "23UDSWrBNk4r6VttNahvUHa9U2pHZqzgTFVQUP6K6Uvu";

    public const int MAX_RING_SIGNATURE_LIMIT = 20;

    public const int MAX_FREE_MESSAGE_SIZE = 3072; // encrypted message data length

    public const int MAX_FREE_SECRET_SIZE = 3072; // encrypted secret length

    public const int FREE_TRIAL_SECRET_RETENTION_PERIOD_DAYS = 30; // secret and contact blocks

    public const int FREE_TRIAL_MESSAGE_RETENTION_PERIOD_DAYS = 7; // chat init and message blocks

    public const int FREE_TRIAL_SECRET_RETENTION_PERIOD_DEV_MINUTES = 4; // development only (localhost or dev.mystsafe.com)

    public const int FREE_TRIAL_MESSAGE_RETENTION_PERIOD_DEV_MINUTES = 2; // development only (localhost or dev.mystsafe.com)

    // TO DO - FREE_TRIAL_MESSAGE_DEFAULT_RETENTION_PERIOD_DAYS = 3;
    // TO DO - FREE_TRIAL_MESSAGE_MAX_RETENTION_PERIOD_DAYS = 7;
    // TO DO - LICENSED_MESSAGE_DEFAULT_RETENTION_PERIOD_DAYS = 14;
    // TO DO - LICENSED_MESSAGE_MAX_RETENTION_PERIOD_DAYS = 365;

    public const int FREE_LICENSE_TYPE = 0;

    public const int PREMIUM_LICENSE = 1;

    public const int PRO_LICENSE = 2;



}