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

namespace MystSafe.Shared.Crypto;

    // 2. Sender: Generating One-time “Stealth” Address of the Recipient: 
    public class StealthAddress
    {
        //private PubKey _scanPubKey;
        //private Key _chatKey;
        private string _stealthAddress;

        private StealthAddress()
        {
            
        }

        public override string ToString()
        {
            return _stealthAddress;
        }

        public static StealthAddress GenerateNew(string remoteScanPubKeyBase58, SecKey localPrivateKey, string salt)
        {
            var stealthAddress = new StealthAddress();

            var shared_secret = DiffieHellman.GetSharedSecret(localPrivateKey.ToBase58(), remoteScanPubKeyBase58);
            stealthAddress._stealthAddress = KDF.GetStealthKey(shared_secret, salt);
            
            return stealthAddress;
        }

        public static StealthAddress Restore(string ScanKey, string ChatPubKey, string salt)
        {
            var stealthAddress = new StealthAddress();
            var shared_secret = DiffieHellman.GetSharedSecret(ScanKey, ChatPubKey);
            stealthAddress._stealthAddress = KDF.GetStealthKey(shared_secret, salt);
            return stealthAddress;
        }
        
        public bool IsMatch(string MessageStealthAddress)
        {
            // Note: this instance needs to be restored first by calling Restore() method
            return (this._stealthAddress == MessageStealthAddress);
        }
    }


