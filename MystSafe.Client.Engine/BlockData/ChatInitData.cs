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

using Newtonsoft.Json;
using MystSafe.Client.Base;
using MystSafe.Shared.Crypto;

namespace MystSafe.Client.Engine;

	public class ChatInitData: BaseBlockData
{
    //public string ChatKeyBase64 { get; set; }
    public string ChatKeyBase58 { get; set; }
    //public string MsgKey { get; set; }
    public string RecipientAddress { get; set; }
    public string RecipientSecretAddress { get; set; }

    // use this one to create a new msg data to send
    public ChatInitData(): base()
		{
    }

    // use this one to restore the received msg data
    public ChatInitData(string initData): base(initData)
    {

        ChatInitData data = JsonConvert.DeserializeObject<ChatInitData>(initData);

        Params = data.Params;
        ChatKeyBase58 = data.ChatKeyBase58;
        //MsgKey = data.ChatKey;
        RecipientAddress = data.RecipientAddress;
        RecipientSecretAddress = data.RecipientSecretAddress;
    }

    public override string ToString()
    {
        // need to serialize the content to a string here
        return JsonConvert.SerializeObject(this);
    }

}

