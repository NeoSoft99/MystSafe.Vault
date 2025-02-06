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

using System.Diagnostics;
using MystSafe.Shared.Crypto;
using MystSafe.Shared.Common;

namespace MystSafe.Client.Engine;

public class ContactBlockEncoder : ContactBlockValidator, IBlockEncoder<ContactBlock>
{
    // these are temporary private fields
    private string _recipient_user_address;
    private string _sender_user_address;
    private KeyPair _block_key_pair;
    private string _sender_nick_name;
    private SecretAddress _sender_secret_address;
    private ContactRequestCommands _command;
    private string _peer_nick_name;

    public ContactBlockEncoder(
        KeyPair block_key_pair,
        string recipient_user_address,
        string sender_user_address,
        string sender_nick_name,
        SecretAddress sender_secret_address,
        ContactRequestCommands command,
        int network,
        bool delete_flag,
        int height,
        string prev_hash,
        string peer_nick_name,
        int license_type)
    {
        _block_key_pair = block_key_pair;
        _recipient_user_address = recipient_user_address;
        _sender_user_address = sender_user_address;
        _sender_nick_name = sender_nick_name;
        _sender_secret_address = sender_secret_address;
        _command = command;
        Network = network;
        DeleteFlag = delete_flag;
        Height = height;
        PrevHash = prev_hash;
        _peer_nick_name = peer_nick_name;
        LicenseType = license_type;
        Difficulty = license_type != Constants.FREE_LICENSE_TYPE
            ? Constants.LICENSED_NETWORK_DIFFICULTY
            : Constants.FREE_NETWORK_DIFFICULTY;
    }

    public async Task<ContactBlock> Encode()
    {
        return await Task.Run(() =>
        {
            TimeStamp = UnixDateTime.Now;
            Version = 1;
            if (string.IsNullOrEmpty(PrevHash)) 
                PrevHash = RandomSeed.GenerateRandomSeed();
            BlockPubKey = _block_key_pair.PublicKey.ToString();
            License = string.Empty;
            Reserved = string.Empty;

            var recipient_address = PublicAddress.RecreateFromAddressString(_recipient_user_address, Network);
            var sender_address = PublicAddress.RecreateFromAddressString(_sender_user_address, Network);

            var block_salt = CalculateBlockSalt();
            RecipientStealthAddress = StealthAddress.GenerateNew(recipient_address.ScanPubKey.ToString(), _block_key_pair.PrivateKey, block_salt).ToString();
            SenderStealthAddress = StealthAddress.GenerateNew(sender_address.ScanPubKey.ToString(), _block_key_pair.PrivateKey, block_salt).ToString();
            
            //if (Debugger.IsAttached)
            {
                Console.WriteLine("ContactBlock Encode() ----------->");
                Console.WriteLine("block_salt: " + block_salt);
                Console.WriteLine("recipient_address.ScanPubKey: " + recipient_address.ScanPubKey.ToString());
                Console.WriteLine("sender_address.ScanPubKey: " + sender_address.ScanPubKey.ToString());
                Console.WriteLine("_block_key_pair.PrivateKey: " + _block_key_pair.PrivateKey.ToString());
                Console.WriteLine("_block_key_pair.PublicKey: " + _block_key_pair.PublicKey.ToString());
            }

            var init_data = new ContactRequestData();

            init_data.RecipientUserAddress = _recipient_user_address;
            init_data.SecretAddress = _sender_secret_address;
            init_data.BlockPrivateKey = _block_key_pair.PrivateKey.ToString();
            init_data.PeerNickname = _peer_nick_name;

            InitData = DiffieHellman.Encrypt(
                _block_key_pair.PrivateKey,
                sender_address.ReadPubKey.ToString(),
                block_salt,
                init_data.ToString());

            var msg_data = new MsgBlockData();

            msg_data.AddParam(MsgBlockData.SENDER_ADDRESS, _sender_user_address);
            msg_data.AddParam(MsgBlockData.SENDER_NICKNAME, _sender_nick_name);
            msg_data.AddParam(MsgBlockData.SENDER_SECRET_ADDRESS, _sender_secret_address.ToString());
            msg_data.AddParam(MsgBlockData.CONTACT_REQUEST_COMMAND, ((int)_command).ToString());

            var msg_data_str = msg_data.ToString();

            BlockData =
                DiffieHellman.Encrypt(
                    _block_key_pair.PrivateKey,
                    recipient_address.ReadPubKey.ToString(),
                    block_salt,
                    msg_data_str);

            SignBlock(_block_key_pair.PrivateKey);

            return CopyToProto();
        });
    }

    public ContactBlock CopyToProto()
    {
        var block = new ContactBlock();
        block.Hash = this.Hash;
        block.TimeStamp = this.TimeStamp;
        block.Height = this.Height;
        block.PrevHash = this.PrevHash;
        block.Network = this.Network;
        block.PubKey = this.BlockPubKey;
        block.RecipientStealthAddress = this.RecipientStealthAddress;
        block.SenderStealthAddress = this.SenderStealthAddress;
        block.InitData = this.InitData;
        block.MessageData = this.BlockData;
        block.Version = this.Version;
        block.Difficulty = this.Difficulty;
        block.Nonce = this.Nonce;
        block.Signature = this.Signature;
        block.License = this.License;
        block.LicenseType = this.LicenseType;
        block.DeleteFlag = this.DeleteFlag;
        block.Reserved = this.Reserved;
        return block;
    }

    public async Task<ContactBlock> EncodeDelete(string? message_hash_to_delete = null)
    {
        return await Task.Run(() =>
        {
            TimeStamp = UnixDateTime.Now;
            //Difficulty = 1;
            Version = 1;
            Nonce = 0;
            //PrevHash = string.Empty;
            BlockPubKey = _block_key_pair.PublicKey.ToString();
            License = string.Empty;
            Reserved = string.Empty;
            License = string.Empty;
            var recipient_address = PublicAddress.RecreateFromAddressString(_recipient_user_address, Network);
            var sender_address = PublicAddress.RecreateFromAddressString(_sender_user_address, Network);

            var block_salt = CalculateBlockSalt();
            RecipientStealthAddress = StealthAddress
                .GenerateNew(recipient_address.ScanPubKey.ToString(), _block_key_pair.PrivateKey, block_salt).ToString();
            SenderStealthAddress = StealthAddress
                .GenerateNew(sender_address.ScanPubKey.ToString(), _block_key_pair.PrivateKey, block_salt).ToString();

            InitData = string.Empty;

            BlockData = string.Empty;

            SignBlock(_block_key_pair.PrivateKey);

            return CopyToProto();
        });
    }

    public async Task<ContactBlock> EncodeUpdate(string? message_hash_to_update = null)
    {
        throw new NotImplementedException();
    }

    public override string CalculateBlockHash()
    {
        return MinePoWHash();
    }
}


