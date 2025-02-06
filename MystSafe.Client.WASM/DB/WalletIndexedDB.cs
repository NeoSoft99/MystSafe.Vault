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

using IndexedDB.Blazor;
using MystSafe.Client.App;
using MystSafe.Client.CryptoLicense;
using MystSafe.Shared.Common;

namespace MystSafe.Client.App;

public class WalletIndexedDb : IWalletDatabase
{
    private readonly IIndexedDbFactory _DbFactory;

    public WalletIndexedDb(IIndexedDbFactory DbFactory)
    {
        _DbFactory = DbFactory;
    }
    
    // This method takes a TimeSpan parameter that allows you to configure the period (e.g., 24 hours)
    // and returns a list of transaction public keys created within that period.
    public async Task<List<string>> GetRecentTxPublicKeys(TimeSpan timeSpan)
    {
        var currentUnixTime = UnixDateTime.Now;
        var cutoffUnixTime = UnixDateTime.FromDateTime(DateTime.UtcNow.Subtract(timeSpan));

        using var db = await _DbFactory.Create<ClientDb>();
        var recentTxs = db.Transactions
            .Where(tx => tx.TimeStamp >= cutoffUnixTime)
            .Select(tx => tx.PubKey)
            .ToList();

        return recentTxs;
    }

    public async Task<TxLocalRecord> GetTx(string txPubKey)
    {
        using var db = await _DbFactory.Create<ClientDb>();
        var tx = db.Transactions.SingleOrDefault(x => x.PubKey == txPubKey);
        if (tx != null)
        {
            return tx;
        }

        return null;
    }

    public async Task<long> GetHeight(string accountId) // returns the max Tx timestamp
    {
        using var db = await _DbFactory.Create<ClientDb>();
        var walletRecord = db.Wallets.SingleOrDefault(x => x.AccountId == accountId);
        if (walletRecord != null)
        {
            return walletRecord.Height;
        }

        return 0;
    }

    public async Task UpdateHeight(long newHeight, string accountId)
    {
        using var db = await _DbFactory.Create<ClientDb>();
        var walletRecord = db.Wallets.SingleOrDefault(x => x.AccountId == accountId);
        if (walletRecord != null)
        {
            walletRecord.Height = newHeight;
        }
        else
        {
            db.Wallets.Add(new WalletLocalRecord() { Height = newHeight, AccountId = accountId });
        }

        await db.SaveChanges();
    }

    public async Task<bool> AddTx(TxLocalRecord txRecord) // add new tx to the local storage
    {
        try
        {
            using var db = await _DbFactory.Create<ClientDb>();
            var existingTx =
                db.Transactions.SingleOrDefault(x => x.PubKey == txRecord.PubKey);
            if (existingTx != null)
            {
                return false;
            }

            db.Transactions.Add(txRecord);
            await db.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task Delete(string txPubKey)
    {
        try
        {
            using var db = await _DbFactory.Create<ClientDb>();
            var txRecord = db.Transactions.SingleOrDefault(x => x.PubKey == txPubKey);
            if (txRecord != null)
            {
                db.Transactions.Remove(txRecord);
                await db.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    public async Task<bool> AddOutput(OutputLocalRecord outputRecord)
    {
        try
        {
            using var db = await _DbFactory.Create<ClientDb>();
            var existingOutput = db.Outputs.SingleOrDefault(x =>
                x.StealthAddress == outputRecord.StealthAddress); //&& x.TxPubKey == outputRecord.TxPubKey &&
                //x.AccountId == outputRecord.AccountId);
            if (existingOutput != null)
            {
                return false;
            }

            db.Outputs.Add(outputRecord);
            await db.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<List<OutputLocalRecord>> GetAllUnspentOutputs(string accountId, int tokenId)
    {
        using var db = await _DbFactory.Create<ClientDb>();
        return await Task.FromResult(
            db.Outputs.Where(output => !output.Spent && output.AccountId == accountId && output.TokenId == tokenId).ToList());
    }

    public async Task<bool> UpdateSpentOutput(string stealthAddress, string spentTxPubKey)
    {
        using (var db = await _DbFactory.Create<ClientDb>())
        {
            var output =
                db.Outputs.SingleOrDefault(o => o.StealthAddress == stealthAddress);
            if (output != null)
            {
                output.Spent = true;
                output.SpendTxPubKey = spentTxPubKey;
                await db.SaveChanges();
                return true;
            }

            return false;
        }
    }
}
