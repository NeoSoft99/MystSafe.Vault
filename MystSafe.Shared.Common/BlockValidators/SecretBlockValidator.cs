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

   
    public class SecretBlockValidator : LicensableBlockValidator<SecretBlock> 
    {
        public string BlockStealthAddress { get; set; }
        public string Group { get; set; }
        public string GroupSignature { get; set; }
        public string SecretValue { get; set; }

        public string DeletionHash { get; set; }
        public int Expiration { get; set; }
        public string ExpiredSig { get; set; }

        protected SecretBlockValidator()
        {
            
        }
        
        public SecretBlockValidator(SecretBlock block) : base(block)
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
            BlockData = block.SecretData;
            DeleteFlag = block.DeleteFlag;
            Signature = block.Signature;
            License = block.License;
           
            LicenseType = block.LicenseType;

            BlockStealthAddress = block.StealthAddress;
            Group = block.SecretGroup;
            GroupSignature = block.SecretGroupSignature;
            SecretValue = block.SecretValue;

            DeletionHash = block.DeletionHash;
            Expiration = block.Expiration;
            ExpiredSig = block.ExpiredSig;

            Reserved = block.Reserved;
        }




        public override string CalculateBlockSalt()
        {
            var str =
                "|" + PrevHash + "|" +
                TimeStamp + "|" +
                Height + "|" +
                DeleteFlag + "|" +
                Expiration + "|" +
                License + "|";

            return GenHash(str);
        }

        // this is for the expired blocks, after the block data is deleted
        protected string CalculateExpiredHash()
        {
            var str =
                "|" + Height + "|" +
                Version + "|" +
                TimeStamp + "|" +
                Group + "|" +
                GroupSignature + "|" +
                Difficulty +
                PrevHash + "|" +
                Network + "|" +
                BlockPubKey + "|" +
                BlockStealthAddress + "|" +
                DeleteFlag + "|" +
                Expiration + "|" +
                Reserved + "|" +
                LicenseType + "|" +
                License + "|";

            return GenHash(str);
        }

        public string GetValueSalt(string param_name, string stealth_salt)
        {
            //return Hashing.SHA256Base64(param_name + stealth_salt); 
            return Hashing.KeccakBase58(param_name + stealth_salt); 
        }


        public string CalculateGroupHash()
        {
            var str =
                "|" + Network + "|" +
                Height + "|" +
                PrevHash + "|" +
                TimeStamp + "|" +
                BlockData + "|" +
                BlockPubKey + "|" +
                Group + "|" +
                BlockStealthAddress + "|" +
                SecretValue + "|" +
                DeleteFlag + "|" +
                Expiration + "|" +
                Reserved + "|" +
                LicenseType + "|" +
                License + "|";

            return GenHash(str);
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
                    throw new ApplicationException("Expired Hash validation failed.");

                if (!VerifyExpiredSignature())
                    throw new ApplicationException("Expired Signature validation failed.");
            }
        }

        public override void ServerValidate()
        {
            CommonValidate();

            ValidatePoW();

            ValidateLicenseType();
        }

        //public void ValidateLicenseType()
        //{
        //    if ((LicenseType == Constants.FREE_LICENSE_TYPE && Difficulty != Constants.FREE_NETWORK_DIFFICULTY) ||
        //        (LicenseType != Constants.FREE_LICENSE_TYPE && Difficulty != Constants.LICENSED_NETWORK_DIFFICULTY))
        //        throw new ApplicationException("Incorrect network difficulty");

        //    if (LicenseType == Constants.FREE_LICENSE_TYPE && BlockData.Length > Constants.MAX_FREE_SECRET_SIZE)
        //        throw new LicenseValidationException(LicenseValidationException.LICENSE_VALIDATION_ERROR_DATA_SIZE_MESSAGE);
        //}

        public override void ClientValidate()
        {

            CommonValidate();

            ValidateRetention();

            //var retention_interval = UnixTimeInterval.FromRetentionInterval(Network, typeof(SecretBlock));
            //var deletion_threshold = UnixDateTime.DeletionThreshold(retention_interval);
            //if (TimeStamp < deletion_threshold && LicenseType == Constants.FREE_LICENSE_TYPE)
            //    throw new BlockExpiredWithNoLicenseException();
        }

        private bool VerifyExpiredHash()
        {
            return DeletionHash == CalculateExpiredHash();
        }

        private bool VerifyExpiredSignature()
        {
            return ECDSA.VerifySignature(DeletionHash, BlockPubKey, ExpiredSig);
        }


        public override void PrintBlock()
        {
            Console.WriteLine("\nNew {0}", "Secret block");
            base.PrintBlock();
            Console.WriteLine("StealthAddress          : {0}", this.BlockStealthAddress);
            Console.WriteLine("Group                   : {0}", this.Group);
            Console.WriteLine("GroupSignature          : {0}", this.GroupSignature);
            Console.WriteLine("SecretValue             : {0}", this.SecretValue);
            Console.WriteLine("Expiration              : {0}", UnixDateTime.ToDateTime(this.Expiration));
            Console.WriteLine("DeletionHash            : {0}", this.DeletionHash);
            Console.WriteLine("ExpiredSig              : {0}", this.ExpiredSig);
            //Console.WriteLine("License                 : {0}", this.License);
            //Console.WriteLine("License Type            : {0}", this.LicenseType);
        }

        public override string GetPoWInput()
        {
            var str =
                "|" + Height + "|" +
                Version + "|" +
                TimeStamp + "|" +
                BlockData + "|" +
                Group + "|" +
                GroupSignature + "|" +
                SecretValue + "|" +
                Difficulty + "|" +
                //Nonce +
                PrevHash + "|" +
                Network + "|" +
                BlockPubKey + "|" +
                BlockStealthAddress + "|" +
                DeleteFlag + "|" +
                Expiration + "|" +
                DeletionHash + "|" +
                Reserved + "|" +
                LicenseType + "|" +
                License + "|";
            return str;
        }

    }


