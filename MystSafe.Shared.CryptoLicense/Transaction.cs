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

namespace MystSafe.Shared.CryptoLicense;

// public enum TxTypes
// {
//     NotSet = 0,
//     Mining = 1, // create new license tokens
//     Burning = 2, // use license tokens
//     Transfer = 3 // send license tokens
// }

// public enum TxDirections // income or spending for particular wallet
// {
//     Undefined = 0, // it is neither in or out for a validating node
//     In = 1,
//     Out = 2
// }

public class Transaction
{
    public string PubKey; // one-time Tx public key; base58
    //public TxTypes TxType;
    //public TxDirections TxDirection; // determine whether it is income or spending for this wallet
    //public string Hash; // 32 bytes in base58; points to the Tx block that contains the output
    //public string PrevTxPubKey; // Points to one of most recent Tx in Tx database
    
    public long TimeStamp; // MystSafe unix format
    
    //public decimal Amount; // Total amount of transaction including fee: Sum of all outputs + Fee = Sum of all inputs
    public decimal Fee;
    
    //public string FeeTarget; // Id of a data block that's being targeted for fee payment 

    //public int TokenId; // Id of the token being transacted; default: 0 - license token; 1 - reward token.

    public int Network; // This is MystSafe network, don't confuse with Monero

    //public string SenderAddress; // stealth address of the sender that allows the owner to find their transactions in the ledger 
    public string Signature; // Signing the Hash using Tx SecKey that can be validated with Tx PubKey
    
    public List<Input> Inputs;
    public List<Output> Outputs;

    public Transaction()
    {
        Inputs = new List<Input>();
        Outputs = new List<Output>();
    }
    
    public decimal TotalInputsAmount
    {
        get
        {
            decimal total = 0;
            foreach (var input in Inputs)
            {
                total += input.Amount;
            } 
            return total;
        }
    }
    
    // public virtual string CalculateSalt()
    // {
    //     return GenHash(this.PrevHash + this.TimeStamp + this.Amount + this.PubKey);
    // }

    protected static string GenHash(string data) => Hashing.KeccakBase58(data);

    public void Sign(SecKey txPrivateKey)
    {
        byte[] hash = Codecs.FromBase58ToBytes(CalculateHash());
        byte[] public_key = Codecs.FromBase58ToBytes(PubKey);

        var sig = ECDSA.Sign(hash, public_key, txPrivateKey.ToBytes());
        this.Signature = Codecs.FromBytesToBase58(sig);
    }

    public virtual string CalculateHash()
    {
        var hashInput =
            "|" +
            this.TimeStamp + "|" + 
            this.Fee.ToString("0.00") + "|" + 
            //this.FeeTarget + "|" + 
            this.Network + "|" + 
            this.PubKey+ "|";

        foreach (var input in Inputs)
        {
            hashInput += input.CalculateHash();
        }
        foreach (var output in Outputs)
        {
            hashInput += output.CalculateHash();
        }

        return GenHash(hashInput);
    }
   
    public void AddOutput(string recipientAddress, SecKey txPrivateKey, int tokenId, Output output)
    {
        int maxIndex = Outputs.Count > 0 ? Outputs.Max(o => o.Index) : -1;
        output.Index = maxIndex + 1;
        output.SetStealthAddress(recipientAddress, txPrivateKey, Network);
        output.TxPubKey = PubKey;
        output.TokenId = tokenId;
        Outputs.Insert((int)output.Index, output);
    }
    
    // public void AddInput(Input input)
    // {
    //     Inputs.Insert((int)input.Index, input);
    // }
    
    public void AddInput(decimal inputAmount, int tokenId, uint inputIndex, byte[][] pubs, SecKey x, int secIndex)
    {
        var input = new Input(inputAmount, inputIndex, tokenId);
        input.CreateRingSignature(pubs, pubs.Length, x, secIndex, MystSafe.Shared.Crypto.PubKey.FromBase58String(PubKey));
        Inputs.Insert((int)input.Index, input);
    }
    
    // // this is the change - the difference between the sum of the inputs and the burn amount - going back to the sender
    // public void GenerateChangeOutputs(string destination_address, SecKey txPrivateKey, int network)
    // {
    //     // total Tx amount = transfer_amount + fee + change_amount = TotalInputsAmount
    //     var denominated_outputs = DecomposeAmount(TotalInputsAmount - Fee);
    //     foreach (var output in denominated_outputs)
    //     {
    //         AddOutput(destination_address, txPrivateKey, output);
    //     }
    // }
    
    
    private static void ValidateAmount(decimal amount)
    {
        // Validate the input amount
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");
        
        if ((amount * (decimal)Math.Pow(10, GlobalConstants.AMOUNT_PRECISION)) % 1 != 0)
        {
            throw new ArgumentException($"Amount cannot have more than {GlobalConstants.AMOUNT_PRECISION} decimal places.");
        }
    }
    
    public static List<Output> DecomposeAmount(decimal amount)
    {
        ValidateAmount(amount);
        
        List<Output> outputs = new List<Output>();
        double logBase10 = (double)amount;
        int power = (int)Math.Floor(Math.Log10(Math.Abs(logBase10)));

        while (amount >= (decimal)Math.Pow(10, -GlobalConstants.AMOUNT_PRECISION))
        {
            decimal powerOfTen = (decimal)Math.Pow(10, power);
            decimal leadingDigit = Math.Floor(amount / powerOfTen);
            if (leadingDigit != 0)
            {
                var output_amount = leadingDigit * powerOfTen;
                ValidateAmount(output_amount);
                outputs.Add(new Output(output_amount));
                //output_index_counter++;
                amount -= leadingDigit * powerOfTen;
            }
            power--;
        }

        // Handle remaining decimal fraction if any
        if (amount > 0) // This captures any remaining decimal part
        {
            decimal roundedDecimal = Math.Round(amount, 2); // Round to two decimal places
            ValidateAmount(roundedDecimal);
            outputs.Add(new Output(roundedDecimal));
            //output_index_counter++;
        }

        return outputs;
    }
  
    public TxDTO ToDTO()
    {
        var dto = new TxDTO
        {
            TimeStamp = TimeStamp,
            PubKey = PubKey,
            Fee = Fee,
            //FeeTarget = FeeTarget,
            //TokenId = TokenId,
            //PrevTxPubKey = PrevTxPubKey,
            Network = Network,
            Signature = Signature,
            Inputs = Inputs.Select(input => new InputDTO
            {
                PubKeys = input.PubKeys,
                KeyImage = input.KeyImage,
                RingSignature = input.RingSignature,
                Index = input.Index,
                Amount = input.Amount,
                TokenId = input.TokenId
            }).ToList(),
            Outputs = Outputs.Select(output => new OutputDTO
            {
                StealthAddress = output.StealthAddress,
                Amount = output.Amount,
                Index = output.Index,
                TxPubKey = output.TxPubKey,
                TokenId = output.TokenId
            }).ToList()
        };

        return dto;
    }
    
    public static Transaction FromDto(TxDTO dto)
    {
        return new Transaction
        {
            TimeStamp = dto.TimeStamp,
            Fee = dto.Fee,
            //FeeTarget = dto.FeeTarget,
            //TokenId = dto.TokenId,
            PubKey = dto.PubKey,
            //PrevTxPubKey = dto.PrevTxPubKey,
            Network = dto.Network,
            Signature = dto.Signature,
            Inputs = dto.Inputs.Select(i => new Input
            {
                Index = i.Index,
                KeyImage = i.KeyImage,
                Amount = i.Amount,
                PubKeys = i.PubKeys,
                TokenId = i.TokenId,
                RingSignature = i.RingSignature
            }).ToList(),
            Outputs = dto.Outputs.Select(o => new Output
            {
                Index = o.Index,
                StealthAddress = o.StealthAddress,
                Amount = o.Amount,
                TxPubKey = o.TxPubKey,
                TokenId = o.TokenId
            }).ToList()
        };
    }
    
    
}





