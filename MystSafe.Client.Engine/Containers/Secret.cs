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
using MystSafe.Shared.Common;


namespace MystSafe.Client.Engine;

public class Secret 
{
    // this is just a unique key for the local client DB
    // 
    public string Id;

    // This is the id of the secret to maintain between verious versions
    // of Secret blocks; it is invisible to public but stored locally in the client DB. 
    //public string SecretId { get { return Data.SecretId;  } }

    //public string Id { get { return Data.SecretId; } }


    // the block PUBLIC key is the permanent id of the secret, it will secretly link between different versions of secret blocks
    public string BlockPubKey { get; set; }

    // this is the public key
    public string Group { get; set; }

    public int Height { get; set; }

    public string BlockHash { get; set; }

    public string PrevHash { get; set; }

    public SecretBlockData Data { get; set; }

    public long TimeStamp { get; set; }

    public int Expiration { get; set; }

    public int LicenseType { get; set; }
    
    public string LicensePubKey { get; set; }  

    public int DataSize { get; set; }

    public string GetInstantShareLink(string baseUri)
    {
        return string.Format(baseUri + "instantshare#{0}-{1}", BlockHash, Data.InstantKey);
    }

    //public string GetInstantShareEmail(string baseUri)
    //{
    //    var link = GetInstantShareLink(baseUri);

    //}

    public Account Account { get; set; }

    public bool FolderExpanded { get; set; }

    public List<Secret> FolderSecrets(List<Secret> all_secrets)
    {

        return all_secrets.Where(secret => secret.Data.SecretType != SecretTypes.Folder && secret.Data.FolderId == this.Data.FolderId).ToList();

    }

    public int FolderSecretsCount(List<Secret> all_secrets)
    {
        return all_secrets.Count(secret => secret.Data.SecretType != SecretTypes.Folder && secret.Data.FolderId == this.Data.FolderId);
    }

    private Secret()
    {
    }

    public static Secret FromRecord(string id)
    {
        var secret = new Secret();
        secret.Id = id;
        return secret;
    }

    //public static Secret FromBlock(SecretBlock block, SecretBlockData blockData)
    public static Secret FromBlock(SecretBlockValidator block, SecretBlockData blockData)
    {
        var secret = new Secret();
        secret.BlockPubKey = block.BlockPubKey;
        secret.BlockHash = block.Hash;
        secret.TimeStamp = block.TimeStamp;
        secret.Data = blockData;
        secret.PrevHash = block.PrevHash;
        secret.Group = block.Group;
        secret.Expiration = block.Expiration;
        secret.LicenseType = block.LicenseType;
        secret.LicensePubKey = block.License;
        secret.DataSize = block.DataSizeInBytes(); //CalculateBufferSizeInBytes(block.SecretData);
        return secret;
    }

    // returns size in bytes
    /*protected static int CalculateBufferSizeInBytes(string base64String)
    {
        if (string.IsNullOrEmpty(base64String))
        {
            return 0;
        }

        // Calculate the effective length of the Base64 string without padding
        int effectiveLength = base64String.TrimEnd('=').Length;

        // Calculate the size of the original data
        int dataSize = (effectiveLength * 3) / 4;

        return dataSize;
    }*/

    public string Export()
    {
        var result = new StringBuilder();

        result.AppendLine("TimeStamp: " + UnixDateTime.ToDateTime(this.TimeStamp));

        if (Data is not null)
            result.AppendLine(Data.Export());

        return result.ToString();
    }

 
}

