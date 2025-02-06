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
//using MystSafe.Backend.CryptoLicense;
using MystSafe.Backend.DB;
using MystSafe.Shared.Common;
using MystSafe.Shared.Crypto;

namespace MystSafe.Backend.Relay;

[ApiController]
[Route("api/[controller]")]
public class ContactBlockController : ControllerBase
{
    private readonly ILogger<ContactBlockController> _logger;
    private readonly ContactBlockDb _blockDb;
    private readonly DatabaseService _databaseService;
    private readonly BackendLicenseValidationService _licenseProofValidationService;

    public ContactBlockController(
        ILogger<ContactBlockController> logger, 
        ContactBlockDb blockDb,
        //LicenseBlockDb licenseDb, 
        DatabaseService databaseService,
        BackendLicenseValidationService licenseProofValidationService)
    {
        _logger = logger;
        _blockDb = blockDb;
        //_licenseDb = licenseDb;
        _databaseService = databaseService;
        _licenseProofValidationService = licenseProofValidationService;
        //_validator = validator;
    }

    [HttpPost("Broadcast")]
    public async Task<ActionResult<BroadcastContactResult>> Broadcast([FromBody] ContactBlock block)
    {
        var result = new BroadcastContactResult();
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
                return Ok(result);
            }

            var validator = new ContactBlockValidator(block);
            validator.ServerValidate();
            
            await _licenseProofValidationService.ValidateLicense(validator);

            //first add to the DB synchroniously to be able to detect and reject echo broadcasts
            bool database_result = await _blockDb.Add(block);
            if (!database_result)
            {
                result.Status = ResultStatusCodes.BLOCK_FAILED_TO_ADD_TO_DATABASE;
                result.Message = string.Format("Failed to add block to database: {0} ", block.Hash);
                Console.WriteLine(result.Message);
                return Ok(result);
            }

            var delete_result = await ProcessDeleteBlock(validator);
            // TO DO - log error result

            result.Status = ResultStatusCodes.SUCCESS;
            //if (!Controllers.ProductionMode)
            //{
            //    await Task.Run(() =>
            //    {
            //        validator.PrintBlock();
            //    });
            //}
            return Ok(result);
        }
        catch (Exception e)
        {
            result.Status = ResultStatusCodes.EXCEPTION;
            result.Message = $"Broadcast of contact block failed: {e.Message}";
            _logger.LogError(e, result.Message);
            return Ok(result);
        }
    }

    [HttpPost("Scan")]
    public async Task<ActionResult<ScanContactResult>> Scan([FromBody] ScanContactParams scanParams)
    {
        var result = new ScanContactResult();
        try
        {
            //var result = await _nodeService.ScanAsync(scanParams);
            var blocks = await _blockDb.Scan(scanParams.LastScannedDateTime, scanParams.PageSize);
            if (blocks == null)
            {
                result.Status = ResultStatusCodes.NOT_FOUND;
                result.Message = string.Empty;
                return Ok(result);
            }

            result.Status = ResultStatusCodes.SUCCESS;
            result.Message = string.Empty;
            foreach (var block in blocks)
                result.Blocks.Add(block);
            return Ok(result);
        }
        catch (Exception e)
        {
            result.Status = ResultStatusCodes.EXCEPTION;
            result.Message = $"Scan for contact blocks failed: {e.Message}";
            _logger.LogError(e, result.Message);
            return Ok(result);
        }
    }

    private async Task<bool> ProcessDeleteBlock(ContactBlockValidator block)
    {
        if (!block.DeleteFlag)
            return true;

        // to do - validate the original block

        // only reset the data awhen the block is delete block (not update block), i.e no data in such a block
        if (!string.IsNullOrEmpty(block.InitData))
            return true;

        bool delete_result = await _blockDb.ResetBlockDataByPubKey(block.BlockPubKey);

        return delete_result;
    }
}
