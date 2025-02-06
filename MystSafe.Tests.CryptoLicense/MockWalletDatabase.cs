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

    using MystSafe.Client.CryptoLicense;
    using MystSafe.Shared.Common;

    public class MockWalletDatabase : IWalletDatabase
    {
        private readonly Dictionary<string, long> _heights = new Dictionary<string, long>();
        private readonly List<TxLocalRecord> _transactions = new List<TxLocalRecord>();
        private readonly List<OutputLocalRecord> _outputs = new List<OutputLocalRecord>();

        public async Task<long> GetHeight(string accountId)
        {
            _heights.TryGetValue(accountId, out var height);
            return await Task.FromResult(height);
        }
        
        public async Task<List<string>> GetRecentTxPublicKeys(TimeSpan timeSpan)
        {
            var cutoffUnixTime = UnixDateTime.FromDateTime(DateTime.UtcNow.Subtract(timeSpan));
            var recentTxs = _transactions
                .Where(tx => tx.TimeStamp >= cutoffUnixTime)
                .Select(tx => tx.PubKey)
                .ToList();

            return await Task.FromResult(recentTxs);
        }

        public async Task Delete(string txPubKey)
        {
            var txRecord = _transactions.FirstOrDefault(tx => tx.PubKey == txPubKey);
            if (txRecord != null)
            {
                _transactions.Remove(txRecord);
            }

            await Task.CompletedTask;
        }

        public async Task<bool> AddTx(TxLocalRecord txRecord) // add new tx to the local storage
        {
            var tx = _transactions.FirstOrDefault(tx => tx.PubKey == txRecord.PubKey);
            if (tx == null)
            {
                _transactions.Add(txRecord);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<TxLocalRecord> GetTx(string txPubKey)
        {
            var result = _transactions.FirstOrDefault(tx => tx.PubKey == txPubKey);
            return await Task.FromResult(result);
        }

        public async Task<bool> AddOutput(OutputLocalRecord outputRecord)
        {
            var existingOutput = _outputs.FirstOrDefault(output => output.StealthAddress == outputRecord.StealthAddress);
            if (existingOutput == null)
            {
                _outputs.Add(outputRecord);
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task UpdateHeight(long newHeight, string accountId) // update the latest height
        {
            _heights[accountId] = newHeight;
        }

        public async Task<List<OutputLocalRecord>> GetAllUnspentOutputs(string accountId, int tokenId)
        {
            var unspentOutputs = _outputs.Where(output => !output.Spent && output.AccountId == accountId && output.TokenId == tokenId).ToList();
            return await Task.FromResult(unspentOutputs);
        }

        public async Task<bool> UpdateSpentOutput(string stealthAddress, string spendTxPubKey)
        {
            var output = _outputs.FirstOrDefault(o => o.StealthAddress == stealthAddress);
            if (output != null)
            {
                output.Spent = true;
                output.SpendTxPubKey = spendTxPubKey;
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }
    }
