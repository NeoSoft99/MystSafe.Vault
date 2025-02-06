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


public class
    ChatBlockDecoder : ChatBlockValidator, IBlockDecoder<MsgBlockData> //LicensableBlockCodec<InitBlock, MsgBlockData>
{
    //public MsgBlockData MessageDataObject { get; set; }
    public MsgBlockData MsgDataUnpacked { get; set; }

    public ChatBlockDecoder(InitBlock block) : base(block)
    {

    }

    public ChatInitData DecodeSelfData(SecKey sender_secret_address_read_key)
    {
        var init_data_decrypted = DiffieHellman.Decrypt(
            sender_secret_address_read_key,
            BlockPubKey,
            CalculateBlockSalt(),
            InitData);
        return new ChatInitData(init_data_decrypted);
    }



    //  this is when scanning the network for its own blocks and restoring the data that was sent to a peer
    public MsgBlockData DecodeMsgDataBySelf(SecKey chatPrivateKey, string recipientSecretPublicKey)
    {
        if (string.IsNullOrEmpty(BlockData))
        {
            MsgDataUnpacked = MsgBlockData.EmptyData;
        }
        else
        {
            var msg_data_decrypted =
                DiffieHellman.Decrypt(
                    chatPrivateKey,
                    recipientSecretPublicKey,
                    CalculateBlockSalt(),
                    BlockData);

            MsgDataUnpacked = new MsgBlockData(msg_data_decrypted);
        }

        return MsgDataUnpacked;
    }

    public MsgBlockData Decode(SecKey readKey)
    {
        if (string.IsNullOrEmpty(BlockData))
        {
            MsgDataUnpacked = MsgBlockData.EmptyData;
        }
        else
        {
            var msg_data_decrypted = DecryptBlockData(readKey);
            MsgDataUnpacked = new MsgBlockData(msg_data_decrypted);
        }

        return MsgDataUnpacked;
    }

   
} 


