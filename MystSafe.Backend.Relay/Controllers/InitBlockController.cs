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

//using MystSafe.Shared.Crypto.Addresses;

namespace MystSafe.Backend.Relay
{
    [ApiController]
    [Route("api/[controller]")]
    public class InitBlockController : ControllerBase
    {
        //private readonly InitBlockNodeServiceClient _nodeService;
        private readonly ILogger<InitBlockController> _logger;
        private readonly ChatBlockDb _blockDb;
        //private readonly LicenseBlockDb _licenseDb;
        private readonly DatabaseService _databaseService;
        private readonly BackendLicenseValidationService _licenseProofValidationService;

        public InitBlockController(ILogger<InitBlockController> logger, ChatBlockDb blockDb, DatabaseService databaseService, BackendLicenseValidationService licenseProofValidationService)
        {
            _logger = logger;
            _blockDb = blockDb;
            //_licenseDb = licenseDb;
            _databaseService = databaseService;
            _licenseProofValidationService = licenseProofValidationService;
        }

        [HttpPost("Broadcast")]
        public async Task<ActionResult<AddInitBlockStatus>> Broadcast([FromBody] InitBlock block)
        {
            var result = new AddInitBlockStatus();
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

                var validator = new ChatBlockValidator(block);

                validator.ServerValidate();

                await _licenseProofValidationService.ValidateLicense(validator);

                var database_result = await _blockDb.Add(block);
                if (!database_result)
                {
                    result.Status = ResultStatusCodes.BLOCK_FAILED_TO_ADD_TO_DATABASE;
                    result.Message = string.Format("Failed to add block to database: {0} ", block.Hash);
                    Console.WriteLine(result.Message);
                    return Ok(result);
                }

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
            catch (Exception e)
            {
                result.Status = ResultStatusCodes.EXCEPTION;
                result.Message = $"Broadcast of chat block failed: {e.Message}";
                _logger.LogError(e, result.Message);
                return Ok(result);
            }
        }

        [HttpPost("Scan")]
        public async Task<ActionResult<ScanChatResult>> Scan([FromBody] ChatScanParams scanParams)
        {
            var result = new ScanChatResult();
            result.Message = string.Empty;
            try
            {
                //var result = await _nodeService.ScanAsync(scanParams);
                const int PAGE_SIZE = 100;

                var blocks = await _blockDb.Scan(scanParams.LastScannedDateTime, PAGE_SIZE);
                if (blocks != null && blocks.Count > 0)
                {
                    foreach (var block in blocks)
                    {
                        var validator = new ChatBlockValidator(block);
                        var stealth_address = StealthAddress.Restore(scanParams.ScanKey, block.ChatPubkey, validator.CalculateBlockSalt());
                        if (stealth_address.IsMatch(block.RecipientStealthAddress))
                        {
                            result.Block = block;
                            result.Status = ResultStatusCodes.SUCCESS;
                            result.IsSelf = false;
                            return Ok(result);
                        }
                        else if (stealth_address.IsMatch(block.SenderStealthAddress))
                        {
                            result.Block = block;
                            result.Status = ResultStatusCodes.SUCCESS;
                            result.IsSelf = true;
                            return Ok(result);
                        }
                    }
                }
                result.Status = ResultStatusCodes.NOT_FOUND;
                result.Message = "BLock not found";
                return Ok(result);
            }
            catch (Exception e)
            {
                result.Status = ResultStatusCodes.EXCEPTION;
                result.Message = $"Scan for chat blocks failed: {e.Message}";
                _logger.LogError(e, result.Message);
                return Ok(result);
            }
        }
    }
}
