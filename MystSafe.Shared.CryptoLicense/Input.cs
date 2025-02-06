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

using MoneroRing.Crypto;
using MystSafe.Shared.Crypto;

namespace MystSafe.Shared.CryptoLicense;

public class Input
{
    
    //public string? Pubkey; // one-time address key; empty if burning
    public uint Index;
    public string PubKeys; // array of ring public keys; base58
    public string KeyImage; // key image; base58
    public string RingSignature; // ring signature; base58
    public decimal Amount;
    public int TokenId; // Id of the token being transacted; default: 0 - license token; 1 - reward token.
    
    //public int RealIndex;
    //public Output RealOutput;

    // public decimal Amount
    // {
    //     get
    //     {
    //         if (RealOutput != null)
    //             return RealOutput.Amount;
    //         return 0;
    //     }
    // }
    //
    //public List<Output> RingOutputs;
    
    // public Input(Output realOutput)
    // {
    //     RealOutput = realOutput;
    // }
    
    public Input(decimal amount, uint index, int tokenId)
    {
        Index = index;
        Amount = amount;
        TokenId = tokenId;
    }
    
    public Input()
    {

    }

  
    // Generates both key image and ring signayire and assigns them to the input properties.
    // sec_index: index of the private key within pubs[] array
    // pubs_count: number of pub keys including all decoys and the real key ("ring size")
    // pubs: array of all pub keys 
    // txPubKey: R
    public void CreateRingSignature(byte[][] pubs, int pubs_count, SecKey sec, int sec_index, PubKey txPubKey)
    {
        PubKeys = ByteArrayToBase58(pubs);
        
        byte[] key_image = new byte[32];
        // I = xHp(P)
        RingSig.generate_key_image(pubs[sec_index], sec.ToBytes(), key_image);
        
        KeyImage = Codecs.FromBytesToBase58(key_image);

        byte[] hash = CalculateRingHash(txPubKey);
        byte[] ring_sig = RingSig.generate_ring_signature(hash, key_image, pubs, pubs_count, sec.ToBytes(), sec_index);
        RingSignature = Codecs.FromBytesToBase58(ring_sig);
    }
    
    public static string ByteArrayToBase58(byte[][] byteArray)
    {
        // Calculate the total length of the flattened byte array
        int totalLength = 0;
        foreach (var arr in byteArray)
        {
            totalLength += arr.Length;
        }

        // Flatten the byte[][] array into a single byte[] array
        byte[] flattenedArray = new byte[totalLength];
        int offset = 0;
        foreach (var arr in byteArray)
        {
            Buffer.BlockCopy(arr, 0, flattenedArray, offset, arr.Length);
            offset += arr.Length;
        }

        // Convert the flattened byte array to a Base58 string
        //return Convert.ToBase64String(flattenedArray);
        return Codecs.FromBytesToBase58(flattenedArray);
    }

    //public static byte[][] Base64ToByteArray(string base64String)
    public static byte[][] Base58ToByteArray(string base58String)
    {
        int elementLength = 32;
        // Convert the Base58 string back to a byte array
        //byte[] flattenedArray = Convert.FromBase64String(base64String);
        byte[] flattenedArray = Codecs.FromBase58ToBytes(base58String);

        // Calculate the number of elements in the byte[] array
        int numberOfElements = flattenedArray.Length / elementLength;

        // Split the byte array back into a byte[][] array
        byte[][] byteArray = new byte[numberOfElements][];
        for (int i = 0; i < numberOfElements; i++)
        {
            byteArray[i] = new byte[elementLength];
            Buffer.BlockCopy(flattenedArray, i * elementLength, byteArray[i], 0, elementLength);
        }

        return byteArray;
    }

    public string CalculateHash()
    {
        var hashInput =   "|" +Index + "|" + PubKeys + "|" + KeyImage + "|" + Amount.ToString("0.00") + "|" + TokenId + "|" + RingSignature + "|";
        return Hashing.KeccakBase58(hashInput);
    }
    
    public byte[] CalculateRingHash(PubKey txPubKey)
    {
        var hashInput = Codecs.FromASCIIToBytes("|" + Index + "|" + PubKeys + "|" + KeyImage + "|" + Amount.ToString("0.00") + "|" + TokenId + "|" + txPubKey.ToString() + "|");
        return Hashing.KeccakBytes(hashInput);
    }
}








