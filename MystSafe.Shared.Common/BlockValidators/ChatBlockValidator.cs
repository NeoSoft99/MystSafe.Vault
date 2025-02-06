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

namespace MystSafe.Shared.Common
{
   
    public class ChatBlockValidator: LicensableBlockValidator<InitBlock> //, ILicensableBlock //LicensableBlockCodec<InitBlock, MsgBlockData>
    {

        public string RecipientStealthAddress { get; set; }
        public string SenderStealthAddress { get; set; }
        public int ChatExpirationDays { get; set; }
        public string InitData { get; set; }

        
        //public string License { get; set; }
        //public bool ProLicense { get; set; }
        //public int LicenseType { get; set; }
        //public MsgBlockData MessageDataObject { get; set; }

        protected ChatBlockValidator()
        {

        }

        public ChatBlockValidator(InitBlock block) : base(block)
        {
            Hash = block.Hash;
            Height = block.Height;
            TimeStamp = block.TimeStamp;
            Difficulty = block.Difficulty;
            Version = block.Version;
            Nonce = block.Nonce;
            PrevHash = block.PrevHash;
            Network = block.Network;
            BlockPubKey = block.ChatPubkey;
            BlockData = block.MessageData;
            Signature = block.Signature;
            License = block.License;
            LicenseType = block.LicenseType;
            RecipientStealthAddress = block.RecipientStealthAddress;
            SenderStealthAddress = block.SenderStealthAddress;
            InitData = block.InitData;
            ChatExpirationDays = block.ChatExpirationDays;
            Reserved = block.Reserved;
        }

        protected override void CommonValidate()
        {
            // TO DO - validate the block with deleted BlockData by searching and validating the corresponding Delete message block!!! 
            if (string.IsNullOrEmpty(BlockData))
                return;

            base.CommonValidate();

     
        }

        public override void ServerValidate()
        {
            CommonValidate();

            ValidatePoW();

            ValidateLicenseType();

            //if ((!ProLicense && Difficulty != Constants.FREE_NETWORK_DIFFICULTY) ||
            //   (ProLicense && Difficulty != Constants.LICENSED_NETWORK_DIFFICULTY))
            //    throw new ApplicationException("Incorrect network difficulty");

            //if (!ProLicense && BlockData.Length > Constants.MAX_FREE_MESSAGE_SIZE)
            //    throw new LicenseValidationException(LicenseValidationException.LICENSE_VALIDATION_ERROR_DATA_SIZE_MESSAGE);
        }

        public override void ClientValidate()
        {
            CommonValidate();

            ValidateRetention();

            //var chat_interval = UnixTimeInterval.FromRetentionInterval(Network, typeof(InitBlock));
            //var chat_threshold = UnixDateTime.DeletionThreshold(chat_interval);
            //if (TimeStamp < chat_threshold && !ProLicense)
            //    throw new BlockExpiredWithNoLicenseException();

        }

        public override void PrintBlock()
        {
            Console.WriteLine("\nNew {0}", "Chat Init block");
            base.PrintBlock();
            Console.WriteLine("RecipientStealthAddress : {0}", this.RecipientStealthAddress);
            Console.WriteLine("SenderStealthAddress    : {0}", this.SenderStealthAddress);
            Console.WriteLine("ChatExpirationDays      : {0}", this.ChatExpirationDays);
            Console.WriteLine("InitData                : {0}", this.InitData);
            //Console.WriteLine("License                 : {0}", this.License);
            //Console.WriteLine("License Type            : {0}", this.LicenseType);
        }

        public override string GetPoWInput()
        {
            var str =
                "|" + Version + "|" +
                TimeStamp + "|" +
                BlockData + "|" +
                Difficulty + "|" +
                PrevHash + "|" +
                Network + "|" +
                BlockPubKey + "|" +
                RecipientStealthAddress + "|" +
                SenderStealthAddress + "|" +
                InitData + "|" +
                ChatExpirationDays + "|" +
                Reserved + "|" +
                LicenseType + "|" +
                License + "|";
            return str;
        }

    } 
}

