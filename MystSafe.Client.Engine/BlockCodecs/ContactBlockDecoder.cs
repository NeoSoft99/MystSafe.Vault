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

using MystSafe.Shared.Crypto;
using MystSafe.Shared.Common;
using MystSafe.Client.Base;

namespace MystSafe.Client.Engine;


public class ContactBlockDecoder : ContactBlockValidator, IBlockDecoder<MsgBlockData>
{
    public MsgBlockData MsgDataUnpacked { get; set; }
    //public ContactRequestData ContactData { get; set; }

    public ContactBlockDecoder(ContactBlock block) : base(block)
    {

    }


    public ContactRequestData DecodeSelfData(SecKey readKey)
    {
        var init_data_decrypted = DiffieHellman.Decrypt(
            readKey,
            BlockPubKey,
            CalculateBlockSalt(),
            InitData);
        return new ContactRequestData(init_data_decrypted, Network);
    }

    public MsgBlockData Decode(SecKey readKey)
    {
        var msg_data_decrypted = DecryptBlockData(readKey);
        MsgDataUnpacked = new MsgBlockData(msg_data_decrypted);
        return MsgDataUnpacked;
    }
}

