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

	public class ResultStatusCodes
    {
		public const int SUCCESS = 0;
        public const int NODE_IS_DOWN = -1;
        public const int NOT_FOUND = -2;
        public const int WRONG_NETWORK = -3;
        public const int BLOCK_ALREADY_EXISTS = -4;
        public const int BLOCK_FAILED_TO_ADD_TO_DATABASE = -5;

        public const int EXCEPTION = -10;
        public const int EXPIRED = -11;

        public const int AUTHENTICATION_FAILED = -20;

        public const int WRONG_AUTHENTICATION_PARAMETER = -21;

        public const int UNKNOWN_ERROR = -100;

        // Licenses
        public const int LICENSE_VIOLATION = -200; // attempting to do somethign that is not covered by free license
        public const int NO_MASTER_LICENSE_KEYS_FOUND = -201;

        // Contacts / chats
        public const int CONTACT_IS_NOT_ESTABLISHED = -300;
    }


