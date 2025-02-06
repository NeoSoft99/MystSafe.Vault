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

public class SecretBlockDb : IBlockDb
{
    public const string SECRETS_COLLECTION = "secrets";

    private readonly IMongoCollection<SecretBlockRecord> _blockCollection;

    public SecretBlockDb(DatabaseService database)
    {
        _blockCollection = database.MystSafeDB.GetCollection<SecretBlockRecord>(SECRETS_COLLECTION);

        var indexKeysDefinitionTimeStamp = Builders<SecretBlockRecord>.IndexKeys.Ascending(x => x.TimeStamp);
        var indexOptionsTimeStamp = new CreateIndexOptions { Name = "SecretsTimeStampIndex", Background = true };
        //var indexOptionsTimeStamp = new CreateIndexOptions { Background = true };
        var indexModelTimeStamp =
            new CreateIndexModel<SecretBlockRecord>(indexKeysDefinitionTimeStamp, indexOptionsTimeStamp);
        _blockCollection.Indexes.CreateOne(indexModelTimeStamp);

        var indexKeysDefinitionChatPubKey = Builders<SecretBlockRecord>.IndexKeys.Ascending(x => x.PubKey);
        var indexOptionsChatPubKey = new CreateIndexOptions { Name = "SecretsPubKeyIndex", Background = true };
        //var indexOptionsChatPubKey = new CreateIndexOptions { Background = true };
        var indexModelChatPubKey =
            new CreateIndexModel<SecretBlockRecord>(indexKeysDefinitionChatPubKey, indexOptionsChatPubKey);
        _blockCollection.Indexes.CreateOne(indexModelChatPubKey);

        var indexKeysDefinitionGroup = Builders<SecretBlockRecord>.IndexKeys.Ascending(x => x.Group);
        var indexOptionsGroup = new CreateIndexOptions { Name = "SecretsGroupIndex", Background = true };
        //var indexOptionsGroup = new CreateIndexOptions { Background = true };
        var indexModelGroup = new CreateIndexModel<SecretBlockRecord>(indexKeysDefinitionGroup, indexOptionsGroup);
        _blockCollection.Indexes.CreateOne(indexModelGroup);
    }

    // public bool Exists(string hash)
    // {
    //     var filter = Builders<SecretBlockRecord>.Filter.Eq(x => x.Hash, hash);
    //     return _blockCollection.Find(filter).Any();
    // }

    public async Task<bool> ExistsAsync(string hash)
    {
        var filter = Builders<SecretBlockRecord>.Filter.Eq(x => x.Hash, hash);
        var block = await _blockCollection.FindAsync(filter);
        return block.Any();
    }

    public async Task<bool> ExistsByLicenseAsync(string licensePubKey)
    {
        var filter = Builders<SecretBlockRecord>.Filter.Eq(x => x.License, licensePubKey);
        var secretBlockRecords = await _blockCollection.Find(filter).AnyAsync();
        return secretBlockRecords;
    }

    public async Task<List<SecretBlock>> GetRuntimeBlock(string group_key)
    {
        var filter = Builders<SecretBlockRecord>.Filter.Eq(x => x.Group, group_key);
        var sort = Builders<SecretBlockRecord>.Sort.Ascending(x => x.TimeStamp);

        var cursor = await _blockCollection.FindAsync(filter, new FindOptions<SecretBlockRecord> { Sort = sort });
        //var cursor = await _blockCollection.FindAsync(filter);
        var collection = await cursor.ToListAsync();

        // Group by PubKey and select the one with the greatest TimeStamp
        var groupedCollection = collection
            .GroupBy(x => x.PubKey)
            .Select(g => g.OrderByDescending(x => x.TimeStamp).First())
            .ToList();

        var result = new List<SecretBlock>();
        foreach (var item in groupedCollection) result.Add(item.ToProto());
        return result;
    }

    public async Task<List<SecretBlock>> Scan(long lastScannedDateTime, int resultPerPage)
    {
        var filter = Builders<SecretBlockRecord>.Filter.Gt(x => x.TimeStamp, lastScannedDateTime);
        var sort = Builders<SecretBlockRecord>.Sort.Ascending(x => x.TimeStamp);

        var cursor = await _blockCollection.FindAsync(filter,
            new FindOptions<SecretBlockRecord> { Sort = sort, Limit = resultPerPage });
        var collection = await cursor.ToListAsync();

        var result = new List<SecretBlock>();
        foreach (var item in collection) result.Add(item.ToProto());
        return result;
    }

    public async Task<bool> Add(SecretBlock block)
    {
        try
        {
            await _blockCollection.InsertOneAsync(SecretBlockRecord.FromProto(block));
            return true;
        }
        catch
        {
            return false;
        }
    }


    public async Task<SecretBlock> GetByHash(string hash)
    {
        var filter = Builders<SecretBlockRecord>.Filter.Eq(x => x.Hash, hash);
        var block = await _blockCollection.Find(filter).FirstOrDefaultAsync();
        if (block != null) return block.ToProto();

        return null;
    }


    // this will "neutralize" the block by deleteing it's secret data part while leaving the block itself for a while to preserve the DAG integrity
    public async Task<bool> ResetBlockDataAsync(string hash)
    {
        try
        {
            var filter = Builders<SecretBlockRecord>.Filter.Eq(x => x.Hash, hash);
            var update = Builders<SecretBlockRecord>.Update.Set(x => x.SecretData, string.Empty);

            var result = await _blockCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
        catch
        {
            // Handle exception, log error, etc.
            return false;
        }
    }

    //public async Task<long> DeleteExpiredSecretBlocksAsync(long thresholdUnixTime)
    //{
    //    var filter = Builders<SecretBlockRecord>.Filter.And(
    //        Builders<SecretBlockRecord>.Filter.Lt(x => x.TimeStamp, thresholdUnixTime),
    //        Builders<SecretBlockRecord>.Filter.Or(
    //            Builders<SecretBlockRecord>.Filter.Eq(x => x.License, null),
    //            Builders<SecretBlockRecord>.Filter.Eq(x => x.License, "")
    //        )
    //    );

    //    var deleteResult = await _blockCollection.DeleteManyAsync(filter);

    //    return deleteResult.DeletedCount;
    //}

    public async Task<List<DeleteLogDTO>> DeleteExpiredSecretBlocksAsync(long thresholdUnixTime)
    {
        var filter = Builders<SecretBlockRecord>.Filter.And(
            Builders<SecretBlockRecord>.Filter.Lt(x => x.TimeStamp, thresholdUnixTime),
            Builders<SecretBlockRecord>.Filter.Or(
                Builders<SecretBlockRecord>.Filter.Eq(x => x.License, null),
                Builders<SecretBlockRecord>.Filter.Eq(x => x.License, "")
            )
        );

        // Retrieve the blocks to be deleted
        var secretBlocksToDelete = await _blockCollection.Find(filter).ToListAsync();

        // Log deletion details
        var deletionLogs = secretBlocksToDelete.Select(x => new DeleteLogDTO
        {
            BlockHash = x.Hash,
            CreatedTimeStamp = x.TimeStamp,
            DeletedTimeStamp = UnixDateTime.Now,
            BlockType = (int)BlockTypes.Secret,
            BlockPubKey = x.PubKey
        }).ToList();

        // Perform the deletion
        if (secretBlocksToDelete.Any())
        {
            var deleteResult = await _blockCollection.DeleteManyAsync(filter);
        }

        return deletionLogs;
    }
}