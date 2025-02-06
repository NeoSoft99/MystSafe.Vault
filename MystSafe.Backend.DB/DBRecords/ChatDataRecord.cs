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

/*
    MystSafe is a secret vault with anonymous access and zero activity tracking protected by cryptocurrency-grade tech.

    Copyright (C) 2024-2025 MystSafe, NeoSoft99

    MystSafe: The Only Privacy-Preserving Password Manager.
    https://mystsafe.com

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
    See the GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using MongoDB.Bson.Serialization.Attributes;
using MystSafe.Shared.Common;

namespace MystSafe.Backend.DB;

// Class to store the block in MongoDB
public class ChatDataRecord
{
    public int ChatExpirationDays;
    public string ChatPubkey;
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
    public string RecipientStealthAddress;
    public string Reserved;
    public string SenderStealthAddress;
    public string Signature;
    public long TimeStamp;
    public int Version;

    private ChatDataRecord()
    {
    }

    public InitBlock ToProto()
    {
        var chat_proto = new InitBlock();
        chat_proto.Hash = Hash;
        chat_proto.TimeStamp = TimeStamp;
        chat_proto.Height = Height;
        chat_proto.PrevHash = PrevHash;
        chat_proto.ChatPubkey = ChatPubkey;
        chat_proto.ChatExpirationDays = ChatExpirationDays;
        chat_proto.RecipientStealthAddress = RecipientStealthAddress;
        chat_proto.SenderStealthAddress = SenderStealthAddress;
        chat_proto.InitData = InitData;
        chat_proto.MessageData = MessageData;
        chat_proto.Version = Version;
        chat_proto.Difficulty = Difficulty;
        chat_proto.Nonce = Nonce;
        chat_proto.Signature = Signature;
        chat_proto.License = License;
        chat_proto.LicenseType = LicenseType;
        chat_proto.Network = Network;
        chat_proto.Reserved = Reserved;
        return chat_proto;
    }

    public static ChatDataRecord FromProto(InitBlock contact_proto)
    {
        var chat_data = new ChatDataRecord();
        chat_data.Hash = contact_proto.Hash;
        chat_data.TimeStamp = contact_proto.TimeStamp;
        chat_data.Height = contact_proto.Height;
        chat_data.PrevHash = contact_proto.PrevHash;
        chat_data.ChatPubkey = contact_proto.ChatPubkey;
        chat_data.ChatExpirationDays = contact_proto.ChatExpirationDays;
        chat_data.RecipientStealthAddress = contact_proto.RecipientStealthAddress;
        chat_data.SenderStealthAddress = contact_proto.SenderStealthAddress;
        chat_data.InitData = contact_proto.InitData;
        chat_data.MessageData = contact_proto.MessageData;
        chat_data.Version = contact_proto.Version;
        chat_data.Difficulty = contact_proto.Difficulty;
        chat_data.Nonce = contact_proto.Nonce;
        chat_data.Signature = contact_proto.Signature;
        chat_data.License = contact_proto.License;
        chat_data.LicenseType = contact_proto.LicenseType;
        chat_data.Network = contact_proto.Network;
        chat_data.Reserved = contact_proto.Reserved;
        return chat_data;
    }
}