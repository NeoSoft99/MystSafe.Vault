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
using MystSafe.Shared.Common;
using MystSafe.Client.Base;

namespace MystSafe.Client.Engine;

public class SecretBlockDecoder : SecretBlockValidator, IBlockDecoder<SecretBlockData>
{
    //private readonly ILogger<SendProcessor> _logger;

    public SecretBlockDecoder(SecretBlock block) : base(block)
    {
        //_logger = logger;
    }

    public SecretBlockData Decode(SecKey readKey)
    {
        var msg_data_decrypted = DecryptBlockData(readKey);
        //_logger.LogInformation("msg_data_decrypted: " + msg_data_decrypted);
        return new SecretBlockData(msg_data_decrypted);
    }

    public SecretBlockData DecodeInstantShareData(byte[] endcryptionKey)
    {
        var salt = CalculateBlockSalt();
        var msg_data_decrypted = AES.Decrypt(endcryptionKey, salt, SecretValue);
        return new SecretBlockData(msg_data_decrypted);
    }

    //public override void Validate()
    //{
    //    if (!VerifyHash())
    //        throw new ApplicationException("Hash validation failed.");

    //    if (!VerifyBlockSignature())
    //        throw new ApplicationException("Signature validation failed.");

    //    var retention_interval = UnixTimeInterval.FromRetentionInterval(Network, typeof(SecretBlock));
    //    var deletion_threshold = UnixDateTime.DeletionThreshold(retention_interval);
    //    if (TimeStamp < deletion_threshold && !ProLicense)
    //        throw new BlockExpiredWithNoLicenseException();
    //}
}

public class InstantShareLinkInfo
{
    public string? BlockHash { get; set; }

    public byte[]? EncryptionKeyBytes { get; set; }

    public InstantShareLinkInfo(string instant_link)
    {
        var uri = new Uri(instant_link);
        var fragment = uri.Fragment;

        // Process the fragment
        if (!string.IsNullOrEmpty(fragment))
        {
            // Assuming the fragment is in the format "#hash-secretKey"
            fragment = fragment.TrimStart('#'); // Remove the "#" at the start
            var parts = fragment.Split('-');
            if (parts.Length == 2)
            {
                BlockHash = parts[0];
                var EncryptionKeyBase58 = parts[1];
                EncryptionKeyBytes = !string.IsNullOrEmpty(EncryptionKeyBase58)
                    ? Codecs.FromBase58ToBytes(EncryptionKeyBase58)
                    : null;
            }
        }
    }
}

   



