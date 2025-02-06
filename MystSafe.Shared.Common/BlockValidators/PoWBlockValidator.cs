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

public abstract class PoWBlockValidator<BlockType> : BaseBlockValidator<BlockType>
{
    public int Difficulty { get; set; }
    public int Nonce { get; set; }

    public PoWBlockValidator(BlockType block)
    {
    
    }

    public PoWBlockValidator()
    {
    }

    protected string MinePoWHash()
    {
        var pow_input = GetPoWInput();
        var pow = new PoW(pow_input, Difficulty);
        pow.Mine();
        Nonce = pow.Nonce;
        return pow.HashBase58;
    }

    protected void ValidatePoW()
    {


        var pow = new PoW(Difficulty);
        if (!pow.Validate(Hash))
            throw new ApplicationException("PoW validation failed");
    }

    //public virtual void ServerValidate()
    //{
    //    CommonValidate();
    //}

    //public virtual void ClientValidate()
    //{
    //    CommonValidate();
    //}

    public virtual string GetPoWInput()
    {
        throw new NotImplementedException();
    }

    public override string CalculateBlockHash()
    {
        var pow_input = GetPoWInput();
        return GenHash(pow_input + Nonce);
    }

    public override void PrintBlock()
    {
        base.PrintBlock();
        Console.WriteLine("Difficulty              : {0}", this.Difficulty);
        Console.WriteLine("Nonce                   : {0}", this.Nonce);
    }
} 

