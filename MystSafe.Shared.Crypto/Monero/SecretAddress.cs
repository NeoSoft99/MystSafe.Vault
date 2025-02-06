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

using MoneroSharp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;

namespace MystSafe.Shared.Crypto;
   
    // 1. Both Recipient and Sender: Generating Account Address and providing to the sender(offline - by email, text, or another chat)
    public class SecretAddress : BasePrivateAddress
    {
        protected SecretAddress(MoneroNetwork network) : base(network)
        {
        }

        // this is when a new contact request is sent out to a peer;
        // make it deterministically random using self private keys and peer's public keys as input.
        // NEW!
        public static SecretAddress GenerateFromPeerAddress(UserAddress accountAddress, string peerAddress, int network)
        {
            PublicAddress peer_address = RecreateFromAddressString(peerAddress, network);
            SecretAddress secret_address = new SecretAddress(ConvertMystSafeNetworkToMonero(network));
            
            var scanKeyPair = GenerateDeterministicallyRandomKeyPair(accountAddress.ReadKey.ToBytes(), peer_address.ScanPubKey.ToBytes());
            var readKeyPair = GenerateDeterministicallyRandomKeyPair(accountAddress.ReadKey.ToBytes(), peer_address.ReadPubKey.ToBytes());
            
            secret_address.ComposeAddress(readKeyPair, scanKeyPair);
            return secret_address;
        }

        // this is when restoring from database
        public static SecretAddress RestoreFromKeys(string readKey, string scanKey, int network)
        {
            var address = new SecretAddress(ConvertMystSafeNetworkToMonero(network));
            var scanKeyPair = KeyPair.FromPrivateKeyBase58(scanKey); //address.RestoreKeyPairFromPrivateKeyBase64(scanKey);
            var readKeyPair = KeyPair.FromPrivateKeyBase58(readKey); //address.RestoreKeyPairFromPrivateKeyBase64(readKey);

            address.ComposeAddress(readKeyPair, scanKeyPair);
            return address;
        }
        
        private static KeyPair GenerateDeterministicallyRandomKeyPair(byte[] seed1, byte[] seed2)
        {
            byte[] combined_seed = seed1.Concat(seed2).ToArray();
            var keccak256 = new Nethereum.Util.Sha3Keccack();
            var hash = keccak256.CalculateHash(combined_seed);
            SecKey newPrivateKey = SecKey.FromSeed(hash);
            return KeyPair.FromPrivateKeyBytes(newPrivateKey.ToBytes());
        }

        /*private KeyPair GenerateDeterministicallyRandomKeyPair(byte[] private_key, byte[] public_key)
        {
            // Define the hash function for HKDF (SHA-256 in this case)
            IDigest digest = new Sha256Digest();

            // Use the master private key as the HKDF "IKM" (input key material)
            var hkdfKeyGenerator = new HkdfBytesGenerator(digest);
            hkdfKeyGenerator.Init(new HkdfParameters(private_key, null, public_key));

            // Generate the new private key
            byte[] newPrivateKeyBytes = new byte[32];
            hkdfKeyGenerator.GenerateBytes(newPrivateKeyBytes, 0, newPrivateKeyBytes.Length);

            SecKey newPrivateKey = new SecKey(newPrivateKeyBytes);
            PubKey newPublicKey = PubKey.FromPrivateKeyBytes(newPrivateKeyBytes); // Derive the corresponding public key

            var keyPair = new KeyPair(newPrivateKey.ToBytes(), newPublicKey.ToBytes());
            return keyPair;
        }*/
    }

