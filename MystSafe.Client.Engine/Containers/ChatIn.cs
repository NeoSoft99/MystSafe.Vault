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

public class ChatIn
{
    // this is just a unique ID for the local client DB
    // 
    public string Id { get; set; }

    // the hash of the associated block - to avoid duplicate processing
    public string BlockHash { set; get; }

    public string ChatPubKey { get; set; }

    public int Height { get; set; }

    //public DateTime TimeStamp { get; set; }
    public long TimeStamp { get; set; }

    //public string MessageText { get; set; }
    public MsgBlockData MessageData { get; set; }

    public Contact Contact { get; set; }

    public readonly List<Message> Messages = new List<Message>();

    public void AddMessage(Message message)
    {
        Messages.Add(message);
        message.Chat = this;
    }

    public bool RemoveMessage(string message_id)
    {
        int indexToRemove = Messages.FindIndex(message => message.Id == message_id);
        if (indexToRemove != -1)
        {
            Messages.RemoveAt(indexToRemove);
            return true;
        }

        return false;
    }

    public bool RemoveMessageByHash(string hash)
    {
        int indexToRemove = Messages.FindIndex(message => message.Hash == hash);
        if (indexToRemove != -1)
        {
            Messages.RemoveAt(indexToRemove);
            return true;
        }

        return false;
    }

    public void ClearMessages()
    {
        Messages.Clear();
    }

    public ChatIn()
    {

    }
}

    


