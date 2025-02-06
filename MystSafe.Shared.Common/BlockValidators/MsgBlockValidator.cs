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

namespace MystSafe.Shared.Common;


public class MsgBlockValidator: PoWBlockValidator<MsgBlock>
{
   
    public string DeletionHash { get; set; }
    public int Expiration { get; set; }
    public string ExpiredSig { get; set; }

    //public MsgBlockData MsgDataUnpacked { get; set; }

    protected MsgBlockValidator()
    {

    }

    public MsgBlockValidator(MsgBlock block) : base(block)
    {
        Hash = block.Hash;
        Height = block.Height;
        TimeStamp = block.TimeStamp;
        Difficulty = block.Difficulty;
        Version = block.Version;
        Nonce = block.Nonce;
        PrevHash = block.PrevHash;
        Network = block.Network;
        BlockPubKey = block.ChatPubKey;
        BlockData = block.MessageData;
        Signature = block.Signature;

        DeleteFlag = block.DeleteFlag;
        DeletionHash = block.DeletionHash;
        Expiration = block.Expiration;
        ExpiredSig = block.ExpiredSig;
        Reserved = block.Reserved;
    }

    public override void ServerValidate()
    {
        throw new NotImplementedException();
    }

    public void ServerValidate(ChatBlockValidator chat_block_validator)
    {
        CommonValidate();

        ValidatePoW();


        // assuming chat init bloack's license has been validated already, we aonly need to cehkc the message size etc.
        //chat_block_validator.ValidateLicenseType();
        if (chat_block_validator.LicenseType == Constants.FREE_LICENSE_TYPE && BlockData.Length > Constants.MAX_FREE_SECRET_SIZE)
            throw new LicenseValidationException(LicenseValidationException.LICENSE_VALIDATION_ERROR_DATA_SIZE_MESSAGE);
    }

    public override void ClientValidate()
    {
        //throw new NotImplementedException();
        CommonValidate();
    }

    public void ClientValidate(ChatBlockValidator chat_block_validator)
    {
        CommonValidate();

        chat_block_validator.ValidateRetention();
    }

    protected override void CommonValidate()
    {
        if (Expiration == 0) 
        {
            base.CommonValidate();
        }
        else // this is self-expiring message
        {
            if (!VerifyExpiredHash())
                throw new ApplicationException("Expired Hash validation failed");

            if (!VerifyExpiredSignature())
                throw new ApplicationException("Expired Signature validation failed");
        }

        if (Height == 0)
            throw new ApplicationException("Message heignt cannot be zero");
    }

    private bool VerifyExpiredHash()
    {
        return DeletionHash == CalculateExpiredHash();
    }

    private bool VerifyExpiredSignature()
    {
        return ECDSA.VerifySignature(DeletionHash, BlockPubKey, ExpiredSig);
    }
    
    // this is for the expired messages, after the block data is deleted
    protected string CalculateExpiredHash()
    {
        var str =
            "|" + Version + "|" + 
            TimeStamp + "|" + 
            Difficulty + "|" + 
            PrevHash + "|" + 
            Network + "|" + 
            BlockPubKey + "|" + 
            Height + "|" + 
            DeleteFlag + "|" + 
            Expiration + "|" + 
            Reserved + "|";

        return GenHash(str);
    }

    public override string CalculateBlockSalt()
    {
        var str =
            "|" + PrevHash +
            TimeStamp + "|" +
            Height + "|" +
            DeleteFlag + "|" +
            Expiration + "|";
        return GenHash(str);
    }


    public override void PrintBlock()
    {
        Console.WriteLine("\nNew {0}", "Message block");
        base.PrintBlock();
        Console.WriteLine("Deletion Hash           : {0}", this.DeletionHash);
        Console.WriteLine("Expiration              : {0}", UnixDateTime.ToDateTime(this.Expiration));
        Console.WriteLine("Expired Sig             : {0}", this.ExpiredSig);
    }

    public override string GetPoWInput()
    {
        var str =
           "|" + Height + "|" + 
           Version + "|" +
           TimeStamp + "|" +
           BlockData + "|" +
           Difficulty + "|" +
           PrevHash + "|" +
           Network + "|" +
           BlockPubKey + "|" +
           DeleteFlag + "|" +
           Expiration + "|" +
           DeletionHash + "|";
        return str;
    }
}

