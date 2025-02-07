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

using System.Diagnostics;
using MoneroRing.Crypto;
using MoneroSharp;
using System.Runtime.CompilerServices;
using MoneroSharp.Utils;
//using NBitcoin;

[assembly: InternalsVisibleTo("MystSafe.Shared.Tests")]

namespace MystSafe.Shared.Crypto;
   
    // 1. Both Recipient and Sender: Generating Account Address and providing to the sender(offline - by email, text, or another chat)
    public class UserAddress : BasePrivateAddress
    {
        private readonly NBitcoin.Mnemonic _mnemonic12;
        private readonly string _mnemonic12String;
        private string _mnemonic25String;

        private SecKey _hiddenScanKey;

        private PubKey _hiddenScanPubKey;
        
        public NBitcoin.Mnemonic Mnemonic12
        {
            get { return _mnemonic12; }
        }

        public string Mnemonic12String
        {
            get { return _mnemonic12String; }
        }

        public string Mnemonic25String
        {
            get { return _mnemonic25String; }
        }

        public PubKey HiddenScanPubKey
        {
            get { return _hiddenScanPubKey; }
        }
        
        public SecKey HiddenScanKey
        {
            get
            {
                return _hiddenScanKey; //Encoders.Base58.EncodeData(_scanKey.SecKey.ToBytes());
            }
        }
        
     
        private UserAddress(string mnemonic12String, NBitcoin.Mnemonic mnemonic, MoneroNetwork network) : base(network)
        {
            _mnemonic12 = mnemonic;
            _mnemonic12String = mnemonic12String;
        }

        // from mnemonic
        public static UserAddress GenerateFromMnemonic(int network)
        {
            bool check_result = false;
            UserAddress address = null;
            while (!check_result)
            {
                var mnemonic = new NBitcoin.Mnemonic(NBitcoin.Wordlist.English, NBitcoin.WordCount.Twelve);
                var mnemonic_string = mnemonic.ToString();
                address = new UserAddress(mnemonic_string, mnemonic, ConvertMystSafeNetworkToMonero(network));
                check_result = address.SetAddressFromMnemonic12();
            }

            return address;
        }

        // this is when restoring from database
        public static UserAddress RestoreFromMnemonic(string mnemonic_string, int network)
        {
            var mnemonic = new NBitcoin.Mnemonic(mnemonic_string);
            var address = new UserAddress(mnemonic_string, mnemonic, ConvertMystSafeNetworkToMonero(network));
            var check_result = address.SetAddressFromMnemonic12();
            if (!check_result)
                throw new ApplicationException("This 12-word mnemonic phrase is not suitable for Monero mnemonc seed");
            return address;
        }

        // private to avoid mistake and use static creator methods
        internal bool SetAddressFromMnemonic12()
        {
            try
            {
                // Convert the 12-words mnemonic to a seed
                byte[] seed64 = _mnemonic12.DeriveSeed();

                // Ensure the seed is generated as expected by Monero
                var check_result = RingSig.generate_mnemonic_seed(seed64, out byte[] initial_seed32);
                if (!check_result)
                    return check_result;

                // // EncodeMnemonics will reverse the seed bytes so make sure we preserve the original seed to derive the keys
                _seed32 = new byte[32];
                Array.Copy(initial_seed32, _seed32, 32);
                if (Debugger.IsAttached)
                {
                    Console.WriteLine("_seed32: " + MoneroUtils.BytesToHex(_seed32));
                }

                string[] words25 = MoneroAccount.EncodeMnemonics(_seed32, MoneroSharp.WordList.Languages.English);
                _mnemonic25String = string.Join(" ", words25);

                DeriveKeys();
                SetAddressFromPublicKeys(_scanPubKey, _readPubKey);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        
       
        internal void DeriveKeys()
        {
            var keccak256 = new Nethereum.Util.Sha3Keccack();
            var hash1 = keccak256.CalculateHash(_seed32);
            byte[] seed2 = _seed32.Concat(hash1).ToArray();
            var hash2 = keccak256.CalculateHash(seed2);

            _readKey = SecKey.FromSeed(_seed32);
            _scanKey = SecKey.FromSeed(hash1);
            _hiddenScanKey = SecKey.FromSeed(hash2);

            _readPubKey = PubKey.FromPrivateKeyBytes(_readKey.ToBytes());
            _scanPubKey = PubKey.FromPrivateKeyBytes(_scanKey.ToBytes());
            _hiddenScanPubKey = PubKey.FromPrivateKeyBytes(_hiddenScanKey.ToBytes());
        }
    }

