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

namespace MystSafe.Shared.CryptoLicense;

public class BaseLicenseResponse
{
    public int ResultCode { get; set; }
    public string ResultMessage { get; set; }

    public new string ToString()
    {
        return "ResultCode" + ResultCode + " ResultMessage: " + ResultMessage;
    }
}

public class LicenseTransactionValidationResponse: BaseLicenseResponse
{
    public TxDTO Transaction { get; set; }
}

public class GetTransactionsResponse : BaseLicenseResponse
{
    public List<TxDTO> Transactions { get; set; }
    public long NewLastHeight { get; set; }
    public bool HasMore { get; set; }
}

public class FetchDecoyOutputsResponse: BaseLicenseResponse
{
    public List<OutputDTO> Outputs { get; set; }
}

public class FetchTransactionsResponse: BaseLicenseResponse
{
    public List<TxDTO> Transactions { get; set; }
}

public class SendTransactionResponse: BaseLicenseResponse
{
    public Transaction Tx { get; set; }
}

public class GetRecentTransactionResponse: BaseLicenseResponse
{
    public TxDTO Transaction { get; set; }
}

public class AddTransactionResponse: BaseLicenseResponse
{
    public TxDTO Transaction { get; set; }
}


