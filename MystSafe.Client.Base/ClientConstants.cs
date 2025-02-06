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

namespace MystSafe.Client.Base;

public static class ClientConstants
{
	public const int MAX_ACCOUNT_NICKNAME_LENGTH_BYTES = 25;

	//public const int INIT_BLOCK_SCAN_INTERVAL_SECONDS = 5 * 1000; // 5 sec but can be more

	public const int CONTEXT_SCAN_PAGE_SIZE_BLOCKS = 100;

	public const int SECRET_SCAN_PAGE_SIZE_BLOCKS = 100;

	public const int LICENSE_SCAN_PAGE_SIZE_BLOCKS = 100;

	// for dev env. only
	public const int CHAT_BLOCK_RENEWAL_DEV_MINUES = 2;

	// chat init blocks are generated daily so the messages can gradually deleted after retention period on daily basic,
	// to keep an overall message history for the retention period
	public const int CHAT_BLOCK_RENEWAL_DAYS = 1;

	public const int DEFAULT_INACTIVITY_TIMEOUT_SECONDS = 5 * 60; // 5 minutes

	public const int CHAT_MESSAGES_UPDATE_INTERVAL_SECONDS = 5; // should be around 5

	public const int SECRETS_REFRESH_INTERVAL_SECONDS = 30; // 
}

