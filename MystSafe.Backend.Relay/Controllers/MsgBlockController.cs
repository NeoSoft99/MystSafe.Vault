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

using Microsoft.AspNetCore.Mvc;
using MystSafe.Backend.DB;
using MystSafe.Shared.Common;
using MystSafe.Shared.Crypto;

namespace MystSafe.Backend.Relay;

[ApiController]
[Route("api/[controller]")]
public class MsgBlockController : ControllerBase
{
    //private readonly MsgBlockNodeServiceClient _nodeService;
    private readonly ILogger<MsgBlockController> _logger;
    private readonly MsgBlockDb _blockDb;
    private readonly ChatBlockDb _chatDb;
    //private readonly LicenseBlockDb _licenseDb;
    private readonly DatabaseService _databaseService;
    private readonly BackendLicenseValidationService _licenseProofValidationService;

    public MsgBlockController(ILogger<MsgBlockController> logger, MsgBlockDb blockDb, ChatBlockDb chatDb,
        DatabaseService databaseService,
        BackendLicenseValidationService licenseProofValidationService)
    {
        _logger = logger;
        _blockDb = blockDb;
        _chatDb = chatDb;
        //_licenseDb = licenseDb;
        _databaseService = databaseService;
        _licenseProofValidationService = licenseProofValidationService;
    }

    [HttpPost("Broadcast")]
    public async Task<ActionResult<AddMsgBlockStatus>> Broadcast([FromBody] MsgBlock block)
    {
        var result = new AddMsgBlockStatus();
        try
        {
            if (block.Network != _databaseService.Network)
            {
                result.Status = ResultStatusCodes.WRONG_NETWORK;
                result.Message = string.Format("Wrong network: {0} ", Networks.ToString(block.Network));
                _logger.LogError(result.Message);
                return Ok(result);
            }

            //var result = await _nodeService.BroadcastAsync(block);
            if (await _blockDb.ExistsAsync(block.Hash))
            {
                result.Status = ResultStatusCodes.BLOCK_ALREADY_EXISTS;
                result.Message = string.Format("Block already exists in the database: {0} ", block.Hash);
                Console.WriteLine(result.Message);
                return result;
            }

            var chat_block = await _chatDb.GetChatInit(block.ChatPubKey);
            if (chat_block == null)
            {
                result.Status = ResultStatusCodes.NOT_FOUND;
                result.Message = string.Format("Chat block bot found: {0} ", block.ChatPubKey);
                _logger.LogError(result.Message);
                return Ok(result);
            }

            var chat_validator = new ChatBlockValidator(chat_block);
            
            var validator = new MsgBlockValidator(block);
            validator.ServerValidate(chat_validator);
            
            await _licenseProofValidationService.ValidateLicense(chat_validator);

            var database_result = await _blockDb.Add(block);
            if (!database_result)
            {
                result.Status = ResultStatusCodes.BLOCK_FAILED_TO_ADD_TO_DATABASE;
                result.Message = string.Format("Failed to add block to database: {0} ", block.Hash);
                _logger.LogError(result.Message);
                return Ok(result);
            }

            var delete_result = await ProcessDeleteBlock(validator);

            //if (!Controllers.ProductionMode)
            //{
            //    await Task.Run(() =>
            //    {
            //        validator.PrintBlock();
            //    });
            //}
            result.Status = ResultStatusCodes.SUCCESS;
            return Ok(result);
        }
        catch (LicenseValidationException e)
        {
            result.Status = ResultStatusCodes.LICENSE_VIOLATION;
            result.Message = $"License validation failed: {e.Message}";
            _logger.LogError(e, result.Message);
            return Ok(result);
        }

        catch (Exception e)
        {
            result.Status = ResultStatusCodes.EXCEPTION;
            result.Message = $"Broadcast of message block failed: {e.Message}";
            _logger.LogError(e, result.Message);
            return Ok(result);
        }
    }

    [HttpPost("Scan")]
    public async Task<ActionResult<ScanMsgResult>> Scan([FromBody] ScanMsgParams scanParams)
    {
        var result = new ScanMsgResult();
        try
        {
            //var result = await _nodeService.ScanAsync(scanParams);
            var db_result = await _blockDb.Scan(scanParams.LastScannedHeight, scanParams.ChatPubKey);
            if (db_result is null || db_result.Block is null)
            {
                result.Status = ResultStatusCodes.NOT_FOUND;
                result.Message = "No blocks found";
                return Ok(result);
            }

            result.Status = ResultStatusCodes.SUCCESS;
            result.Block = db_result.Block;
            result.HasMore = db_result.HasMore;
            return Ok(result);
        }
        catch (Exception e)
        {
            result.Status = ResultStatusCodes.EXCEPTION;
            result.Message = $"Scan for message blocks failed: {e.Message}";
            _logger.LogError(e, result.Message);
            return Ok(result);
        }
    }

    private async Task<bool> ProcessDeleteBlock(MsgBlockValidator block)
    {
        if (!block.DeleteFlag)
            return true;

        // to do - validate the original block

        // first let's see if the message is written into the chat block itself
        bool delete_result = await _chatDb.ResetBlockDataAsync(block.DeletionHash);
        if (!delete_result)
        {
            delete_result = await _blockDb.ResetBlockDataAsync(block.DeletionHash);
        }

        return delete_result;
    }
}

