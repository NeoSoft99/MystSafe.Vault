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

using MystSafe.Client.Base;
using MystSafe.Client.Engine;
using IndexedDB.Blazor;


namespace MystSafe.Client.App;

public class ChatInRecord
{
    [System.ComponentModel.DataAnnotations.Key]
    public string Id { get; set; }

    public string ContactId { get; set; }

    public string BlockHash { get; set; }

    public string ChatPubKey { get; set; }

    public string Height { get; set; }

    public string MessageData { get; set; }

    public string TimeStamp { get; set; }

    public static ChatInRecord ChatInToRecord(ChatIn chat, LocalStorageEncryptionService local_storage_encryptor)
    {
        var chat_record = new ChatInRecord();
        chat_record.Id = chat.Id;
        chat_record.ContactId = chat.Contact.Id;
        chat_record.ChatPubKey = local_storage_encryptor.EncryptString(chat.Contact.Account.Id, chat.ChatPubKey,
            chat.Id + nameof(chat.ChatPubKey));

        chat_record.Height =
            local_storage_encryptor.EncryptInt(chat.Contact.Account.Id, chat.Height,
                chat.Id + nameof(chat.Height)); //chat.Height;
        chat_record.MessageData = local_storage_encryptor.EncryptString(chat.Contact.Account.Id,
            chat.MessageData.ToString(), chat.Id + nameof(chat.MessageData));
        chat_record.TimeStamp = local_storage_encryptor.EncryptLong(chat.Contact.Account.Id, chat.TimeStamp,
            chat.Id + nameof(chat.TimeStamp)); //chat.TimeStamp;
        chat_record.BlockHash = local_storage_encryptor.EncryptString(chat.Contact.Account.Id, chat.BlockHash,
            chat.Id + nameof(chat.BlockHash));
        return chat_record;
    }

    public static ChatIn RecordToChatIn(string account_id, ChatInRecord chat_record,
        LocalStorageEncryptionService local_storage_encryptor)
    {
        var chat = new ChatIn();
        chat.Id = chat_record.Id;


        chat.ChatPubKey =
            local_storage_encryptor.DecryptString(account_id, chat_record.ChatPubKey,
                chat.Id + nameof(chat.ChatPubKey));

        chat.Height =
            local_storage_encryptor.DecryptInt(account_id, chat_record.Height,
                chat.Id + nameof(chat.Height)); //chat_record.Height;
        chat.MessageData = new MsgBlockData(local_storage_encryptor.DecryptString(account_id, chat_record.MessageData,
            chat.Id + nameof(chat.MessageData)));
        chat.TimeStamp =
            local_storage_encryptor.DecryptLong(account_id, chat_record.TimeStamp,
                chat.Id + nameof(chat.TimeStamp)); //chat_record.TimeStamp;
        chat.BlockHash =
            local_storage_encryptor.DecryptString(account_id, chat_record.BlockHash, chat.Id + nameof(chat.BlockHash));
        return chat;
    }

    public void Update(ChatInRecord new_record)
    {
        if (this.Id != new_record.Id)
            throw new Exception("Chat Id does not match");

        this.ContactId = new_record.ContactId;
        this.ChatPubKey = new_record.ChatPubKey;

        this.Height = new_record.Height;
        this.MessageData = new_record.MessageData;
        this.TimeStamp = new_record.TimeStamp;
        this.BlockHash = new_record.BlockHash;
    }
}


public class ChatOutRecord : ChatInRecord
{
    public string ChatKey { get; set; }

    //public string MsgKey { get; set; }
    public string LastOutMessageHash { get; set; }

    public static ChatOutRecord ChatOutToRecord(ChatOut chat, LocalStorageEncryptionService local_storage_encryptor)
    {
        var chat_record = new ChatOutRecord();
        chat_record.Id = chat.Id;
        chat_record.ContactId = chat.Contact.Id;
        chat_record.ChatPubKey = local_storage_encryptor.EncryptString(chat.Contact.Account.Id, chat.ChatPubKey,
            chat.Id + nameof(chat.ChatPubKey));

        chat_record.Height =
            local_storage_encryptor.EncryptInt(chat.Contact.Account.Id, chat.Height,
                chat.Id + nameof(chat.Height)); //chat.Height;
        chat_record.MessageData = local_storage_encryptor.EncryptString(chat.Contact.Account.Id,
            chat.MessageData.ToString(), chat.Id + nameof(chat.MessageData));
        chat_record.TimeStamp = local_storage_encryptor.EncryptLong(chat.Contact.Account.Id, chat.TimeStamp,
            chat.Id + nameof(chat.TimeStamp)); //chat.TimeStamp;
        chat_record.ChatKey =
            local_storage_encryptor.EncryptString(chat.Contact.Account.Id, chat.ChatKey,
                chat.Id + nameof(chat.ChatKey));

        chat_record.LastOutMessageHash = local_storage_encryptor.EncryptString(chat.Contact.Account.Id,
            chat.LastOutMessageHash, chat.Id + nameof(chat.LastOutMessageHash));
        chat_record.BlockHash = local_storage_encryptor.EncryptString(chat.Contact.Account.Id, chat.BlockHash,
            chat.Id + nameof(chat.BlockHash));
        return chat_record;
    }

    public static ChatOut RecordToChatOut(string account_id, ChatOutRecord chat_record,
        LocalStorageEncryptionService local_storage_encryptor)
    {
        var chat = new ChatOut();
        chat.Id = chat_record.Id;


        chat.ChatPubKey =
            local_storage_encryptor.DecryptString(account_id, chat_record.ChatPubKey,
                chat.Id + nameof(chat.ChatPubKey));

        chat.Height =
            local_storage_encryptor.DecryptInt(account_id, chat_record.Height,
                chat.Id + nameof(chat.Height)); //chat_record.Height;
        chat.TimeStamp =
            local_storage_encryptor.DecryptLong(account_id, chat_record.TimeStamp,
                chat.Id + nameof(chat.TimeStamp)); //chat_record.TimeStamp;
        chat.ChatKey =
            local_storage_encryptor.DecryptString(account_id, chat_record.ChatKey, chat.Id + nameof(chat.ChatKey));

        chat.LastOutMessageHash = local_storage_encryptor.DecryptString(account_id, chat_record.LastOutMessageHash,
            chat.Id + nameof(chat.LastOutMessageHash));
        chat.MessageData = new MsgBlockData(local_storage_encryptor.DecryptString(account_id, chat_record.MessageData,
            chat.Id + nameof(chat.MessageData)));
        //chat.TimeStamp = chat_record.TimeStamp;
        chat.BlockHash =
            local_storage_encryptor.DecryptString(account_id, chat_record.BlockHash, chat.Id + nameof(chat.BlockHash));
        return chat;
    }

    public void Update(ChatOutRecord new_record)
    {
        if (this.Id != new_record.Id)
            throw new Exception("Chat Id does not match");

        this.ContactId = new_record.ContactId;
        this.ChatPubKey = new_record.ChatPubKey;

        this.Height = new_record.Height;
        this.MessageData = new_record.MessageData;
        this.TimeStamp = new_record.TimeStamp;

        this.ChatKey = new_record.ChatKey;

        this.LastOutMessageHash = new_record.LastOutMessageHash;
        this.BlockHash = new_record.BlockHash;
    }
}


public class ChatInIndexedDb : IChatInDB
{
    protected readonly IIndexedDbFactory _DbFactory;
    private readonly LocalStorageEncryptionService _local_storage_encryptor;

    public ChatInIndexedDb(IIndexedDbFactory DbFactory, LocalStorageEncryptionService local_storage_encryptor)
    {
        _DbFactory = DbFactory;
        _local_storage_encryptor = local_storage_encryptor;
    }

    /// <summary>
    /// Store a new chat
    /// </summary>
    public async Task Add(ChatIn chat)
    {
        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            db.ChatsIn.Add(ChatInRecord.ChatInToRecord(chat, _local_storage_encryptor));
            await db.SaveChanges();
        }

    }


    /// <summary>
    /// Retrive all the chats that belong to Contact 
    /// </summary>
    public async Task<List<ChatIn>> GetAll(string ContactId, string account_id)
    {
        var chat_list = new List<ChatIn>();
        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            var chat_records = db.ChatsIn.Where(x => x.ContactId == ContactId).ToList();
            if (chat_records != null && chat_records.Count() > 0)
            {
                foreach (var chat_record in chat_records)
                {
                    chat_list.Add(ChatInRecord.RecordToChatIn(account_id, chat_record, _local_storage_encryptor));
                }
            }
        }

        return chat_list;
    }


    /// <summary>
    /// update an Account 
    /// </summary>
    public async Task Update(ChatIn chat)
    {
        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            var chat_record = db.ChatsIn.SingleOrDefault(x => x.Id == chat.Id);
            if (chat_record != null)
            {
                var updated_chat_record = ChatInRecord.ChatInToRecord(chat, _local_storage_encryptor);
                chat_record.Update(updated_chat_record);
                await db.SaveChanges();
            }
        }
    }

    /// <summary>
    /// Delete all chats in that belong to Contact 
    /// </summary>
    public async Task DeleteAll(string ContactId, string account_id)
    {
        var chat_records = await GetAll(ContactId, account_id);
        foreach (var chat_record in chat_records)
        {
            await Delete(chat_record.Id, account_id);
        }
    }


    /// <summary>
    /// Delete chat  
    /// </summary>
    public async Task Delete(string chat_id, string account_id)
    {
        var msg_indexed_db = new MessageIndexedDb(_DbFactory, _local_storage_encryptor);
        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            var chat_record = db.ChatsIn.SingleOrDefault(x => x.Id == chat_id);
            if (chat_record != null)
            {
                var result = db.ChatsIn.Remove(chat_record);
                await db.SaveChanges();
                await msg_indexed_db.DeleteAll(chat_id, account_id);
            }
        }
    }
}

public class ChatOutIndexedDb : IChatOutDB
{
    protected readonly IIndexedDbFactory _DbFactory;
    private readonly LocalStorageEncryptionService _local_storage_encryptor;

    public ChatOutIndexedDb(IIndexedDbFactory DbFactory, LocalStorageEncryptionService local_storage_encryptor)
    {
        _DbFactory = DbFactory;
        _local_storage_encryptor = local_storage_encryptor;
    }

    /// <summary>
    /// Store a new chat
    /// </summary>
    public async Task Add(ChatOut chat)
    {
        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            db.ChatsOut.Add(ChatOutRecord.ChatOutToRecord(chat, _local_storage_encryptor));
            await db.SaveChanges();
        }

    }


    /// <summary>
    /// Retrive all the chats that belong to Contact 
    /// </summary>
    public async Task<List<ChatOut>> GetAll(string ContactId, string account_id)
    {
        var chat_list = new List<ChatOut>();
        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            var chat_records = db.ChatsOut.Where(x => x.ContactId == ContactId).ToList();
            if (chat_records != null && chat_records.Count() > 0)
            {
                foreach (var chat_record in chat_records)
                {
                    chat_list.Add(ChatOutRecord.RecordToChatOut(account_id, chat_record, _local_storage_encryptor));
                }
            }
        }

        return chat_list;
    }


    /// <summary>
    /// update or insert a new one if it does nor exists 
    /// </summary>
    public async Task Update(ChatOut chat)
    {
        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            var chat_record = db.ChatsOut.SingleOrDefault(x => x.Id == chat.Id);

            if (chat_record != null)
            {
                // Update existing record
                var updated_chat_record = ChatOutRecord.ChatOutToRecord(chat, _local_storage_encryptor);
                chat_record.Update(updated_chat_record);
            }
            else
            {
                // Insert new record
                var new_chat_record = ChatOutRecord.ChatOutToRecord(chat, _local_storage_encryptor);
                db.ChatsOut.Add(new_chat_record);
            }

            await db.SaveChanges();
        }
    }


    /// <summary>
    /// Delete all chats in that belong to Contact 
    /// </summary>
    public async Task DeleteAll(string ContactId, string account_id)
    {
        var chat_records = await GetAll(ContactId, account_id);
        foreach (var chat_record in chat_records)
        {
            await Delete(chat_record.Id, account_id);
        }
    }


    /// <summary>
    /// Delete chat  
    /// </summary>
    public async Task Delete(string chat_id, string account_id)
    {
        var msg_indexed_db = new MessageIndexedDb(_DbFactory, _local_storage_encryptor);
        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            var chat_record = db.ChatsOut.SingleOrDefault(x => x.Id == chat_id);
            if (chat_record != null)
            {
                var result = db.ChatsOut.Remove(chat_record);
                await db.SaveChanges();
                await msg_indexed_db.DeleteAll(chat_id, account_id);
            }
        }
    }
}

