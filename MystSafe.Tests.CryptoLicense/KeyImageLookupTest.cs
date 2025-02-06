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
using MystSafe.Shared.Crypto;

public class KeyImageLookupTest
{
    
    [Fact]
    public void GenerateKeyImage_ThrowsException_WithInvalidPrivateKey()
    {
        // r: Sender’s one-time private key, used to generate the transaction public key R and the shared secret rA.
        // R: Transaction public key, included in the transaction and used by the recipient to generate the shared secret.
        // a: Recipient’s private view (scan) key, used to derive the shared secret and confirm the output.
        // A: Recipient’s public view (scan) key, used by the sender to generate the shared secret.
        // b: Recipient’s private spend (read) key, used to derive the shared secret.
        // B: Recipient’s public spend (read) key, used by the sender to generate the public key P.
        // P: Public key for the output, included in the transaction and verified by the recipient.
        // x: Private key for the output, derived by the recipient and used to spend by creating the key image and ring signature.
        // I: Key image, used to prevent double-spending and included in the transaction.

        
        // Scenario:
        // 1. Someone sends 10 tokens to Bob. 
        // 2. Bob finds out that he has received 10 tokens.
        // 3. Bob sends 5 tokens to Alice 
        // 4. Bob finds out that he's spent 10 tokens

        UserAddress Bob = UserAddress.GenerateFromMnemonic(Networks.mainnet);
        KeyPair rPair1 = KeyPair.GenerateRandom();
        SecKey r1 = rPair1.PrivateKey;
        PubKey R1 = rPair1.PublicKey;
        
        // 1. Unknown Sender: generates output key P to send 10 tokens to Bob
        // P = Hs(rA)G + B
        Output output1 = new Output(10);
        output1.Index = 0;
        output1.SetStealthAddress(Bob.ToString(), r1, Networks.mainnet);
        output1.TxPubKey = R1.ToString();
        byte[] P1 = Codecs.FromBase58ToBytes(output1.StealthAddress);

        // 2. Bob: Finds out that he received tokens by trying to restore P using his a and B
        // P = Hs(aR || i) + B
        byte[] aR1 = new byte[32];
        SecKey a1 = Bob.ScanKey;
        
        if (!RingSig.generate_key_derivation(R1.ToBytes(), a1.ToBytes(), aR1))
            throw new ApplicationException("Could not derive shared secret");
        var P1_Test = new byte[32];
        PubKey B1 = Bob.ReadPubKey;
        if (!RingSig.derive_public_key(aR1, 0, B1.ToBytes(), P1_Test))
            throw new ApplicationException("Could not derive public key");
        
        // Bob: Determines whether the output is sent to him by comparing P and P1
        // If they match, the recipient can use the output in new Tx
        Assert.Equal(P1, P1_Test);
        
        // 3. Bob: wants to use tokens to send them to Alice so he creates input
        // by extracting spend key x and generating key image 
        // x = Hs(aR)+b
        // I = xHp(P)
        SecKey b1 = Bob.ReadKey;
        SecKey x1 = output1.DeriveOutputSpendPrivateKey(a1, b1);
   
        UserAddress Alice = UserAddress.GenerateFromMnemonic(Networks.mainnet);
        KeyPair rPair2 = KeyPair.GenerateRandom();
        SecKey r2 = rPair2.PrivateKey;
        PubKey R2 = rPair2.PublicKey;

        Input input = new Input(10, 0, Tokens.LICENSE_TOKEN);
        byte[][] pubs = new byte[1][];
        pubs[0] = P1;
        input.CreateRingSignature(pubs, 1, x1, 0, R2);
        
        
        // Bob: finds out he's spent 10 tokens, so he needs to mark output1 as spent in his wallet.
        // He scans through the list of outputs that were sent to him, and checks whether any of the spend keys x of those
        // outputs are used in any of the key images (i.e. they have been spent by him).
        byte[] I1 = new byte[32];
        RingSig.generate_key_image(P1, x1.ToBytes(), I1);
        Assert.Equal(I1, Codecs.FromBase58ToBytes(input.KeyImage));
     
        // Output output2 = new Output(10);
        // output1.Index = 0;
        // output1.SetStealthAddress(Bob.ToString(), r2, Networks.mainnet);
        // byte[] P2 = Codecs.FromBase58ToBytes(output2.StealthAddress);

    

        //Assert.Equal("invalid private key", ex.Message);
    }

}