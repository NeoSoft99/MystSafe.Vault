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

using System.ComponentModel;
using MystSafe.Shared.Crypto;

namespace MystSafe.Shared.Common;

public interface IPrinableBlock
{
    void PrintBlock();
}

public class BlockExpiredWithNoLicenseException: Exception
{

}

public abstract class BaseBlockValidator<BlockType> : IPrinableBlock
{

    public string Hash { get; set; }
    public int Height { get; set; }
    public long TimeStamp { get; set; }
    public string BlockPubKey { get; set; }
    public string BlockData { get; set; }

    public string PrevHash { get; set; }

    //public int Difficulty { get; set; }
    //public int Nonce { get; set; }
    public int Version { get; set; }
    public int Network { get; set; }
    public bool DeleteFlag { get; set; }
    public string Signature { get; set; }
    public string Reserved { get; set; }

    //// this is the output decoding field
    //public MsgBlockData MsgDataUnpacked { get; set; }

    public BaseBlockValidator(BlockType block)
    {

    }

    public BaseBlockValidator()
    {
    }

    //protected string MinePoWHash()
    //{
    //    var pow_input = GetPoWInput();
    //    var pow = new PoW(pow_input, Difficulty);
    //    pow.Mine();
    //    Nonce = pow.Nonce;
    //    return pow.HashBase58;
    //}

    //protected void ValidatePoW()
    //{


    //    var pow = new PoW(Difficulty);
    //    if (!pow.Validate(Hash))
    //        throw new ApplicationException("PoW validation failed");
    //}


    // protected void SignBlock(string blockPrivateKey)
    // {
    //     Hash = CalculateBlockHash();
    //     Signature = ECDSA.Sign(blockPrivateKey, BlockPubKey, Hash);
    // }

    protected virtual void SignBlock(SecKey blockPrivateKey)
    {
        Hash = CalculateBlockHash();
        Signature = ECDSA.Sign(Hash, BlockPubKey, blockPrivateKey);
    }

    protected string DecryptBlockData(SecKey readKey)
    {
        var msg_data_decrypted =
            DiffieHellman.Decrypt(
                readKey,
                BlockPubKey,
                CalculateBlockSalt(),
                BlockData);

        return msg_data_decrypted;
    }

    protected bool VerifyHash()
    {
        return Hash == CalculateBlockHash();
    }

    protected virtual bool VerifyBlockSignature()
    {
        return ECDSA.VerifySignature(Hash, BlockPubKey, Signature);
    }


    protected virtual void CommonValidate()
    {
        if (!VerifyHash())
        {
            this.PrintBlock();
            throw new ApplicationException("Hash validation failed.");
        }

        if (!VerifyBlockSignature())
            throw new ApplicationException("Signature validation failed.");
    }

    public virtual void ServerValidate()
    {
        CommonValidate();
    }

    public virtual void ClientValidate()
    {
        CommonValidate();
    }

    public virtual string CalculateBlockHash()
    {
        throw new NotImplementedException();
    }
    
    public virtual string CalculateBlockSalt()
    {
        var str =
            "|" + PrevHash + "|" + 
            TimeStamp + "|" + 
            Height + "|" + 
            DeleteFlag + "|" + 
            Reserved + "|";
        return GenHash(str);
    }


    protected static string GenHash(string data)
    {
        //return Hashing.SHA256Base58(data);
        return Hashing.KeccakBase58(data);
    }

   
    // public virtual int DataSize
    // {
    //     get { return CalculateBufferSizeInBytes(BlockData); }
    // }

    public virtual void PrintBlock()
    {
        Console.WriteLine("Hash                    : {0}", this.Hash);
        Console.WriteLine("Height                  : {0}", this.Height);
        Console.WriteLine("Pubkey                  : {0}", this.BlockPubKey);
        Console.WriteLine("Timestamp               : {0}", UnixDateTime.ToDateTime(this.TimeStamp));

        Console.WriteLine("Version                 : {0}", this.Version);
        Console.WriteLine("Network                 : {0}", this.Network.ToString());
        Console.WriteLine("BlockData               : {0}", this.BlockData);
        Console.WriteLine("Previous Hash           : {0}", this.PrevHash);

        //Console.WriteLine("Difficulty              : {0}", this.Difficulty);
        //Console.WriteLine("Nonce                   : {0}", this.Nonce);
        Console.WriteLine("Delete Flag             : {0}", this.DeleteFlag);
        Console.WriteLine("Signature               : {0}", this.Signature);
        Console.WriteLine("Reserved                : {0}", this.Reserved);
    }
} 

