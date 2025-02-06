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
using MoneroRing.Crypto;
using MoneroSharp.NaCl;
using MoneroSharp.Utils;
using MystSafe.Shared.Crypto;
using Org.BouncyCastle.Utilities.Bzip2;

namespace MystSafe.Shared.CryptoLicense;


public class Output // this is the output that can be used by the wallet as an input for next Tx
{
    public string? StealthAddress; // steals address of the funds recipient, created from recipient's address (scan pubkey) and Tx private key; empty if burning
    //public string? PublicKey; // the output data loaded from the Tx
    public decimal Amount;
    //public string OutputData; // the output private key (SecKey) encrypted by recipient's read pub key and Tx private key
    public int Index; // Output Index in the Tx

    public string TxPubKey;
    
    public int TokenId; // Id of the token being transacted; default: 0 - license token; 1 - reward token.

    public Output()
    {
    }
    
    public Output(decimal amount)
    {
        Amount = amount;
    }
    
    // call it only when restoring from comms or DB
    public Output(decimal amount, string stealthAddress, int index, string txPubKey, int tokenId)
    {
        Amount = amount;
        StealthAddress = stealthAddress;
        Index = index;
        TxPubKey = txPubKey;
        TokenId = tokenId;
    }
    
    // Calculate output key P (stealth address)
    // P = Hs(rA || i) + B
    // txPrivateKey: r 
    // recipientAddress: A and B
    // index: i
    public void SetStealthAddress(string recipientAddress, SecKey txPrivateKey, int network)
    {
        var recipient = PublicAddress.RecreateFromAddressString(recipientAddress, network);
        var A = recipient.ScanPubKey.ToBytes();
        var B = recipient.ReadPubKey.ToBytes();
        var r = txPrivateKey.ToBytes();
        var derivation = new byte[32];
        try
        {
            if (!RingSig.generate_key_derivation(A, r, derivation))
                throw new ApplicationException("Could not derive shared secret");
            var P = new byte[32];
            if (!RingSig.derive_public_key(derivation, (uint)Index, B, P))
                throw new ApplicationException("Could not derive public key");
            //Index = index;
            //var result = new PubKey(P).ToBase58();
            //return result;
            StealthAddress = new PubKey(P).ToBase58();
        }
        finally
        {
            CryptoBytes.Wipe(r);
            CryptoBytes.Wipe(derivation);
        }
    }

    // x = H_s(aR) + b
    /*public static SecKey DeriveOutputSpendPrivateKey(PubKey txPubKey, SecKey recipientScanKey, SecKey recipientReadKey, int output_index)
    {
        byte[] x = new byte[32];
        byte[] shared_secret = new byte[32];
        try
        {
            RingSig.generate_key_derivation(txPubKey.ToBytes(), recipientScanKey.ToBytes(), shared_secret);
            RingSig.derive_secret_key(shared_secret, (uint)output_index, recipientReadKey.ToBytes(), x);
            return new SecKey(x);
        }
        finally
        {
            CryptoBytes.Wipe(x);
            CryptoBytes.Wipe(shared_secret);
        }
    }*/
    
    // x = Hs(aR)+b
    public SecKey DeriveOutputSpendPrivateKey(SecKey recipientScanKey, SecKey recipientReadKey)
    {
        byte[] x = new byte[32];
        byte[] shared_secret = new byte[32];
        try
        {
            RingSig.generate_key_derivation(Codecs.FromBase58ToBytes(TxPubKey), recipientScanKey.ToBytes(), shared_secret);
            RingSig.derive_secret_key(shared_secret, (uint)Index, recipientReadKey.ToBytes(), x);
            return new SecKey(x);
        }
        finally
        {
            CryptoBytes.Wipe(x);
            CryptoBytes.Wipe(shared_secret);
        }
    }


    // examines the stealth address to determine whether this output was aimed to this wallet's address
    // P = Hs(aR || i) + B
    public static bool IsSentToMe(string stealthAddress, UserAddress walletAddress, string txPubKey, int outputIndex)
    {
        var derivation = new byte[32];
        var a = walletAddress.ScanKey.ToBytes();
        try
        {
            var B = walletAddress.ReadPubKey.ToBytes();
            var R = PubKey.FromBase58String(txPubKey).ToBytes();
            var P = PubKey.FromBase58String(stealthAddress).ToBytes();

            if (!RingSig.generate_key_derivation(R, a, derivation))
                throw new ApplicationException("Could not derive shared secret");
            var P1 = new byte[32];
            if (!RingSig.derive_public_key(derivation, (uint)outputIndex, B, P1))
                throw new ApplicationException("Could not derive public key");
            return P1.SequenceEqual(P);
        }
        finally
        {
            CryptoBytes.Wipe(a);
            CryptoBytes.Wipe(derivation);
        }
    }
    
    public string CalculateHash()
    {
        var hashInput =   "|" +StealthAddress + "|" + TxPubKey + "|" + Amount.ToString("0.00") + "|" + Index + "|" + TokenId + "|";
        return GenHash(hashInput);
    }
    
    protected static string GenHash(string data) => Hashing.KeccakBase58(data);
}







