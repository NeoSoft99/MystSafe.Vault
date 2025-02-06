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

namespace MystSafe.Shared.Crypto;

    // 1. Both Recipient and Sender: Generating Account Address and providing to the sender(offline - by email, text, or another chat)
    public abstract class BasePrivateAddress: PublicAddress
    {
        protected byte[] _seed32;
        
        protected SecKey _scanKey;
        protected SecKey _readKey;
        
        public SecKey ScanKey
        {
            get
            {
                return _scanKey; 
            }
        }
        
        public SecKey ReadKey
        {
            get
            {
                return _readKey; 
            }
        }
        
        internal string Seed32Hex
        {
            get { return Codecs.FromBytesToHex(_seed32); }
        }
        
        protected BasePrivateAddress(MoneroNetwork network): base(network)
        {
        }
               
        protected void ComposeAddress(KeyPair readKeyPair, KeyPair scanKeyPair)
        {
            _scanKey = scanKeyPair.PrivateKey; //SecKey;
            _readKey = readKeyPair.PrivateKey; // SecKey;
            _scanPubKey = scanKeyPair.PublicKey; //PubKey;
            _readPubKey = readKeyPair.PublicKey; //PubKey;

            SetAddressFromPublicKeys(_scanPubKey, _readPubKey);
        }
        
       

    }

