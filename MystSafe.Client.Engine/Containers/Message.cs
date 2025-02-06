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

namespace MystSafe.Client.Engine;

public enum MessageDirections : int
{
    Outgoing = 0,
    Incoming = 1
}

public class Message : IComparable<Message>
{
    //public const string MSG_TYPE_TEXT = "text";
    //public const string MSG_TYPE_SECRET = "secret";

    // this is just a unique ID for the local client DB
    public string Id { get; set; }

    //// required to find all chats in the local client DB that belong to particular account/address
    //public string ChatId { get; set; }

    // wheather it is incoming (=true) or outgoing (=false) message
    //public MessageDirections Direction { get; set; }
    public readonly MessageDirections Direction;

    public int Height { get; set; }

    public string Hash { get; set; }

    public readonly MsgBlockData MessageData;

    public readonly SecretBlockData SecretData;

    //public DateTime TimeStamp { get; set; }
    public long TimeStamp { get; set; }

    public ChatIn Chat { get; set; }



    //private readonly ILogger _logger;

    public Message(MessageDirections direction, MsgBlockData msgBlockData)
    {
        Direction = direction;
        MessageData = msgBlockData;
        if (MessageData.MessageType == MessageTypes.SECRET)
        {
            //ClientSideLogger.Logger.LogInformation("msgBlockData: " + MessageData.MsgText);
            SecretData = new SecretBlockData(MessageData.MsgText);
        }
        //_logger = logger;
    }

    //public Message()
    //{

    //}

    public int CompareTo(Message? other)
    {
        if (other == null)
            return 1;
        return TimeStamp.CompareTo(other.TimeStamp);
    }

    public static Message? GenerateFromChatOut(ChatOut chat)
    {
        if (string.IsNullOrEmpty(chat.MessageData.MsgText))
            return null;

        var message = new Message(
            MessageDirections.Outgoing,
            chat.MessageData);
        message.Id = chat.Id;
        message.Chat = chat;


        message.TimeStamp = chat.TimeStamp;

        message.Height = 0;
        message.Hash = chat.BlockHash; //string.Empty;

        return message;
    }

    public static Message? GenerateFromChatIn(ChatIn chat)
    {
        if (string.IsNullOrEmpty(chat.MessageData.MsgText))
            return null;

        var message = new Message(
            MessageDirections.Incoming,
            chat.MessageData);
        message.Id = chat.Id;
        message.Chat = chat;

        message.TimeStamp = chat.TimeStamp;

        message.Height = 0;
        message.Hash = chat.BlockHash; //string.Empty;
        return message;
    }
}

