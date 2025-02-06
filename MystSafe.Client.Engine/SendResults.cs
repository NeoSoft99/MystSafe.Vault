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

using MystSafe.Shared.Common;

namespace MystSafe.Client.Engine;

public abstract class BaseSendResult
{
    public int ResultCode = ResultStatusCodes.UNKNOWN_ERROR;
    public string ResultMessage = string.Empty;
}

public class SendContactRequestResult: BaseSendResult
{
    public Contact? NewContact;
}

public class SendSecretResult : BaseSendResult
{
    public Secret? NewSecret;
}

public class SendMessageResult : BaseSendResult
{
    public Message? NewMessage;
}

public class SendChatOutResult : BaseSendResult
{
    public ChatOut? NewChatOut;
}

public class SendLicenseFeeResult: BaseSendResult
{
    public string? LicensePubKey;
}
