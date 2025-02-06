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

using System.Text;

namespace MystSafe.Shared.Crypto;

public class PoW
{
    public string? Data { get; private set; }
    public int Nonce { get; private set; }
    public string HashBase58 { get; private set; } 
    public readonly int Difficulty;

    private readonly string Target;
    public string BinaryHashString { get; private set; }

    // for mining
    public PoW(string data, int difficulty): this(difficulty)
    {
        Data = data;
    }

    // for validation
    public PoW(int difficulty)
    {
        Difficulty = difficulty;
        Target = new string('0', Difficulty);
    }

    private byte[] CalculateHash()
    {
            byte[] inputBytes = Encoding.ASCII.GetBytes(Data+Nonce);
            //byte[] hashBytes = Hashing.SHA256Bytes(inputBytes); 
            byte[] hashBytes = Hashing.KeccakBytes(inputBytes); 
            return hashBytes;
    }

    public void Mine()
    {
        var hash_bytes = CalculateHash();
        while (!Validate(hash_bytes))
        {
            Nonce++;
            hash_bytes = CalculateHash();
        }
        HashBase58 = Codecs.FromBytesToBase58(hash_bytes);
    }

    public bool Validate(string hash_base58)
    {
        var hash_bytes = Codecs.FromBase58ToBytes(hash_base58);
        return Validate(hash_bytes);
    }

    private bool Validate(byte[]? hash_bytes)
    {
        if (hash_bytes is null || hash_bytes.Length != 32)
            throw new ApplicationException("Invalid hash");
        //Hash = hash;
        BinaryHashString = ByteArrayToBinaryString(hash_bytes);
        return BinaryHashString.StartsWith(Target);
    }

    private static string ByteArrayToBinaryString(byte[] bytes)
    {
        StringBuilder binary = new StringBuilder();
        foreach (byte b in bytes)
        {
            binary.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
        }
        return binary.ToString();
    }

}

