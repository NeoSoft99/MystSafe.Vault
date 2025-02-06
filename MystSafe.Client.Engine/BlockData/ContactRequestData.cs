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

using MystSafe.Client.Base;
using Newtonsoft.Json;
using MystSafe.Shared.Crypto;

namespace MystSafe.Client.Engine;

// this is the internal data of the contact block - can be only read by the sender itself
public class ContactRequestData: BaseBlockData
{
    public string RecipientUserAddress { get; set; }
    public string SecretScanKey { get; set; }
    public string SecretReadKey { get; set; }

    // this is the block private key
    public string BlockPrivateKey { get; set; }

    // This is to store the peer nickname if it is updated by the sender during UpdateContact
    // inititially, it is set to empty string
    public string PeerNickname { get; set; } 

    [Newtonsoft.Json.JsonIgnore]
    public SecretAddress SecretAddress { get; set; }

    // use this one to create a new msg data to send
    public ContactRequestData(): base()
		{
    }

    // use this one to restore the received msg data
    public ContactRequestData(string contactData, int network) : base(contactData)
    {
        // need to deserialize from MessageData string here
        //var options = new JsonSerializerOptions
        //{
        //    PropertyNameCaseInsensitive = true
        //};

        //var data = JsonSerializer.Deserialize<ContactRequestData>(contactData, options);
        var data = JsonConvert.DeserializeObject<ContactRequestData>(contactData);

        Params = data.Params;
        SecretReadKey = data.SecretReadKey;
        SecretScanKey = data.SecretScanKey;
        RecipientUserAddress = data.RecipientUserAddress;
        SecretAddress = SecretAddress.RestoreFromKeys(data.SecretReadKey, data.SecretScanKey, network); //data.SecretAddress;
        BlockPrivateKey = data.BlockPrivateKey;
        PeerNickname = data.PeerNickname;

    }

    public override string ToString()
    {
        SecretScanKey = SecretAddress.ScanKey.ToString();
        SecretReadKey = SecretAddress.ReadKey.ToString();
        return JsonConvert.SerializeObject(this);
    }

}

