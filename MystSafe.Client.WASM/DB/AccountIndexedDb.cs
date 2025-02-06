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

using Microsoft.JSInterop;
using IndexedDB.Blazor;
using MystSafe.Shared.Common;
using MystSafe.Client.Engine;
using MystSafe.Client.Base;
using MystSafe.Client.CryptoLicense;

namespace MystSafe.Client.App;




public class AccountIndexedDb : IAccountDB
{
    private readonly IIndexedDbFactory _DbFactory;
    private readonly LocalStorageEncryptionService _local_storage_encryptor;

    public AccountIndexedDb(IIndexedDbFactory DbFactory, LocalStorageEncryptionService local_storage_encryptor)
    {
        _DbFactory = DbFactory;
        _local_storage_encryptor = local_storage_encryptor;
    }

    /// <summary>
    /// Store a new Account
    /// </summary>
    public async Task Add(Account account)
    {

        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            var account_record = AccountRecord.AccountToRecord(account, _local_storage_encryptor);
            db.Accounts.Add(account_record);
            await db.SaveChanges();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public async Task<AccountAuthnResult> GetMostRecentlyUsedAccountAuthnData()
    {
        AccountAuthnResult result = new AccountAuthnResult();
        try
        {
            using (var db = await this._DbFactory.Create<ClientDb>())
            {
                var account_record = db.Accounts.OrderByDescending(e => e.LastUpdated).FirstOrDefault();
                if (account_record != null)
                {
                    result.AccountId = account_record.Id;
                    result.ResultCode = ResultStatusCodes.SUCCESS;
                    result.LocalAuthnType = account_record.LocalAuthnType;
                    result.LocalEncryptionKey = account_record.LocalEncryptionKey;
                    result.LocalKeyId = account_record.LocalKeyId;
                    result.PasskeyCredentials = account_record.PasskeyCredentials;
                }
                else
                {
                    result.ResultCode = ResultStatusCodes.NOT_FOUND;
                    result.ResultMessage = "No accounts found";
                }
            }
        }
        catch (Exception e)
        {
            result.ResultCode = ResultStatusCodes.EXCEPTION;
            result.ResultMessage = e.Message;
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    public async Task<Account> RetrieveAccount(string account_id)
    {
        Account account = null;

        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            var account_record = db.Accounts.SingleOrDefault(x => x.Id == account_id);

            if (account_record != null)
            {
                account = AccountRecord.RecordToAccount(account_record, _local_storage_encryptor);

                var contact_indexed_db = new ContactIndexedDb(_DbFactory, _local_storage_encryptor);
                var contacts = await contact_indexed_db.GetAll(account.Id);
                foreach (var contact in contacts)
                {

                    var chat_in_indexed_db = new ChatInIndexedDb(_DbFactory, _local_storage_encryptor);
                    var chats_in = await chat_in_indexed_db.GetAll(contact.Id, account_id);
                    foreach (var chat_in in chats_in)
                    {
                        var message_db = new MessageIndexedDb(_DbFactory, _local_storage_encryptor);
                        var messages = await message_db.GetAll(chat_in.Id, account_id);
                        foreach (var message in messages)
                            chat_in.AddMessage(message);

                        contact.AddChatIn(chat_in);
                    }

                    var chat_out_indexed_db = new ChatOutIndexedDb(_DbFactory, _local_storage_encryptor);
                    var chats_out = await chat_out_indexed_db.GetAll(contact.Id, account_id);
                    foreach (var chat_out in chats_out)
                    {
                        var message_db = new MessageIndexedDb(_DbFactory, _local_storage_encryptor);
                        var messages = await message_db.GetAll(chat_out.Id, account_id);
                        foreach (var message in messages)
                            chat_out.AddMessage(message);

                        contact.AddChatOut(chat_out);
                    }

                    account.AddContact(contact);
                }

                var secret_indexed_db = new SecretIndexedDb(_DbFactory, _local_storage_encryptor);
                var secrets = await secret_indexed_db.GetAll(account.Id);
                foreach (var secret in secrets)
                {
                    account.AddSecret(secret);
                }
            }
        }

        return account;
    }

    /// <summary>
    /// update an Account 
    /// </summary>
    public async Task Update(Account account)
    {
        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            var account_record = db.Accounts.SingleOrDefault(x => x.Id == account.Id);
            if (account_record != null)
            {
                var updated_record = AccountRecord.AccountToRecord(account, _local_storage_encryptor);
                account_record.Update(updated_record);
                await db.SaveChanges();
            }
        }
    }

    /// <summary>
    /// Delete Account 
    /// </summary>
    public async Task Delete(string Id)
    {
        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            try
            {
                var account_record = db.Accounts.SingleOrDefault(x => x.Id == Id);
                if (account_record != null)
                {
                    var result = db.Accounts.Remove(account_record);
                    await db.SaveChanges();

                    var contact_db = new ContactIndexedDb(_DbFactory, _local_storage_encryptor);
                    await contact_db.DeleteAll(Id);

                    var secret_db = new SecretIndexedDb(_DbFactory, _local_storage_encryptor);
                    await secret_db.DeleteAll(Id);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }
}


