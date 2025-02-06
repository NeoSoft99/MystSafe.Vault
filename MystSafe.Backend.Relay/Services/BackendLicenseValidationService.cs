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

using MystSafe.Backend.DB;
using MystSafe.Shared.Common;
using MystSafe.Shared.CryptoLicense;

namespace MystSafe.Backend.Relay;

public class BackendLicenseValidationService
{
    private readonly SecretBlockDb _secretDB;
    private readonly ContactBlockDb _contactDB;
    private readonly ChatBlockDb _chatDB;
    private readonly ILicenseRelayClientService _licenseRelay;

    // those services are injected by DI 
    public BackendLicenseValidationService(SecretBlockDb secretDB, ContactBlockDb contactDB, ChatBlockDb chatDB, ILicenseRelayClientService licenseRelay)
    {
        _secretDB = secretDB;
        _contactDB = contactDB;
        _chatDB = chatDB;
        _licenseRelay = licenseRelay;
    }

    public async Task ValidateLicense(ILicensableBlock block)
    {
        try
        {
            if (block.LicenseType == Constants.FREE_LICENSE_TYPE)
            {
                return;
            }

            if (string.IsNullOrEmpty(block.License))
                throw new LicenseValidationException("No license block hash.");
            
            var double_spend_found = await _secretDB.ExistsByLicenseAsync(block.License);
            if (double_spend_found)
                throw new LicenseValidationException("This license is already in use.");
            
            double_spend_found = await _contactDB.ExistsByLicenseAsync(block.License);
            if (double_spend_found)
                throw new LicenseValidationException("This license is already in use.");
            
            double_spend_found = await _chatDB.ExistsByLicenseAsync(block.License);
            if (double_spend_found)
                throw new LicenseValidationException("This license is already in use.");

            var valid_result = await _licenseRelay.ValidateLicenseTransaction(block.License);
            if (valid_result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new LicenseValidationException(valid_result.ToString());

            var txDTO = valid_result.Transaction;
            if (txDTO.Fee < block.CalculateFeeAmount())
                throw new LicenseValidationException("Wrong fee amount");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new LicenseValidationException("Unknown error: " + e.Message);
        }
    }
}


