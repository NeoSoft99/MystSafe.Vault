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
///     Message storage
/// </summary>
public class MsgBlockDb : IBlockDb
{
    public const string MESSAGES_COLLECTION = "messages";
    private readonly IMongoCollection<MessageBlockRecord> _blockCollection;

    public MsgBlockDb(DatabaseService database)
    {
        _blockCollection = database.MystSafeDB.GetCollection<MessageBlockRecord>(MESSAGES_COLLECTION);

        var indexKeysDefinitionTimeStamp = Builders<MessageBlockRecord>.IndexKeys.Ascending(x => x.TimeStamp);
        var indexOptionsTimeStamp = new CreateIndexOptions { Name = "MessagesTimeStampIndex", Background = true };
        //var indexOptionsTimeStamp = new CreateIndexOptions { Background = true };
        var indexModelTimeStamp =
            new CreateIndexModel<MessageBlockRecord>(indexKeysDefinitionTimeStamp, indexOptionsTimeStamp);
        _blockCollection.Indexes.CreateOne(indexModelTimeStamp);

        var indexKeysDefinitionChatPubKey = Builders<MessageBlockRecord>.IndexKeys.Ascending(x => x.ChatPubKey);
        var indexOptionsChatPubKey = new CreateIndexOptions { Name = "MessagesPubKeyIndex", Background = true };
        //var indexOptionsChatPubKey = new CreateIndexOptions { Background = true };
        var indexModelChatPubKey =
            new CreateIndexModel<MessageBlockRecord>(indexKeysDefinitionChatPubKey, indexOptionsChatPubKey);
        _blockCollection.Indexes.CreateOne(indexModelChatPubKey);
    }

    // public bool Exists(string hash)
    // {
    //     var result = _blockCollection.Find(x => x.Hash == hash)
    //         .FirstOrDefault();
    //     return result != null;
    // }

    public async Task<bool> ExistsAsync(string hash)
    {
        var filter = Builders<MessageBlockRecord>.Filter.Eq(x => x.Hash, hash);
        var block = await _blockCollection.FindAsync(filter);
        return block.Any();
    }

    public async Task<ScanMsgResult> Scan(int lastScannedHeight, string ChatPubKey)
    {
        var query = await _blockCollection.Find(x =>
                x.ChatPubKey == ChatPubKey && x.Height > lastScannedHeight && x.Height <= lastScannedHeight + 2)
            .SortBy(x => x.Height)
            .Limit(2)
            .ToListAsync();

        var result = new ScanMsgResult();
        if (query.Count >= 2)
        {
            result.Block = query.FirstOrDefault().ToProto();
            result.HasMore = true;
        }
        else if (query.Count == 1)
        {
            result.Block = query.FirstOrDefault().ToProto();
            result.HasMore = false;
        }
        else
        {
            result.Block = null;
            result.HasMore = false;
        }

        return result;
    }

    public async Task<bool> Add(MsgBlock block)
    {
        try
        {
            await _blockCollection.InsertOneAsync(MessageBlockRecord.FromProto(block));
            return true;
        }
        catch
        {
            return false;
        }
    }

    // public MsgBlock GetByHash(string hash)
    // {
    //     var message_data = _blockCollection.Find(x => x.Hash == hash).FirstOrDefault();
    //     return message_data.ToProto();
    // }

    // this will "neutralize" the block by deleteing it's secret data part while leaving the block itself for a while to preserve the DAG integrity
    public async Task<bool> ResetBlockDataAsync(string hash)
    {
        try
        {
            var filter = Builders<MessageBlockRecord>.Filter.Eq(x => x.Hash, hash);
            var update = Builders<MessageBlockRecord>.Update.Set(x => x.MsgData, string.Empty);

            var result = await _blockCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }
        catch
        {
            // Handle exception, log error, etc.
            return false;
        }
    }

    //public async Task<long> DeleteMessageBlocksForExpiredChats(List<DeleteLogDTO> deleted_blocks)
    //{
    //    if (deleted_blocks == null || !deleted_blocks.Any())
    //        return 0;

    //    var expiredChatPubkeys = deleted_blocks.Select(log => log.BlockPubKey).ToList();

    //    var filter = Builders<MessageBlockRecord>.Filter.In(x => x.ChatPubKey, expiredChatPubkeys);
    //    var result = await _blockCollection.DeleteManyAsync(filter);
    //    return result.DeletedCount;
    //}

    public async Task<List<DeleteLogDTO>> DeleteMessageBlocksForExpiredChats(List<DeleteLogDTO> deletedBlocks)
    {
        if (deletedBlocks == null || !deletedBlocks.Any())
            return new List<DeleteLogDTO>(); // Return an empty list if there are no blocks to delete

        var expiredChatPubkeys = deletedBlocks.Select(log => log.BlockPubKey).ToList();
        var filter = Builders<MessageBlockRecord>.Filter.In(x => x.ChatPubKey, expiredChatPubkeys);

        // Retrieve the blocks to delete based on the filter
        var messageBlocksToDelete = await _blockCollection.Find(filter).ToListAsync();

        // Create deletion logs for these blocks
        var deletionLogs = messageBlocksToDelete.Select(x => new DeleteLogDTO
        {
            BlockHash = x.Hash,
            CreatedTimeStamp = x.TimeStamp,
            DeletedTimeStamp = UnixDateTime.Now,
            BlockType = (int)BlockTypes.Message,
            BlockPubKey = x.ChatPubKey
        }).ToList();

        // Check if there are records to delete and perform deletion
        if (messageBlocksToDelete.Any())
        {
            var deleteResult = await _blockCollection.DeleteManyAsync(filter);
        }

        return deletionLogs;
    }
}