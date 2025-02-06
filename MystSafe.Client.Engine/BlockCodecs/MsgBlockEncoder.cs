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

namespace MystSafe.Client.Engine;

public class MsgBlockEncoder : MsgBlockValidator, IBlockEncoder<MsgBlock>
{
    // these are temporary private fields
    private string _chatReadPubKey;
    private int _prevHeight;
    private SecKey _chatKey;
    private string _message_text;
    private int _message_type;

    public MsgBlockData MsgDataUnpacked { get; set; }

    public MsgBlockEncoder(
        string chat_read_pub_key,
        int prev_height,
        string prev_hash,
        string chatPubKey,
        SecKey chatKey,
        string message_text,
        int message_type,
        int network,
        bool has_license)
    {
        _chatReadPubKey = chat_read_pub_key;
        _prevHeight = prev_height;
        PrevHash = prev_hash;
        BlockPubKey = chatPubKey;
        _chatKey = chatKey;
        _message_type = message_type;
        _message_text = message_text;
        Network = network;
        Difficulty = has_license ? Constants.LICENSED_NETWORK_DIFFICULTY : Constants.FREE_NETWORK_DIFFICULTY;
    }

    private void PreEncode()
    {
        TimeStamp = UnixDateTime.Now;
        //Difficulty = 1;
        Version = 1;
        //Nonce = 0;

        DeleteFlag = false;
        DeletionHash = string.Empty;
        Expiration = 0;
        ExpiredSig = string.Empty;
        BlockData = string.Empty;
        Reserved = string.Empty;

        //if (!string.IsNullOrWhiteSpace(PrevHash) && _prevHeight > 0)
        if (!string.IsNullOrWhiteSpace(PrevHash) && _prevHeight > 0)
        {
            Height = _prevHeight + 1;
        }
        else
        {
            Height = 1;
            //PrevHash = string.Empty;
        }
    }

    public MsgBlock CopyToProto()
    {
        var block = new MsgBlock();

        block.Hash = this.Hash;
        block.TimeStamp = this.TimeStamp;
        block.Height = this.Height;
        block.Network = this.Network;
        block.PrevHash = this.PrevHash;
        block.ChatPubKey = this.BlockPubKey;
        block.MessageData = this.BlockData;
        block.Version = this.Version;
        block.Difficulty = this.Difficulty;
        block.Nonce = this.Nonce;
        block.Signature = this.Signature;

        block.DeleteFlag = this.DeleteFlag;
        block.Expiration = this.Expiration;
        block.DeletionHash = this.DeletionHash;
        block.ExpiredSig = this.ExpiredSig;

        block.Reserved = this.Reserved;

        return block;
    }

    private void EncodeBlockData()
    {
        MsgDataUnpacked = new MsgBlockData();
        MsgDataUnpacked.MessageType = _message_type;
        MsgDataUnpacked.MsgText = _message_text;

        var msg_data_str = MsgDataUnpacked.ToString();

        BlockData =
            DiffieHellman.Encrypt(
                _chatKey,
                _chatReadPubKey,
                CalculateBlockSalt(),
                msg_data_str);
    }

    public async Task<MsgBlock> Encode()
    {
        return await Task.Run(() =>
        {
            PreEncode();

            EncodeBlockData();

            SignBlock(_chatKey);

            return CopyToProto();
        });
    }

    public async Task<MsgBlock> EncodeDelete(string? message_hash_to_delete)
    {
        return await Task.Run(() =>
        {
            PreEncode();

            BlockData = string.Empty;
            DeleteFlag = true;
            DeletionHash = message_hash_to_delete;

            SignBlock(_chatKey);

            return CopyToProto();
        });
    }

    public async Task<MsgBlock> EncodeUpdate(string? message_hash_to_update)
    {
        return await Task.Run(() =>
        {
            PreEncode();

            EncodeBlockData();

            DeleteFlag = true;
            DeletionHash = message_hash_to_update;

            SignBlock(_chatKey);

            return CopyToProto();
        });
    }

    public async Task<MsgBlock> EncodeSelfExpiring(int expiration)
    {
        return await Task.Run(() =>
        {
            PreEncode();

            EncodeBlockData();

            Expiration = expiration;

            ExpiredSignBlock(_chatKey);

            EncodeBlockData();

            SignBlock(_chatKey);

            return CopyToProto();
        });
    }

    private void ExpiredSignBlock(SecKey blockKey)
    {
        DeletionHash = CalculateExpiredHash();
        ExpiredSig = ECDSA.Sign(DeletionHash, BlockPubKey, blockKey); // blockKey, DeletionHash);
    }

    public override string CalculateBlockHash()
    {
        return MinePoWHash();
    }
} 


