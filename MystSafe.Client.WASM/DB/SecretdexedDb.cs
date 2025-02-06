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

public class SecretRecord
{
    [System.ComponentModel.DataAnnotations.Key]
    public string Id { get; set; }

    //public string SecretId { get; set; }
    public string BlockPubKey { get; set; }
    public string AccountId { get; set; }
    public string EncryptedData { get; set; }
    public string BlockHash { get; set; }
    public string TimeStamp { get; set; }
    public string Height { get; set; }
    public string PrevHash { get; set; }
    public string Group { get; set; }
    public string Expiration { get; set; }
    public string InstantShareLink { get; set; }
    public string LicenseType { get; set; }
    public string DataSize { get; set; }

    public static SecretRecord ContainerToRecord(Secret secret, LocalStorageEncryptionService local_storage_encryptor)
    {
        var record = new SecretRecord();
        record.Id = secret.Id;
        //record.SecretId = secret.SecretId;
        record.BlockPubKey = secret.BlockPubKey;
        record.AccountId = secret.Account.Id;

        string serialized_data = secret.Data.ToString();
        string salt = record.BlockPubKey + nameof(record.EncryptedData);
        record.EncryptedData =
            local_storage_encryptor.EncryptString(record.AccountId, serialized_data,
                salt); //secret.Data.ClientEncrypt();
        //Console.WriteLine("To DB: " + record.EncryptedData);

        record.BlockHash = local_storage_encryptor.EncryptString(record.AccountId, secret.BlockHash,
            record.BlockPubKey + nameof(record.BlockHash));
        record.TimeStamp = local_storage_encryptor.EncryptLong(record.AccountId, secret.TimeStamp,
            record.BlockPubKey + nameof(record.TimeStamp));
        record.Height = local_storage_encryptor.EncryptInt(record.AccountId, secret.Height,
            record.BlockPubKey + nameof(record.Height));
        record.PrevHash = local_storage_encryptor.EncryptString(record.AccountId, secret.PrevHash,
            record.BlockPubKey + nameof(record.PrevHash));
        record.Group = local_storage_encryptor.EncryptString(record.AccountId, secret.Group,
            record.BlockPubKey + nameof(record.Group));
        record.Expiration = local_storage_encryptor.EncryptInt(record.AccountId, secret.Expiration,
            record.BlockPubKey + nameof(record.Expiration));
        record.LicenseType = local_storage_encryptor.EncryptInt(record.AccountId, secret.LicenseType,
            record.BlockPubKey + nameof(record.LicenseType));
        record.DataSize = local_storage_encryptor.EncryptInt(record.AccountId, secret.DataSize,
            record.BlockPubKey + nameof(record.DataSize));

        return record;
    }

    public static Secret RecordToContainer(string account_id, SecretRecord record,
        LocalStorageEncryptionService local_storage_encryptor)
    {
        var secret = Secret.FromRecord(record.Id); //new Secret();
        //secret.Id = record.Id;
        secret.BlockPubKey = record.BlockPubKey;
        secret.TimeStamp = local_storage_encryptor.DecryptLong(account_id, record.TimeStamp,
            record.BlockPubKey + nameof(record.TimeStamp));

        Console.WriteLine("From DB: " + record.EncryptedData);

        string salt = record.BlockPubKey + nameof(record.EncryptedData);
        string decrypted_data = local_storage_encryptor.DecryptString(account_id, record.EncryptedData, salt);

        secret.Data = SecretBlockData.ClientDecrypt(decrypted_data);

        secret.BlockHash = local_storage_encryptor.DecryptString(account_id, record.BlockHash,
            record.BlockPubKey + nameof(record.BlockHash));
        secret.Height =
            local_storage_encryptor.DecryptInt(account_id, record.Height, record.BlockPubKey + nameof(record.Height));
        secret.PrevHash = local_storage_encryptor.DecryptString(account_id, record.PrevHash,
            record.BlockPubKey + nameof(record.PrevHash));
        secret.Group =
            local_storage_encryptor.DecryptString(account_id, record.Group, record.BlockPubKey + nameof(record.Group));
        secret.Expiration = local_storage_encryptor.DecryptInt(account_id, record.Expiration,
            record.BlockPubKey + nameof(record.Expiration));
        secret.LicenseType = local_storage_encryptor.DecryptInt(account_id, record.LicenseType,
            record.BlockPubKey + nameof(record.LicenseType));
        try
        {
            secret.DataSize = local_storage_encryptor.DecryptInt(account_id, record.DataSize,
                record.BlockPubKey + nameof(record.DataSize));
        }
        catch (Exception e)
        {
            Console.WriteLine("DATA SIZE EXCEPTION " + e);
        }

        return secret;
    }

    public void Update(SecretRecord new_record)
    {
        //if (this.Id != new_record.Id)
        //    throw new Exception("Id does not match");
        if (this.BlockPubKey != new_record.BlockPubKey || this.AccountId != new_record.AccountId)
            throw new Exception("BlockPubKey does not match");
        //this.AccountId = new_record.AccountId;
        this.Id = new_record.Id;
        this.EncryptedData = new_record.EncryptedData;
        this.BlockHash = new_record.BlockHash;
        this.TimeStamp = new_record.TimeStamp;
        this.Height = new_record.Height;
        this.PrevHash = new_record.PrevHash;
        this.Group = new_record.Group;
        this.Expiration = new_record.Expiration;
        this.LicenseType = new_record.LicenseType;
        this.DataSize = new_record.DataSize;
    }
}


public class SecretIndexedDb : ISecretDB
{
    private readonly IIndexedDbFactory _DbFactory;
    private readonly LocalStorageEncryptionService _local_storage_encryptor;

    public SecretIndexedDb(IIndexedDbFactory DbFactory, LocalStorageEncryptionService local_storage_encryptor)
    {
        _DbFactory = DbFactory;
        _local_storage_encryptor = local_storage_encryptor;
    }

    /// <summary>
    /// Store a new secret
    /// </summary>
    public async Task Add(Secret secret)
    {
        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            db.Secrets.Add(SecretRecord.ContainerToRecord(secret, _local_storage_encryptor));
            await db.SaveChanges();
        }

    }


    /// <summary>
    /// Retrive all the chats that belong to Account 
    /// </summary>
    public async Task<List<Secret>> GetAll(string account_id)
    {
        var list = new List<Secret>();
        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            var records = db.Secrets.Where(x => x.AccountId == account_id).ToList();
            if (records != null && records.Count() > 0)
            {
                foreach (var record in records)
                {
                    list.Add(SecretRecord.RecordToContainer(account_id, record, _local_storage_encryptor));
                }
            }
        }

        return list;
    }

    /// <summary>
    /// Get a single secret by ID 
    /// </summary>
    public async Task<Secret> GetByBlockPubKey(string block_pub_key, string account_id)
    {
        try
        {
            using (var db = await this._DbFactory.Create<ClientDb>())
            {
                //var record = db.Secrets.SingleOrDefault(x => x.BlockPubKey == block_pub_key);
                var record =
                    db.Secrets.SingleOrDefault(x => x.BlockPubKey == block_pub_key && x.AccountId == account_id);
                if (record != null)
                    return SecretRecord.RecordToContainer(account_id, record, _local_storage_encryptor);
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }

        return null;
    }

    /// <summary>
    /// update secret using pub_key as the inique identifier
    /// </summary>
    public async Task Update(Secret updated_secret)
    {
        using (var db = await this._DbFactory.Create<ClientDb>())
        {
            //var record = db.Secrets.SingleOrDefault(x => x.BlockPubKey == updated_secret.BlockPubKey);
            var record = db.Secrets.SingleOrDefault(x =>
                x.BlockPubKey == updated_secret.BlockPubKey && x.AccountId == updated_secret.Account.Id);

            if (record != null)
            {
                var updated_record = SecretRecord.ContainerToRecord(updated_secret, _local_storage_encryptor);
                record.Update(updated_record);
                await db.SaveChanges();
            }
        }
    }

    /// <summary>
    /// Delete all contacts/chats that belong to Account 
    /// </summary>
    public async Task DeleteAll(string account_id)
    {
        if (string.IsNullOrEmpty(account_id))
            return;

        var records = await GetAll(account_id);
        foreach (var record in records)
        {
            //await DeleteById(record.Id);
            await DeleteByBlockPubKey(record.BlockPubKey, account_id);
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
                var record =
                    db.Secrets.SingleOrDefault(x => x.BlockPubKey == block_pub_key && x.AccountId == account_id);

                if (record != null)
                {
                    var result = db.Secrets.Remove(record);
                    await db.SaveChanges();
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception("Failed to delete secret: " + e.Message);
        }
    }
}

