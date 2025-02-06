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


public class ChatBlockEncoder : ChatBlockValidator, IBlockEncoder<InitBlock>
{
    // these are temporary private fields
    private string _recipient_secret_address;
    private string _sender_secret_address;
    private KeyPair _block_key_pair;
    private string _message_text;
    private int _message_type;
    string _nick_name;

    public MsgBlockData MsgDataUnpacked { get; set; }
    //public MsgBlockData MessageDataObject { get; set; }


    public ChatBlockEncoder(
        int height,
        string prev_hash,
        KeyPair block_key_pair,
        string recipient_secret_address,
        string sender_secret_address,
        string nick_name,
        string message_text,
        int message_type,
        int chat_expiration_days,
        int network,
        int license_type)
    {
        _block_key_pair = block_key_pair;
        ChatExpirationDays = chat_expiration_days;
        _recipient_secret_address = recipient_secret_address;
        _sender_secret_address = sender_secret_address;
        _nick_name = nick_name;
        _message_type = message_type;
        _message_text = message_text;
        Network = network;
        Height = height;
        PrevHash = prev_hash;
        LicenseType = license_type;
        Difficulty = license_type != Constants.FREE_LICENSE_TYPE
            ? Constants.LICENSED_NETWORK_DIFFICULTY
            : Constants.FREE_NETWORK_DIFFICULTY;
    }

    public async Task<InitBlock> Encode()
    {
        return await Task.Run(() =>
        {
            //Height = 0;
            TimeStamp = UnixDateTime.Now;
            Version = 1;
            Reserved = string.Empty;
            if (string.IsNullOrEmpty(PrevHash))
                PrevHash = RandomSeed.GenerateRandomSeed();
            BlockPubKey = _block_key_pair.PublicKey.ToString();
            License = string.Empty;

            var recipient_secret_address = PublicAddress.RecreateFromAddressString(_recipient_secret_address, Network);
            var sender_secret_address = PublicAddress.RecreateFromAddressString(_sender_secret_address, Network);

            var salt = CalculateBlockSalt();

            RecipientStealthAddress = StealthAddress
                .GenerateNew(recipient_secret_address.ScanPubKey.ToString(), _block_key_pair.PrivateKey, salt).ToString();
            
            SenderStealthAddress = StealthAddress
                .GenerateNew(sender_secret_address.ScanPubKey.ToString(), _block_key_pair.PrivateKey, salt).ToString();

            var init_data = new ChatInitData();
            init_data.ChatKeyBase58 = _block_key_pair.PrivateKey.ToString();

            InitData = DiffieHellman.Encrypt(
                _block_key_pair.PrivateKey.ToString(),
                sender_secret_address.ReadPubKey.ToString(),
                salt,
                init_data.ToString());

            MsgDataUnpacked = new MsgBlockData();
            MsgDataUnpacked.MsgText = _message_text;
            MsgDataUnpacked.MessageType = _message_type;
            MsgDataUnpacked.AddParam(MsgBlockData.SENDER_ADDRESS,
                _sender_secret_address); // this is in case it's changing in the future
            MsgDataUnpacked.AddParam(MsgBlockData.SENDER_NICKNAME,
                _nick_name); // this is the way to update ? not sure we need it as the recipient can assign their own name 

            var msg_data_str = MsgDataUnpacked.ToString();

            BlockData =
                DiffieHellman.Encrypt(
                    _block_key_pair.PrivateKey,
                    recipient_secret_address.ReadPubKey.ToString(),
                    salt,
                    msg_data_str);

            SignBlock(_block_key_pair.PrivateKey);

            return CopyToProto();
        });
    }

    public async Task<InitBlock> EncodeDelete(string? message_hash_to_delete = null)
    {
        throw new NotImplementedException();
    }

    public async Task<InitBlock> EncodeUpdate(string? message_hash_to_update = null)
    {
        throw new NotImplementedException();
    }

    public InitBlock CopyToProto()
    {
        var block = new InitBlock();

        block.Hash = this.Hash;
        block.TimeStamp = this.TimeStamp;
        block.Height = this.Height;
        block.PrevHash = this.PrevHash;
        block.ChatPubkey = this.BlockPubKey;

        block.RecipientStealthAddress = this.RecipientStealthAddress;
        block.SenderStealthAddress = this.SenderStealthAddress;
        block.InitData = this.InitData;
        block.ChatExpirationDays = this.ChatExpirationDays;

        block.MessageData = this.BlockData;
        block.Version = this.Version;
        block.Network = this.Network;
        block.Difficulty = this.Difficulty;
        block.Nonce = this.Nonce;
        block.Signature = this.Signature;
        block.License = this.License;
        block.LicenseType = this.LicenseType;
        block.Reserved = this.Reserved;
        return block;
    }

    public override string CalculateBlockHash()
    {
        return MinePoWHash();
    }
} 


