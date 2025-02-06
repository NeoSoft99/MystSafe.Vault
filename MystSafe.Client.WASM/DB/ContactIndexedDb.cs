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

using IndexedDB.Blazor;
using MystSafe.Client.Engine;
using MystSafe.Client.Base;
using MystSafe.Shared.Crypto;

namespace MystSafe.Client.App;

public class ContactRecord
{
    [System.ComponentModel.DataAnnotations.Key]
    public string Id { get; set; }

    public string AccountId { get; set; }
    public string PeerUserAddress { get; set; }
    public string PeerSecretAddress { get; set; }
    public string PeerNickName { get; set; }
    public string SecretReadKey { get; set; }
    public string SecretScanKey { get; set; }
    public string ExpirationDays { get; set; }
    public string LastScannedChatBlock { get; set; }

    public string TimeStampOut { get; set; }

    public string TimeStampIn { get; set; }
    public string BlockPrivateKey { get; set; }

    public string Height { get; set; }
    public string BlockHash { get; set; }
    public string PrevHash { get; set; }
    public string BlockPublicKey { get; set; }
    public string LicenseType { get; set; }
    public string LicensePubKey { get; set; }
    public string Network { get; set; }

    public static ContactRecord ContactToRecord(Contact contact, LocalStorageEncryptionService local_storage_encryptor)
    {
        var contact_record = new ContactRecord();
        contact_record.Id = contact.Id;
        contact_record.AccountId = contact.Account.Id;
        contact_record.PeerUserAddress = local_storage_encryptor.EncryptString(contact.Account.Id,
            contact.PeerUserAddress, contact.Id + nameof(contact_record.PeerUserAddress));
        contact_record.PeerSecretAddress = local_storage_encryptor.EncryptString(contact.Account.Id,
            contact.PeerSecretAddress, contact.Id + nameof(contact_record.PeerSecretAddress));
        contact_record.PeerNickName = local_storage_encryptor.EncryptString(contact.Account.Id, contact.PeerNickName,
            contact.Id + nameof(contact_record.PeerNickName));
        contact_record.SecretReadKey = local_storage_encryptor.EncryptString(contact.Account.Id,
            contact.SenderSecretAddress.ReadKey.ToString(), contact.Id + nameof(contact_record.SecretReadKey));
        contact_record.SecretScanKey = local_storage_encryptor.EncryptString(contact.Account.Id,
            contact.SenderSecretAddress.ScanKey.ToString(), contact.Id + nameof(contact_record.SecretScanKey));
        contact_record.ExpirationDays = local_storage_encryptor.EncryptInt(contact.Account.Id, contact.ExpirationDays,
            contact.Id + nameof(contact_record.ExpirationDays));
        contact_record.LastScannedChatBlock = local_storage_encryptor.EncryptLong(contact.Account.Id,
            contact.LastScannedChatBlock, contact.Id + nameof(contact_record.LastScannedChatBlock));
        contact_record.TimeStampOut = local_storage_encryptor.EncryptLong(contact.Account.Id, contact.TimeStampOut,
            contact.Id + nameof(contact_record.TimeStampOut));
        contact_record.TimeStampIn = local_storage_encryptor.EncryptLong(contact.Account.Id, contact.TimeStampIn,
            contact.Id + nameof(contact_record.TimeStampIn));
        contact_record.BlockPrivateKey = local_storage_encryptor.EncryptString(contact.Account.Id,
            contact.BlockPrivateKey.ToString(), contact.Id + nameof(contact_record.BlockPrivateKey));
        contact_record.Height = local_storage_encryptor.EncryptInt(contact.Account.Id, contact.Height,
            contact.Id + nameof(contact_record.Height));
        contact_record.BlockHash = local_storage_encryptor.EncryptString(contact.Account.Id, contact.BlockHash,
            contact.Id + nameof(contact_record.BlockHash));
        contact_record.PrevHash = local_storage_encryptor.EncryptString(contact.Account.Id, contact.PrevHash,
            contact.Id + nameof(contact_record.PrevHash));
        contact_record.BlockPublicKey = contact.BlockPublicKey;
        contact_record.LicenseType = local_storage_encryptor.EncryptInt(contact.Account.Id, contact.LicenseType,
            contact.Id + nameof(contact_record.LicenseType));
        contact_record.LicensePubKey = local_storage_encryptor.EncryptString(contact.Account.Id, contact.LicensePubKey,
            contact.Id + nameof(contact_record.LicensePubKey));
        contact_record.Network = local_storage_encryptor.EncryptInt(contact.Account.Id, contact.Account.Network,
            contact.Id + nameof(contact_record.Network));
        return contact_record;
    }

    public static Contact RecordToContact(string account_id, ContactRecord contact_record,
        LocalStorageEncryptionService local_storage_encryptor)
    {
        var contact = Contact.Restore(
            contact_record.Id,
            local_storage_encryptor.DecryptString(account_id, contact_record.PeerUserAddress,
                contact_record.Id + nameof(contact_record.PeerUserAddress)),
            local_storage_encryptor.DecryptString(account_id, contact_record.PeerSecretAddress,
                contact_record.Id + nameof(contact_record.PeerSecretAddress)),
            local_storage_encryptor.DecryptString(account_id, contact_record.PeerNickName,
                contact_record.Id + nameof(contact_record.PeerNickName)),
            local_storage_encryptor.DecryptString(account_id, contact_record.SecretReadKey,
                contact_record.Id + nameof(contact_record.SecretReadKey)),
            local_storage_encryptor.DecryptString(account_id, contact_record.SecretScanKey,
                contact_record.Id + nameof(contact_record.SecretScanKey)),
            local_storage_encryptor.DecryptInt(account_id, contact_record.ExpirationDays,
                contact_record.Id + nameof(contact_record.ExpirationDays)), //contact_record.ExpirationDays,
            local_storage_encryptor.DecryptLong(account_id, contact_record.LastScannedChatBlock,
                contact_record.Id + nameof(contact_record.LastScannedChatBlock)), //contact_record.LastScannedChatBlock,
            local_storage_encryptor.DecryptLong(account_id, contact_record.TimeStampOut,
                contact_record.Id + nameof(contact_record.TimeStampOut)), //contact_record.TimeStampOut,
            local_storage_encryptor.DecryptLong(account_id, contact_record.TimeStampIn,
                contact_record.Id + nameof(contact_record.TimeStampIn)), //contact_record.TimeStampIn,
            local_storage_encryptor.DecryptString(account_id, contact_record.BlockPrivateKey,
                contact_record.Id + nameof(contact_record.BlockPrivateKey)),
            local_storage_encryptor.DecryptInt(account_id, contact_record.Height,
                contact_record.Id + nameof(contact_record.Height)), //contact_record.Height,
            local_storage_encryptor.DecryptString(account_id, contact_record.BlockHash,
                contact_record.Id + nameof(contact_record.BlockHash)),
            local_storage_encryptor.DecryptString(account_id, contact_record.PrevHash,
                contact_record.Id + nameof(contact_record.PrevHash)),
            contact_record.BlockPublicKey,
            local_storage_encryptor.DecryptInt(account_id, contact_record.LicenseType,
                contact_record.Id + nameof(contact_record.LicenseType)),
            local_storage_encryptor.DecryptString(account_id, contact_record.LicensePubKey,
                contact_record.Id + nameof(contact_record.LicensePubKey)),
            local_storage_encryptor.DecryptInt(account_id, contact_record.Network,
                contact_record.Id + nameof(contact_record.Network))
        );
        return contact;
    }

    public void Update(ContactRecord new_record)
    {
        if (this.Id != new_record.Id)
            throw new Exception("Contact Id does not match");
        this.AccountId = new_record.AccountId;
        this.PeerUserAddress = new_record.PeerUserAddress;
        this.PeerSecretAddress = new_record.PeerSecretAddress;
        this.PeerNickName = new_record.PeerNickName;
        this.SecretReadKey = new_record.SecretReadKey;
        this.SecretScanKey = new_record.SecretScanKey;
        this.ExpirationDays = new_record.ExpirationDays;
        this.LastScannedChatBlock = new_record.LastScannedChatBlock;
        this.TimeStampOut = new_record.TimeStampOut;
        this.TimeStampIn = new_record.TimeStampIn;
        this.BlockPrivateKey = new_record.BlockPrivateKey;
        this.Height = new_record.Height;
        this.BlockHash = new_record.BlockHash;
        this.PrevHash = new_record.PrevHash;
        this.BlockPublicKey = new_record.BlockPublicKey;
        this.LicenseType = new_record.LicenseType;
        this.LicensePubKey = new_record.LicensePubKey;
    }
}


public class ContactIndexedDb : IContactDB
{
    private readonly IIndexedDbFactory _DbFactory;
    private readonly LocalStorageEncryptionService _local_storage_encryptor;

    public ContactIndexedDb(IIndexedDbFactory DbFactory, LocalStorageEncryptionService local_storage_encryptor)
    {
        _DbFactory = DbFactory;
        _local_storage_encryptor = local_storage_encryptor;
    }

    /// <summary>
    /// Get a single secret by ID 
    /// </summary>
    public async Task<Contact> GetByBlockPubKey(string block_pub_key, string account_id)
    {
        try
        {
            using (var db = await this._DbFactory.Create<ClientDb>())
            {
                var record = db.Contacts.SingleOrDefault(x => x.BlockPublicKey == block_pub_key && x.AccountId == account_id);
                if (record != null)
                    return ContactRecord.RecordToContact(account_id, record, _local_storage_encryptor);
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        return null;
    }

    /// <summary>
    /// Store a new chat
    /// </summary>
    public async Task Add(Contact contact)
    {
        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            db.Contacts.Add(ContactRecord.ContactToRecord(contact, _local_storage_encryptor));
            await db.SaveChanges();
        }

    }


    /// <summary>
    /// Retrive all the chats that belong to Account 
    /// </summary>
    public async Task<List<Contact>> GetAll(string account_id)
    {
        var contact_list = new List<Contact>();
        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            var contact_records = db.Contacts.Where(x => x.AccountId == account_id).ToList();
            if (contact_records != null && contact_records.Count() > 0)
            {
                foreach (var contact_record in contact_records)
                {
                    contact_list.Add(ContactRecord.RecordToContact(account_id, contact_record, _local_storage_encryptor));
                }
            }
        }

        return contact_list;
    }

    ///// <summary>
    ///// Get all messages related to current chat (including those from previous chats for the same peer address)
    ///// </summary>
    //public async Task<List<Message>> GetAllMessages(string PeerAddress)
    //{
    //    var all_messages = new List<Message>();
    //    using (var db = await this._DbFactory.Create<ClientDb>())
    //    {
 
    //        var contact_record = db.Contacts.Single(x => x.PeerAddress == PeerAddress);
            
    //        if (contact_record != null)
    //        {

    //            // get all chat records ever existed for this peer address
    //            var all_chats_in = db.ChatsIn.
    //                Where(x => x.ContactId == contact_record.Id).
    //                //OrderBy(x => x.TimeStamp).
    //                ToList();

    //            var all_chats_out = db.ChatsOut.
    //                Where(x => x.ContactId == contact_record.Id).
    //                ToList();

    //            // now get all messages for each chat
    //            var msg_indexed_db = new MessageIndexedDb(_DbFactory);
    //            foreach (var chat in all_chats_in)
    //            {

    //                var messages = await msg_indexed_db.GetAll(chat.Id);
    //                all_messages.AddRange(messages);
    //            }
    //            foreach (var chat in all_chats_out)
    //            {
    //                var messages = await msg_indexed_db.GetAll(chat.Id);
    //                all_messages.AddRange(messages);
    //            }
    //        }
    //    }
    //    all_messages.Sort();
    //    return all_messages;
    //}

    //private void AddListToList(ref List<Message> dest, List<Message> source)
    //{
    //    foreach (var item in source)
    //    {
    //        dest.Add(item);
    //    }
    //}

    /// <summary>
    /// update an Account 
    /// </summary>
    public async Task Update(Contact contact)
    {
        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            var contact_record = db.Contacts.SingleOrDefault(x => x.Id == contact.Id);
            if (contact_record != null)
            {
                var updated_contact_record = ContactRecord.ContactToRecord(contact, _local_storage_encryptor);
                contact_record.Update(updated_contact_record);
                await db.SaveChanges();
            }
        }
    }

    /// <summary>
    /// Delete all contacts/chats that belong to Account 
    /// </summary>
    public async Task DeleteAll(string AccountId)
    {
        var contact_records = await GetAll(AccountId);
        foreach (var contact_record in contact_records)
        {
            await Delete(contact_record.Id, AccountId);
        }
    }

    /// <summary>
    /// Delete  
    /// </summary>
    public async Task DeleteByBlockPubKey(string block_pub_key, string account_id)
    {
        try
        {

            using (var db = await this._DbFactory.Create<ClientDb>())
            {
                //var record = db.Secrets.Single(x => x.BlockPubKey == block_pub_key);
                var record = db.Contacts.SingleOrDefault(x => x.BlockPublicKey == block_pub_key && x.AccountId == account_id);

                if (record != null)
                {
                    var result = db.Contacts.Remove(record);
                    await db.SaveChanges();
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception("Failed to delete contact: " + e.Message);
        }
    }


    /// <summary>
    /// Delete Contact 
    /// </summary>
    public async Task Delete(string contact_id, string account_id)
    {
        //var contact_indexed_db = new ContactIndexedDb(_DbFactory);
        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            var contact_record = db.Contacts.SingleOrDefault(x => x.Id == contact_id);
            if (contact_record != null)
            {
                var result = db.Contacts.Remove(contact_record);
                await db.SaveChanges();

                var chatin_db = new ChatInIndexedDb(_DbFactory, _local_storage_encryptor);
                await chatin_db.DeleteAll(contact_record.Id, account_id);
                //await db.SaveChanges();

                var chatout_db = new ChatOutIndexedDb(_DbFactory, _local_storage_encryptor);
                await chatout_db.DeleteAll(contact_record.Id, account_id);
                //await db.SaveChanges();
            }
        }
    }
}

