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

using MystSafe.Shared.CryptoLicense;

namespace MystSafe.Client.CryptoLicense;

    public class OutputLocalRecord
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string StealthAddress { get; set; }
        public string AccountId { get; set; }
        public int Index { get; set; }

        public int TokenId { get; set; }
        public string TxPubKey { get; set; }

        public decimal Amount { get; set; }
        public bool Spent { get; set; }
        //public string SpentTxHash { get; set; }
        public string SpendTxPubKey { get; set; }
        
        public static OutputLocalRecord FromDTO(OutputDTO outputDTO)
        {
            var outputRecord = new OutputLocalRecord();
            outputRecord.Index = outputDTO.Index;
            outputRecord.Amount = outputDTO.Amount;
            outputRecord.StealthAddress = outputDTO.StealthAddress;
            outputRecord.TokenId = outputDTO.TokenId;
            return outputRecord;
        }
        
        public Output ToOutput()
        {
            var output = new Output();
            output.StealthAddress = StealthAddress;
            output.Index = Index;
            output.TxPubKey = TxPubKey;
            output.Amount = Amount;
            output.TokenId = TokenId;
            return output;
        }

    }

    public class TxLocalRecord
    {
        [System.ComponentModel.DataAnnotations.Key]
        public string PubKey { get; set; }
        

        //public string AccountId { get; set; }
        public long TimeStamp { get; set; }
        //public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        //public string FeeTarget { get; set; } // Id of a data block that's being targeted for fee payment 
        
        public TxLocalRecord()
        {

        }

        public static TxLocalRecord FromDTO(TxDTO txDTO)
        {
            var txRecord = new TxLocalRecord();
            txRecord.TimeStamp = txDTO.TimeStamp;
            txRecord.Fee = txDTO.Fee;
            //txRecord.FeeTarget = txDTO.FeeTarget;
            //txRecord.TokenId = txDTO.TokenId;
            txRecord.PubKey = txDTO.PubKey;

            return txRecord;
        }

    }
