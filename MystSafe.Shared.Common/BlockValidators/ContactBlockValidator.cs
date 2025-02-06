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

namespace MystSafe.Shared.Common;


    public enum ContactRequestCommands : int
    {
        InitialRequest = 0, // default command - typically issied when user requests contact for the first time;
                            // it anticipates reply

        Reply = 1, // this is sent in reply to the InitialRequest; does not require reply

        Update = 2, // This is duplicate request to update some info; it does not anticipate reply

        Block = 3, // this one user sends to themselves to "memorize" the contact block 
    }

    public class ContactBlockValidator: LicensableBlockValidator<ContactBlock> //, ILicensableBlock //LicensableBlockCodec<ContactBlock, MsgBlockData>
    {
        // these are extra contact block properties
        public string RecipientStealthAddress { get; set; }
        public string SenderStealthAddress { get; set; }
        public string InitData { get; set; }

        protected ContactBlockValidator()
        {

        }


        public ContactBlockValidator(ContactBlock block) : base(block)
        {
            Hash = block.Hash;
            Height = block.Height;
            TimeStamp = block.TimeStamp;
            Difficulty = block.Difficulty;
            Version = block.Version;
            Nonce = block.Nonce;
            PrevHash = block.PrevHash;
            Network = block.Network;
            BlockPubKey = block.PubKey;
            BlockData = block.MessageData;
            Signature = block.Signature;
            License = block.License;
            LicenseType = block.LicenseType;
            RecipientStealthAddress = block.RecipientStealthAddress;
            SenderStealthAddress = block.SenderStealthAddress;
            InitData = block.InitData;

            DeleteFlag = block.DeleteFlag;
            Reserved = block.Reserved;
        }

        protected override void CommonValidate()
        {
            base.CommonValidate();
        }

        public override void ServerValidate()
        {
            CommonValidate();

            ValidatePoW();

            ValidateLicenseType();

            //if ((!ProLicense && Difficulty != Constants.FREE_NETWORK_DIFFICULTY) ||
            //    (ProLicense && Difficulty != Constants.LICENSED_NETWORK_DIFFICULTY))
            //    throw new ApplicationException("Incorrect network difficulty");

        }

        public override void ClientValidate()
        {
            CommonValidate();

            ValidateRetention();

            //var contact_interval = UnixTimeInterval.FromRetentionInterval(Network, typeof(ContactBlock));
            //var contact_threshold = UnixDateTime.DeletionThreshold(contact_interval);
            //if (TimeStamp < contact_threshold && !string.IsNullOrWhiteSpace(License))
            //    throw new BlockExpiredWithNoLicenseException();
        }

        public override void PrintBlock()
        {

            Console.WriteLine("\nNew {0}", "Contact block");
            base.PrintBlock();
            Console.WriteLine("RecipientStealthAddress : {0}", this.RecipientStealthAddress);
            Console.WriteLine("SenderStealthAddress    : {0}", this.SenderStealthAddress);
            Console.WriteLine("InitData                : {0}", this.InitData);
            //Console.WriteLine("License                 : {0}", this.License);
            //Console.WriteLine("License Type            : {0}", this.LicenseType);
        }

        public override string GetPoWInput()
        {
            var str =
                "|" + Version + "|" +
                Height + "|" +
                TimeStamp + "|" +
                BlockData + "|" +
                Difficulty + "|" +
                PrevHash + "|" +
                Network + "|" +
                BlockPubKey + "|" +
                RecipientStealthAddress + "|" +
                SenderStealthAddress + "|" +
                InitData + "|" +
                DeleteFlag + "|" +
                Reserved + "|" +
                LicenseType + "|" +
                License + "|";
            return str;
        }

    }


