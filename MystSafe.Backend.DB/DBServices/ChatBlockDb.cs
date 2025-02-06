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

/// <summary>
///     Block Database to keep block persistent
/// </summary>
public class ChatBlockDb : IBlockDb
{
    public const string CHATS_COLLECTION = "chats";
    private readonly IMongoCollection<ChatDataRecord> _blockCollection;

    public ChatBlockDb(DatabaseService database)
    {
        _blockCollection = database.MystSafeDB.GetCollection<ChatDataRecord>(CHATS_COLLECTION);

        var indexKeysDefinitionTimeStamp = Builders<ChatDataRecord>.IndexKeys.Ascending(x => x.TimeStamp);
        var indexOptionsTimeStamp = new CreateIndexOptions { Name = "ChatsTimeStampIndex", Background = true };
        //var indexOptionsTimeStamp = new CreateIndexOptions { Background = true };
        var indexModelTimeStamp =
            new CreateIndexModel<ChatDataRecord>(indexKeysDefinitionTimeStamp, indexOptionsTimeStamp);
        _blockCollection.Indexes.CreateOne(indexModelTimeStamp);

        var indexKeysDefinitionChatPubKey = Builders<ChatDataRecord>.IndexKeys.Ascending(x => x.ChatPubkey);
        var indexOptionsChatPubKey = new CreateIndexOptions { Name = "ChatsPubKeyIndex", Background = true };
        //var indexOptionsChatPubKey = new CreateIndexOptions { Background = true };
        var indexModelChatPubKey =
            new CreateIndexModel<ChatDataRecord>(indexKeysDefinitionChatPubKey, indexOptionsChatPubKey);
        _blockCollection.Indexes.CreateOne(indexModelChatPubKey);
    }

    public async Task<bool> ExistsAsync(string hash)
    {
        var filter = Builders<ChatDataRecord>.Filter.Eq(x => x.Hash, hash);
        var block = await _blockCollection.FindAsync(filter);
        return block.Any();
    }

    public async Task<bool> ExistsByLicenseAsync(string licensePubKey)
    {
        var filter = Builders<ChatDataRecord>.Filter.Eq(x => x.License, licensePubKey);
        var exists = await _blockCollection.Find(filter).AnyAsync();
        return exists;
    }

    public async Task<List<InitBlock>> Scan(long lastScannedDateTime, int resultPerPage)
    {
        //var query = _blockCollection.Find(x => x.TimeStamp > lastScannedDateTime)
        //    .SortBy(x => x.TimeStamp)
        //    .Limit(resultPerPage)
        //    .ToList();

        var filter = Builders<ChatDataRecord>.Filter.Gt(x => x.TimeStamp, lastScannedDateTime);
        var sort = Builders<ChatDataRecord>.Sort.Ascending(x => x.TimeStamp);

        var cursor = await _blockCollection.FindAsync(filter,
            new FindOptions<ChatDataRecord> { Sort = sort, Limit = resultPerPage });
        var collection = await cursor.ToListAsync();

        var result = new List<InitBlock>();
        foreach (var item in collection) result.Add(item.ToProto());

        return result;
    }

    public async Task<bool> Add(InitBlock block)
    {
        try
        {
            await _blockCollection.InsertOneAsync(ChatDataRecord.FromProto(block));
            return true;
        }
        catch
        {
            return false;
        }
    }

    //public InitBlock GetLast()
    //{
    //    var block = _blockCollection.Find(_ => true)
    //        .SortByDescending(x => x.TimeStamp)
    //        .FirstOrDefault();
    //    if (block == null)
    //        return null;
    //    return block.ToProto();
    //}

    public async Task<InitBlock> GetChatInit(string ChatPubkey)
    {
        var block = await _blockCollection.Find(x => x.ChatPubkey == ChatPubkey).FirstOrDefaultAsync();
        if (block == null)
            return null;
        return block.ToProto();
    }

    public bool Exists(string hash)
    {
        var result = _blockCollection.Find(x => x.Hash == hash)
            .FirstOrDefault();
        return result != null;
    }

    public async Task<bool> ResetBlockDataAsync(string hash)
    {
        try
        {
            var filter = Builders<ChatDataRecord>.Filter.Eq(x => x.Hash, hash);
            var update = Builders<ChatDataRecord>.Update.Set(x => x.MessageData, string.Empty);

            var result = await _blockCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
        catch
        {
            // Handle exception, log error, etc.
            return false;
        }
    }

    public async Task<List<DeleteLogDTO>> DeleteOldBlocksAsync(long thresholdUnixTime)
    {
        // Define the filter criteria: TimeStamp older than the threshold and License is null or empty
        var filter = Builders<ChatDataRecord>.Filter.And(
            Builders<ChatDataRecord>.Filter.Lt(x => x.TimeStamp, thresholdUnixTime),
            Builders<ChatDataRecord>.Filter.Or(
                Builders<ChatDataRecord>.Filter.Eq(x => x.License, null),
                Builders<ChatDataRecord>.Filter.Eq(x => x.License, "")
            )
        );

        // Retrieve the blocks to delete
        var chatBlocksToDelete = await _blockCollection.Find(filter).ToListAsync();

        // Convert found records to DTOs
        var deletionLogs = chatBlocksToDelete.Select(x => new DeleteLogDTO
        {
            BlockHash = x.Hash,
            CreatedTimeStamp = x.TimeStamp,
            DeletedTimeStamp = UnixDateTime.Now,
            BlockType = (int)BlockTypes.Chat,
            BlockPubKey = x.ChatPubkey
        }).ToList();

        // Check if there are records to delete and perform deletion
        if (chatBlocksToDelete.Any())
        {
            var deleteResult = await _blockCollection.DeleteManyAsync(filter);
        }

        return deletionLogs;
    }


    //public InitBlock GetByHash(string hash)
    //{
    //    var block = _blockCollection.Find(x => x.Hash == hash).FirstOrDefault();
    //    if (block == null)
    //        return null;
    //    return block.ToProto();
    //}
}