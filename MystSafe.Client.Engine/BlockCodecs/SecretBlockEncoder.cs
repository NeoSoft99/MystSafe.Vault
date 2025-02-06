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

using Microsoft.Extensions.Logging;

namespace MystSafe.Client.Engine;

public class SecretBlockEncoder : SecretBlockValidator, IBlockEncoder<SecretBlock>
{
    // these are temporary private fields
    private int _prevHeight;
    private KeyPair _block_key_pair;
    private UserAddress _user_address;
    private SecretBlockData _block_data;
    private string[]? _variables;
    private readonly ILogger<SendProcessor> _logger;


    public SecretBlockEncoder(
        KeyPair block_key_pair,
        UserAddress user_address,
        int prev_height,
        string prev_hash,
        SecretBlockData block_data,
        string[]? variables,
        int network,
        int license_type,
        ILogger<SendProcessor> logger
    )
    {
        _block_key_pair = block_key_pair;
        _user_address = user_address;
        _block_data = block_data;
        _variables = variables;
        _prevHeight = prev_height;

        Network = network;
        PrevHash = prev_hash;
        BlockPubKey = _block_key_pair.PublicKey.ToString();
        _logger = logger;
        LicenseType = license_type;
        Difficulty = license_type != Constants.FREE_LICENSE_TYPE
            ? Constants.LICENSED_NETWORK_DIFFICULTY
            : Constants.FREE_NETWORK_DIFFICULTY;
    }

    private void PreEncode()
    {
        TimeStamp = UnixDateTime.Now;
        //Difficulty = 1;
        Version = 1;
        //Nonce = 0;
        License = string.Empty;

        DeleteFlag = false;
        DeletionHash = string.Empty;
        Expiration = 0;
        ExpiredSig = string.Empty;
        BlockData = string.Empty;
        SecretValue = string.Empty;
        GroupSignature = string.Empty;
        Group = string.Empty;
        Reserved = string.Empty;

        BlockPubKey = _block_key_pair.PublicKey.ToString();

        if (!string.IsNullOrWhiteSpace(PrevHash) && _prevHeight > 0)
        {
            Height = _prevHeight + 1;
        }
        else
        {
            Height = 0;
            //PrevHash = string.Empty;
            PrevHash = RandomSeed.GenerateRandomSeed();
            // TO DO - request and random recent block from the network and use its hash
        }
    }

    private void EncodeBlockData()
    {
        //byte[] group_private_key_data;

        var salt = CalculateBlockSalt();
        BlockStealthAddress = StealthAddress.GenerateNew(
            _user_address.HiddenScanPubKey.ToString(), 
            _block_key_pair.PrivateKey, 
            salt).ToString();
        
        _block_data.BlockPrivateKey = _block_key_pair.PrivateKey.ToString();

        var block_data_str = _block_data.ToString();

        //_logger.LogInformation("block_data_str: " + block_data_str);

        BlockData =
            DiffieHellman.Encrypt(
                _block_key_pair.PrivateKey.ToString(),
                _user_address.ReadPubKey.ToString(),
                salt,
                block_data_str);

// TO DO - implement later
//GroupSignature = SignGroupData(group_key_pair.KeyBase64);
    }


    public async Task<SecretBlock> Encode()
    {
        return await Task.Run(() =>
        {
            PreEncode();

            EncodeBlockData();

            SignBlock(_block_key_pair.PrivateKey);

            return CopyToProto();
        });
    }

    public async Task<SecretBlock> EncodeUpdate(string? message_hash_to_update = null)
    {
        return await Task.Run(() =>
        {
            PreEncode();

            DeleteFlag = true;

            EncodeBlockData();

            SignBlock(_block_key_pair.PrivateKey);

            return CopyToProto();
        });
    }

    public async Task<SecretBlock> EncodeDelete(string? message_hash_to_delete = null)
    {
        return await Task.Run(() =>
        {
            PreEncode();

            DeleteFlag = true;

            var salt = CalculateBlockSalt();
            BlockStealthAddress = StealthAddress
                .GenerateNew(_user_address.HiddenScanPubKey.ToString(), _block_key_pair.PrivateKey, salt).ToString();


            SignBlock(_block_key_pair.PrivateKey);

            return CopyToProto();
        });
    }

    public async Task<SecretBlock> EncodeInstantShare(int expiration)
    {
        return await Task.Run(() =>
        {
            PreEncode();
            Expiration = expiration;

            var instant_key_bytes = Codecs.FromBase58ToBytes(_block_data.InstantKey);
            var block_data_str = _block_data.ToString();
            var salt = CalculateBlockSalt();
            SecretValue = AES.Encrypt(instant_key_bytes, salt, block_data_str);

            EncodeBlockData();

            ExpiredSignBlock(_block_key_pair.PrivateKey);

            SignBlock(_block_key_pair.PrivateKey);

            return CopyToProto();
        });
    }

    private void ExpiredSignBlock(SecKey blockKey)
    {
        DeletionHash = CalculateExpiredHash();
        ExpiredSig = ECDSA.Sign(DeletionHash, BlockPubKey, blockKey);
    }

    public SecretBlock CopyToProto()
    {
        var block = new SecretBlock();

        block.Hash = this.Hash;
        block.TimeStamp = this.TimeStamp;
        block.Height = this.Height;
        block.PrevHash = this.PrevHash;
        block.PubKey = this.BlockPubKey;
        block.SecretData = this.BlockData;
        block.Network = this.Network;
        block.SecretGroup = this.Group;
        block.SecretGroupSignature = this.GroupSignature;
        block.SecretValue = this.SecretValue;
        block.StealthAddress = this.BlockStealthAddress;

        block.Version = this.Version;
        block.Difficulty = this.Difficulty;
        block.Nonce = this.Nonce;
        block.DeleteFlag = this.DeleteFlag;
        block.Signature = this.Signature;
        block.License = this.License;
        block.LicenseType = this.LicenseType;
        block.Expiration = this.Expiration;
        block.DeletionHash = this.DeletionHash;
        block.ExpiredSig = this.ExpiredSig;

        block.Reserved = this.Reserved;

        return block;
    }

    public override string CalculateBlockHash()
    {
        return MinePoWHash();
    }
}


