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

using MystSafe.Shared.CryptoLicense;

namespace MystSafe.Client.CryptoLicense;

public interface IWalletDatabase
{
    Task<long> GetHeight(string accountId); // get the timestamp of the latest local tx
    Task<bool> AddTx(TxLocalRecord tx); // add new tx to the local storage
    Task<bool> AddOutput(OutputLocalRecord tx); // 
    //Task<string> IsMyKeyImageUsed(string keyImage); // 
    Task<List<OutputLocalRecord>> GetAllUnspentOutputs(string accountId, int tokenId);
    Task<bool> UpdateSpentOutput(string stealthAddress, string spendTxPubKey); // 
    //Task<List<Transaction>> LoadTransactions(); // load all the wallet's Tx from the local storage
    Task UpdateHeight(long newHeight, string accountId); // update the latest height

    Task<TxLocalRecord> GetTx(string txPubKey);

    Task<List<string>> GetRecentTxPublicKeys(TimeSpan timeSpan);
}