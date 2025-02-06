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

using MongoDB.Driver;
using MystSafe.Shared.Common;

namespace MystSafe.Backend.DB;

public class ContactBlockDb : IBlockDb
{
    public const string CONTACTS_COLLECTION = "contacts";
    private readonly IMongoCollection<ContactDataRecord> _blockCollection;

    public ContactBlockDb(DatabaseService database)
    {
        _blockCollection = database.MystSafeDB.GetCollection<ContactDataRecord>(CONTACTS_COLLECTION);

        var indexKeysDefinitionTimeStamp = Builders<ContactDataRecord>.IndexKeys.Ascending(x => x.TimeStamp);
        var indexOptionsTimeStamp = new CreateIndexOptions { Name = "ContactTimeStampIndex", Background = true };
        //var indexOptionsTimeStamp = new CreateIndexOptions { Background = true };
        var indexModelTimeStamp =
            new CreateIndexModel<ContactDataRecord>(indexKeysDefinitionTimeStamp, indexOptionsTimeStamp);
        _blockCollection.Indexes.CreateOne(indexModelTimeStamp);

        var indexKeysDefinitionChatPubKey = Builders<ContactDataRecord>.IndexKeys.Ascending(x => x.PubKey);
        var indexOptionsChatPubKey = new CreateIndexOptions { Name = "ContactPubKeyIndex", Background = true };

        var indexModelChatPubKey =
            new CreateIndexModel<ContactDataRecord>(indexKeysDefinitionChatPubKey, indexOptionsChatPubKey);
        _blockCollection.Indexes.CreateOne(indexModelChatPubKey);
    }


    // public bool Exists(string hash)
    // {
    //     var filter = Builders<ContactDataRecord>.Filter.Eq(x => x.Hash, hash);
    //     return _blockCollection.Find(filter).Any();
    // }

    public async Task<bool> ExistsAsync(string hash)
    {
        var filter = Builders<ContactDataRecord>.Filter.Eq(x => x.Hash, hash);
        var block = await _blockCollection.FindAsync(filter);
        return block.Any();
    }

    public async Task<bool> ExistsByLicenseAsync(string licensePubKey)
    {
        var filter = Builders<ContactDataRecord>.Filter.Eq(x => x.License, licensePubKey);
        var exists = await _blockCollection.Find(filter).AnyAsync();
        return exists;
    }

    public async Task<List<ContactBlock>> Scan(long lastScannedDateTime, int resultPerPage)
    {
        var filter = Builders<ContactDataRecord>.Filter.Gt(x => x.TimeStamp, lastScannedDateTime);
        var sort = Builders<ContactDataRecord>.Sort.Ascending(x => x.TimeStamp);


        var cursor = await _blockCollection.FindAsync(filter,
            new FindOptions<ContactDataRecord> { Sort = sort, Limit = resultPerPage });
        var collection = await cursor.ToListAsync();

        var result = new List<ContactBlock>();
        foreach (var item in collection) result.Add(item.ToProto());
        return result;
    }

    public async Task<bool> Add(ContactBlock block)
    {
        try
        {
            await _blockCollection.InsertOneAsync(ContactDataRecord.FromProto(block));
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception in ContactMonfgoDB.Add(): " + e.Message);
            return false;
        }
    }

    // public ContactBlock GetByHash(string hash)
    // {
    //     var filter = Builders<ContactDataRecord>.Filter.Eq(x => x.Hash, hash);
    //     var block = _blockCollection.Find(filter).FirstOrDefault();
    //     if (block == null)
    //         return null;
    //     return block.ToProto();
    // }

    // this will delete data from all blocks in the chan i.e. having the same pub key
    // should be used by delete block
    public async Task<bool> ResetBlockDataByPubKey(string block_pub_key)
    {
        try
        {
            var filter = Builders<ContactDataRecord>.Filter.Eq(x => x.PubKey, block_pub_key);

            // Combine both updates into a single operation
            var combinedUpdate = Builders<ContactDataRecord>.Update
                .Set(x => x.InitData, string.Empty)
                .Set(x => x.MessageData, string.Empty);

            var result = await _blockCollection.UpdateManyAsync(filter, combinedUpdate);
            return result.ModifiedCount > 0;
        }
        catch (Exception e)
        {
            throw new Exception("Exception in ResetBlockDataAsync(): " + e.Message);
        }
    }

    public async Task<List<DeleteLogDTO>> DeleteExpiredContactBlocksAsync(long thresholdUnixTime)
    {
        var filter = Builders<ContactDataRecord>.Filter.And(
            Builders<ContactDataRecord>.Filter.Lt(x => x.TimeStamp, thresholdUnixTime),
            Builders<ContactDataRecord>.Filter.Or(
                Builders<ContactDataRecord>.Filter.Eq(x => x.License, null),
                Builders<ContactDataRecord>.Filter.Eq(x => x.License, "")
            )
        );

        // Retrieve the blocks to be deleted
        var contactBlocksToDelete = await _blockCollection.Find(filter).ToListAsync();

        // Log deletion details
        var deletionLogs = contactBlocksToDelete.Select(x => new DeleteLogDTO
        {
            BlockHash = x.Hash,
            CreatedTimeStamp = x.TimeStamp,
            DeletedTimeStamp = UnixDateTime.Now,
            BlockType = (int)BlockTypes.Contact,
            BlockPubKey = x.PubKey
        }).ToList();

        // Perform the deletion
        if (contactBlocksToDelete.Any())
        {
            var deleteResult = await _blockCollection.DeleteManyAsync(filter);
        }

        return deletionLogs;
    }
}