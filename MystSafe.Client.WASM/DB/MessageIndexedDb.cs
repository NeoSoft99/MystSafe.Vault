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

public class MessageRecord
{
    [System.ComponentModel.DataAnnotations.Key]
    public string Id { get; set; }

    public string ChatId { get; set; }
    public string Height { get; set; }
    public string Direction { get; set; }

    public long TimeStamp { get; set; }

    public string Hash { get; set; }

    public string MessageData { get; set; }

    public static MessageRecord MessageToRecord(Message message, LocalStorageEncryptionService local_storage_encryptor)
    {
        var message_record = new MessageRecord();
        message_record.Id = message.Id;
        message_record.ChatId = message.Chat.Id;
        message_record.Height = local_storage_encryptor.EncryptInt(message.Chat.Contact.Account.Id, message.Height,
            message.Id + nameof(message.Height)); //message.Height;
        message_record.Hash =
            message.Hash; //local_storage_encryptor.EncryptString(message.Chat.Contact.Account.Id, message.Hash, message.Id + nameof(message.Hash));
        message_record.MessageData = local_storage_encryptor.EncryptString(message.Chat.Contact.Account.Id,
            message.MessageData.ToString(), message.Id + nameof(message.MessageData));
        message_record.TimeStamp = message.TimeStamp;
        message_record.Direction = local_storage_encryptor.EncryptInt(message.Chat.Contact.Account.Id,
            (int)message.Direction, message.Id + nameof(message.Direction)); //(byte)message.Direction;
        return message_record;
    }

    public static Message RecordToMessage(string account_id, MessageRecord message_record,
        LocalStorageEncryptionService local_storage_encryptor)
    {
        var message = new Message(
            (MessageDirections)local_storage_encryptor.DecryptInt(account_id, message_record.Direction,
                message_record.Id + nameof(message_record.Direction)),
            new MsgBlockData(local_storage_encryptor.DecryptString(account_id, message_record.MessageData,
                message_record.Id + nameof(message_record.MessageData))));

        message.Id = message_record.Id;
        message.Hash =
            message_record
                .Hash; //local_storage_encryptor.DecryptString(account_id, message_record.Hash, message_record.Id + nameof(message_record.Hash));
        message.Height = local_storage_encryptor.DecryptInt(account_id, message_record.Height,
            message_record.Id + nameof(message_record.Height)); //message_record.Height;
        message.TimeStamp = message_record.TimeStamp;
        return message;
    }
}


public class MessageIndexedDb : IMessageDB
    {
        private readonly IIndexedDbFactory _DbFactory;
        private readonly LocalStorageEncryptionService _local_storage_encryptor;

        public MessageIndexedDb(IIndexedDbFactory DbFactory, LocalStorageEncryptionService local_storage_encryptor)
        {
            _DbFactory = DbFactory;
            _local_storage_encryptor = local_storage_encryptor;
        }

        /// <summary>
        /// Store a new chat
        /// </summary>
        public async Task Add(Message message)
        {
            using (var db = await this._DbFactory.Create<ClientDb>())
            {
                db.Messages.Add(MessageRecord.MessageToRecord(message, _local_storage_encryptor));
                await db.SaveChanges();
            }

        }


        /// <summary>
        /// Retrive all the chats that belong to Chat 
        /// </summary>
        public async Task<List<Message>> GetAll(string ChatId, string account_id)
        {
            var message_list = new List<Message>();
            using (var db = await this._DbFactory.Create<ClientDb>())
            {
                var message_records = db.Messages.Where(x => x.ChatId == ChatId).OrderBy(x => x.TimeStamp).ToList();
                if (message_records != null && message_records.Count() > 0)
                {
                    foreach (var message_record in message_records)
                    {
                        var message = MessageRecord.RecordToMessage(account_id, message_record, _local_storage_encryptor);
                        message_list.Add(message);
                    }
                }
            }

            return message_list;
        }

        /// <summary>
        /// Delete all chats that belong to Account 
        /// </summary>
        public async Task DeleteAll(string ChatId, string account_id)
        {
            var message_records = await GetAll(ChatId, account_id);
            foreach (var message_record in message_records)
            {
                await Delete(message_record.Id);
            }
        }


        /// <summary>
        /// Delete  
        /// </summary>
        public async Task Delete(string Id)
        {
            using (var db = await this._DbFactory.Create<ClientDb>())
            {
                var message_record = db.Messages.SingleOrDefault(x => x.Id == Id);
                if (message_record != null)
                {
                    var result = db.Messages.Remove(message_record);
                    await db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Delete  
        /// </summary>
        public async Task DeleteByHash(string hash)
        {
            using (var db = await this._DbFactory.Create<ClientDb>())
            {
                var message_record = db.Messages.SingleOrDefault(x => x.Hash == hash);
                if (message_record != null)
                {
                    var result = db.Messages.Remove(message_record);
                    await db.SaveChanges();
                }
            }
        }
    }


