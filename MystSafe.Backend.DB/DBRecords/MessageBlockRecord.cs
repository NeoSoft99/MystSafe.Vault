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
public class MessageBlockRecord
{
    public string ChatPubKey;
    public bool DeleteFlag;
    public string DeletionHash;
    public int Difficulty;
    public int Expiration;
    public string ExpiredSig;

    [BsonId] public string Hash;

    public int Height;
    public string MsgData;
    public int Network;
    public int Nonce;
    public string PrevHash;
    public string Reserved;
    public string Signature;
    public long TimeStamp;
    public int Version;

    private MessageBlockRecord()
    {
    }

    public MsgBlock ToProto()
    {
        var message_proto = new MsgBlock();
        message_proto.Hash = Hash;
        message_proto.TimeStamp = TimeStamp;
        message_proto.Height = Height;
        message_proto.ChatPubKey = ChatPubKey;
        message_proto.PrevHash = PrevHash;
        message_proto.MessageData = MsgData;
        message_proto.Version = Version;
        message_proto.Difficulty = Difficulty;
        message_proto.Nonce = Nonce;
        message_proto.Signature = Signature;

        message_proto.DeleteFlag = DeleteFlag;
        message_proto.DeletionHash = DeletionHash;
        message_proto.Expiration = Expiration;
        message_proto.ExpiredSig = ExpiredSig;
        message_proto.Network = Network;
        message_proto.Reserved = Reserved;
        return message_proto;
    }

    public static MessageBlockRecord FromProto(MsgBlock proto)
    {
        var msg_data = new MessageBlockRecord();
        msg_data.Hash = proto.Hash;
        msg_data.TimeStamp = proto.TimeStamp;
        msg_data.Height = proto.Height;
        msg_data.ChatPubKey = proto.ChatPubKey;
        msg_data.PrevHash = proto.PrevHash;
        msg_data.MsgData = proto.MessageData;
        msg_data.Version = proto.Version;
        msg_data.Difficulty = proto.Difficulty;
        msg_data.Nonce = proto.Nonce;
        msg_data.Signature = proto.Signature;

        msg_data.DeleteFlag = proto.DeleteFlag;
        msg_data.DeletionHash = proto.DeletionHash;
        msg_data.Expiration = proto.Expiration;
        msg_data.ExpiredSig = proto.ExpiredSig;
        msg_data.Network = proto.Network;
        msg_data.Reserved = proto.Reserved;
        return msg_data;
    }
}