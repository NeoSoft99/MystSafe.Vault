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
using Newtonsoft.Json;

namespace MystSafe.Backend.Relay;

[ApiController]
[Route("api/[controller]")]
public class SecretBlockController : ControllerBase
{
    //private readonly SecretBlockNodeServiceClient _nodeService;
    private readonly ILogger<SecretBlockController> _logger; // For logging purposes
    private readonly SecretBlockDb _blockDb;
    //private readonly LicenseBlockDb _licenseDb;
    private readonly DatabaseService _databaseService;
    private readonly BackendLicenseValidationService _licenseProofValidationService;

    public SecretBlockController(ILogger<SecretBlockController> logger, SecretBlockDb blockDb, 
        DatabaseService databaseService, BackendLicenseValidationService licenseProofValidationService)
    {
        _logger = logger;
        _blockDb = blockDb;
        //_licenseDb = licenseDb;
        _databaseService = databaseService;
        _licenseProofValidationService = licenseProofValidationService;
    }

    // Adjusted to include custom exception handling as per the provided example
    [HttpPost("Broadcast")]
    public async Task<ActionResult<SecretBlockBroadcastResult>> Broadcast([FromBody] SecretBlock block)
    {
        var result = new SecretBlockBroadcastResult();
        try
        {
            if (_databaseService.Network == Networks.devnet)
            {
                var requestJson = JsonConvert.SerializeObject(block, Formatting.Indented);
                _logger.LogInformation($"Request JSON: {requestJson}");
            }
            
            if (block.Network != _databaseService.Network)
            {
                result.Status = ResultStatusCodes.WRONG_NETWORK;
                result.Message = string.Format("Wrong network: {0} ", Networks.ToString(block.Network));
                _logger.LogError(result.Message);
                return Ok(result);
            }

            if (await _blockDb.ExistsAsync(block.Hash))
            {
                result.Status = ResultStatusCodes.BLOCK_ALREADY_EXISTS;
                result.Message = string.Format("Block already exists in the database: {0} ", block.Hash);
                _logger.LogError(result.Message);
                return Ok(result);
            }

            var validator = new SecretBlockValidator(block);
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
            
            if (_databaseService.Network == Networks.devnet)
            {
                var responseJson = JsonConvert.SerializeObject(result, Formatting.Indented);
                _logger.LogInformation($"Response JSON: {responseJson}");
            }

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
            result.Message = $"Broadcast of secret block failed: {e.Message}";
            _logger.LogError(e, result.Message);
            return Ok(result);
        }
    }

    [HttpPost("Scan")]
    public async Task<ActionResult<SecretBlockScanResult>> Scan([FromBody] ScanSecretBlockParams scanParams)
    {
        var result = new SecretBlockScanResult();
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
            result.Message = $"Scan for secret blocks failed: {e.Message}";
            _logger.LogError(e, result.Message);
            return Ok(result);
        }
    }

    // Example for a GET operation with custom exception result
    [HttpPost("GetByHash")]
    public async Task<ActionResult<SecretBlockResult>> GetSecretBlock([FromBody] SecretBlockParams getParams)
    {
        var result = new SecretBlockResult();
        try
        {
            //var result = await _nodeService.GetSecretBlockAsync(getParams);
            var block = await _blockDb.GetByHash(getParams.BlockHash);
            if (block == null)
            {
                result.Status = ResultStatusCodes.NOT_FOUND;
                result.Message = "Block not found";
            }
            else if (block.Expiration > 0 &&
                     UnixDateTime.AddSeconds(block.TimeStamp, block.Expiration) <= UnixDateTime.Now)
            {
                result.Status = ResultStatusCodes.EXPIRED;
                result.Message = "Block is expired";
            }
            else
            {
                result.Status = ResultStatusCodes.SUCCESS;
                result.Message = string.Empty;
                result.Block = block;
            }

            return Ok(result);
        }
        catch (Exception e)
        {
            result.Status = ResultStatusCodes.EXCEPTION;
            result.Message = $"Retrieval of secret block failed: {e.Message}";
            _logger.LogError(e, result.Message);
            return Ok(result);
        }
    }

    private async Task<bool> ProcessDeleteBlock(SecretBlockValidator block)
    {
        if (!block.DeleteFlag)
            return true;

        // to do - validate the original block

        bool delete_result = await _blockDb.ResetBlockDataAsync(block.PrevHash);

        return delete_result;
    }
}

