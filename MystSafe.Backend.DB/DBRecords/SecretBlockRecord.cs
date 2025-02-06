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

using MongoDB.Bson.Serialization.Attributes;
using MystSafe.Shared.Common;

namespace MystSafe.Backend.DB;

// Class to store the block in MongoDB
public class SecretBlockRecord
{
    public bool DeleteFlag;
    public string DeletionHash;
    public int Difficulty;
    public int Expiration;
    public string ExpiredSig;
    public string Group;
    public string GroupSignature;

    [BsonId] public string Hash;

    public int Height;
    public string License;
    public int LicenseType;
    public int Network;
    public int Nonce;
    public string PrevHash;
    public string PubKey;
    public string Reserved;
    public string SecretData;
    public string SecretValue;
    public string Signature;
    public string StealthAddress;
    public long TimeStamp;
    public int Version;

    private SecretBlockRecord()
    {
    }

    public SecretBlock ToProto()
    {
        var proto = new SecretBlock();
        proto.Hash = Hash;
        proto.TimeStamp = TimeStamp;
        proto.Height = Height;
        proto.StealthAddress = StealthAddress;
        proto.PubKey = PubKey;
        proto.PrevHash = PrevHash;
        proto.SecretData = SecretData;
        proto.SecretGroup = Group;
        proto.SecretGroupSignature = GroupSignature;
        proto.SecretValue = SecretValue;
        proto.Version = Version;
        proto.Difficulty = Difficulty;
        proto.Nonce = Nonce;
        proto.DeleteFlag = DeleteFlag;
        proto.Signature = Signature;
        proto.License = License;
        proto.LicenseType = LicenseType;
        proto.DeletionHash = DeletionHash;
        proto.Expiration = Expiration;
        proto.ExpiredSig = ExpiredSig;
        proto.Network = Network;
        proto.Reserved = Reserved;
        return proto;
    }

    public static SecretBlockRecord FromProto(SecretBlock proto)
    {
        var data = new SecretBlockRecord();
        data.Hash = proto.Hash;
        data.TimeStamp = proto.TimeStamp;
        data.Height = proto.Height;
        data.StealthAddress = proto.StealthAddress;
        data.PubKey = proto.PubKey;
        data.PrevHash = proto.PrevHash;
        data.SecretData = proto.SecretData;
        data.Group = proto.SecretGroup;
        data.GroupSignature = proto.SecretGroupSignature;
        data.SecretValue = proto.SecretValue;
        data.Version = proto.Version;
        data.Difficulty = proto.Difficulty;
        data.Nonce = proto.Nonce;
        data.DeleteFlag = proto.DeleteFlag;
        data.Signature = proto.Signature;
        data.License = proto.License;
        data.LicenseType = proto.LicenseType;
        data.DeletionHash = proto.DeletionHash;
        data.Expiration = proto.Expiration;
        data.ExpiredSig = proto.ExpiredSig;
        data.Network = proto.Network;
        data.Reserved = proto.Reserved;
        return data;
    }
}