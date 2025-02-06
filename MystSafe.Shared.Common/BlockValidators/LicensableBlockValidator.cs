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


namespace MystSafe.Shared.Common;


public abstract class LicensableBlockValidator<BlockType> : PoWBlockValidator<BlockType>, ILicensableBlock
{
    public string License { get; set; }
    public int LicenseType { get; set; }

    public LicensableBlockValidator(BlockType block)
    {

    }

    public LicensableBlockValidator()
    {
    }
    
    // returns size in bytes
    public int DataSizeInBytes()
    {
        if (string.IsNullOrEmpty(BlockData))
        {
            return 0;
        }

        // Calculate the effective length of the Base64 string without padding
        int effectiveLength = BlockData.TrimEnd('=').Length;

        // Calculate the size of the original data
        int dataSize = (effectiveLength * 3) / 4;

        return dataSize;
    }
    
    /*
    protected static int CalculateBufferSizeInBytes(string base64String)
    {
        if (string.IsNullOrEmpty(base64String))
        {
            return 0;
        }

        // Calculate the effective length of the Base64 string without padding
        int effectiveLength = base64String.TrimEnd('=').Length;

        // Calculate the size of the original data
        int dataSize = (effectiveLength * 3) / 4;

        return dataSize;
    }
    */


    public void ValidateLicenseType()
    {
        if ((LicenseType == Constants.FREE_LICENSE_TYPE && Difficulty != Constants.FREE_NETWORK_DIFFICULTY) ||
            (LicenseType != Constants.FREE_LICENSE_TYPE && Difficulty != Constants.LICENSED_NETWORK_DIFFICULTY))
            throw new ApplicationException("Incorrect network difficulty");

        if (LicenseType == Constants.FREE_LICENSE_TYPE && BlockData.Length > Constants.MAX_FREE_SECRET_SIZE)
            throw new LicenseValidationException(LicenseValidationException.LICENSE_VALIDATION_ERROR_DATA_SIZE_MESSAGE);
    }

    public virtual void ValidateRetention()
    {
        var retention_interval = UnixTimeInterval.FromRetentionInterval(Network, typeof(BlockType));
        var deletion_threshold = UnixDateTime.DeletionThreshold(retention_interval);
        if (TimeStamp < deletion_threshold && LicenseType == Constants.FREE_LICENSE_TYPE)
            throw new BlockExpiredWithNoLicenseException();
    }
    
    public decimal CalculateFeeAmount()
    {
        const int BytesPerKB = 1024;

        // Convert bytes to KB (fractional values possible)
        decimal dataSizeInKB = (decimal)DataSizeInBytes() / BytesPerKB;

        // Calculate the fee amount (license tokens), rounding up to the nearest whole number
        decimal feeAmount = Math.Ceiling(dataSizeInKB);

        // Ensure the fee is at least 1 token
        return feeAmount < 1 ? 1 : feeAmount;
    }

    public override void PrintBlock()
    {
        base.PrintBlock();
        Console.WriteLine("License                 : {0}", this.License);
        Console.WriteLine("License Type            : {0}", this.LicenseType);
    }

}


