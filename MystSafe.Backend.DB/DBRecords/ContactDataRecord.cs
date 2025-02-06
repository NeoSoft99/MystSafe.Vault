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

// Class to stoe the bvlock in MongoDB
public class ContactDataRecord
{
    public bool DeleteFlag;
    public int Difficulty;

    [BsonId] public string Hash;

    public int Height;
    public string InitData;
    public string License;
    public int LicenseType;
    public string MessageData;
    public int Network;
    public int Nonce;
    public string PrevHash;
    public string PubKey;
    public string RecipientStealthAddress;
    public string Reserved;
    public string SenderStealthAddress;
    public string Signature;
    public long TimeStamp;
    public int Version;

    private ContactDataRecord()
    {
    }

    // for test only
    public ContactDataRecord(string ID)
    {
        Hash = ID;
    }

    public ContactBlock ToProto()
    {
        var contact_proto = new ContactBlock();
        contact_proto.Hash = Hash;
        contact_proto.TimeStamp = TimeStamp;
        contact_proto.Height = Height;
        contact_proto.PrevHash = PrevHash;

        contact_proto.PubKey = PubKey;
        contact_proto.RecipientStealthAddress = RecipientStealthAddress;
        contact_proto.SenderStealthAddress = SenderStealthAddress;
        contact_proto.InitData = InitData;
        contact_proto.MessageData = MessageData;
        contact_proto.Version = Version;
        contact_proto.Difficulty = Difficulty;
        contact_proto.Nonce = Nonce;
        contact_proto.DeleteFlag = DeleteFlag;
        contact_proto.Signature = Signature;
        contact_proto.License = License;
        contact_proto.LicenseType = LicenseType;
        contact_proto.Network = Network;
        contact_proto.Reserved = Reserved;
        return contact_proto;
    }

    public static ContactDataRecord FromProto(ContactBlock contact_proto)
    {
        var contact_data = new ContactDataRecord();
        contact_data.Hash = contact_proto.Hash;
        contact_data.TimeStamp = contact_proto.TimeStamp;
        contact_data.Height = contact_proto.Height;
        contact_data.PrevHash = contact_proto.PrevHash;
        contact_data.PubKey = contact_proto.PubKey;
        contact_data.RecipientStealthAddress = contact_proto.RecipientStealthAddress;
        contact_data.SenderStealthAddress = contact_proto.SenderStealthAddress;
        contact_data.InitData = contact_proto.InitData;
        contact_data.MessageData = contact_proto.MessageData;
        contact_data.Version = contact_proto.Version;
        contact_data.Difficulty = contact_proto.Difficulty;
        contact_data.Nonce = contact_proto.Nonce;
        contact_data.DeleteFlag = contact_proto.DeleteFlag;
        contact_data.Signature = contact_proto.Signature;
        contact_data.License = contact_proto.License;
        contact_data.LicenseType = contact_proto.LicenseType;
        contact_data.Network = contact_proto.Network;
        contact_data.Reserved = contact_proto.Reserved;
        return contact_data;
    }
}