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
using MoneroSharp;
using MoneroSharp.Utils;

namespace MystSafe.Shared.Crypto;

    // 1. Both Recipient and Sender: Generating Account Address and providing to the sender(offline - by email, text, or another chat)
    public class PublicAddress
    {
        protected readonly MoneroNetwork _network; // mystsafe network definied in Networks

        protected PubKey _scanPubKey;
        protected PubKey _readPubKey;
        protected string? _addressStr;

        protected PublicAddress(MoneroNetwork network)
        {
            _network = network;
        }

        public MoneroNetwork Network => _network;

        public override string ToString()
        {
            return _addressStr;
        }

        public string ToShortString()
        {
            return AddressShort(ToString());
        }

        public PubKey ScanPubKey => _scanPubKey;

        public PubKey ReadPubKey => _readPubKey;

        public static PublicAddress CreateFromPublicKeys(PubKey scanPubKey, PubKey readPubKey, int network)
        {
            var address = new PublicAddress(ConvertMystSafeNetworkToMonero(network));
            address.SetAddressFromPublicKeys(scanPubKey, readPubKey);
            return address;
        }

        protected void SetAddressFromPublicKeys(PubKey scanPubKey, PubKey readPubKey)
        {
            _scanPubKey = scanPubKey;
            _readPubKey = readPubKey;

            var readPubKeyHex = MoneroUtils.BytesToHex(readPubKey.ToBytes());
            var scanPubKeyHex = MoneroUtils.BytesToHex(scanPubKey.ToBytes());

            string pub_key = (int)_network + readPubKeyHex + scanPubKeyHex;
            var keccak256 = new Nethereum.Util.Sha3Keccack();
            string checksum = MoneroUtils.BytesToHex(keccak256.CalculateHash(MoneroUtils.HexBytesToBinary(pub_key)))
                .Substring(0, 8).ToString();
            string pub_address_hex = pub_key + checksum;
            byte[] pub_address_hex_bytes = MoneroUtils.HexBytesToBinary(pub_address_hex);
            var publicAddressBase58Bytes = Base58.Encode(pub_address_hex_bytes);
            var publicAddressBase58String = Encoding.ASCII.GetString(publicAddressBase58Bytes);

            //_address = publicAddressBase58Bytes;
            _addressStr = publicAddressBase58String;
        }

        public static PublicAddress RecreateFromAddressString(string address_string, int network)
        {
            var address = new PublicAddress(ConvertMystSafeNetworkToMonero(network));
            address.SetAddressFromAddressString(address_string);
            return address;
        }

        protected void SetAddressFromAddressString(string addressString)
        {
            var _address = Codecs.FromBase58ToBytes(addressString);

            // Verify checksum
            var keccak256 = new Nethereum.Util.Sha3Keccack();
            var pubKeyWithoutChecksum = _address.Take(_address.Length - 4).ToArray();
            var checksum = keccak256.CalculateHash(pubKeyWithoutChecksum);
            var addressChecksum = _address.Skip(_address.Length - 4).ToArray();

            if (!addressChecksum.SequenceEqual(checksum.Take(4)))
            {
                throw new Exception("Invalid address checksum");
            }

            // Remove the network byte and checksum
            var pubKeysWithNetwork = pubKeyWithoutChecksum.Skip(1).ToArray();
            var pubKeys = pubKeysWithNetwork.Take(64).ToArray();

            // Each key should be 32 bytes
            if (pubKeys.Length != 64)
            {
                throw new Exception("Invalid public key length");
            }

            // Split the keys byte array to get the bytes for each public key
            var readKeyBytes = pubKeys.Take(32).ToArray();
            var scanKeyBytes = pubKeys.Skip(32).ToArray();

            if (scanKeyBytes.Length != 32 || readKeyBytes.Length != 32)
            {
                throw new Exception("Invalid key length");
            }

            // Convert the public key bytes to public key objects
            _scanPubKey = new PubKey(scanKeyBytes);
            _readPubKey = new PubKey(readKeyBytes);
        }

        public static string AddressShort(string address_string)
        {
            if (string.IsNullOrWhiteSpace(address_string))
                return string.Empty;

            if (address_string.Length <= 8)
            {
                return address_string;
            }
            else
            {
                string leadingChars = address_string.Substring(0, 4);
                string endingChars = address_string.Substring(address_string.Length - 4);
                return $"{leadingChars}....{endingChars}";
            }
        }

        public static MoneroNetwork ConvertMystSafeNetworkToMonero(int mystsafe_network)
        {
            if (mystsafe_network == Networks.mainnet || mystsafe_network == Networks.custom)
                return MoneroNetwork.MAINNET;
            return MoneroNetwork.TESTNET;
        }
    }

