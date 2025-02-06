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
using Microsoft.Extensions.Logging;
using MoneroRing.Crypto;
using MoneroSharp;
using MystSafe.Shared.Common;
using MystSafe.Shared.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MystSafe.Client.CryptoLicense;

public class Wallet
{
    private readonly ILogger<Wallet> _logger;
    private readonly ILicenseRelayClientService _licenseRelayClientService;
    private readonly IWalletDatabase _localWalletStorage;
    
    private int _network;
    private string _accountId;
    protected UserAddress _userAddress;

    private decimal _licenseBalance = 0;

    public decimal LicenseBalance
    {
        get { return _licenseBalance; }
    }

    public Wallet(ILogger<Wallet> logger, ILicenseRelayClientService licenseRelayClientService, IWalletDatabase localWalletStorage)
    {
        _logger = logger;
        _licenseRelayClientService = licenseRelayClientService;
        _localWalletStorage = localWalletStorage;
    }
    
    public async Task<long> GetHeightAsync()
    {
        return await _localWalletStorage.GetHeight(_accountId);
    }
    
    public async Task<decimal> GetBalanceAsync(int tokenId = Tokens.LICENSE_TOKEN)
    {
            var unspentOutputs = await _localWalletStorage.GetAllUnspentOutputs(_accountId, tokenId);
            var balance = unspentOutputs.Sum(output => output.Amount);
            if (tokenId == Tokens.LICENSE_TOKEN)
                _licenseBalance = balance;
            return balance;

    }
    
    public bool Initialized => _userAddress != null;

    public string Address => _userAddress.ToString(); // the public address of the wallet
    public string AccountId => _accountId; // the public address of the wallet
    public IWalletDatabase Storage => _localWalletStorage; // 

    // initializes the wallet 
    //public async Task Init(UserAddress userAddress, string accountId, long lastTxTimeStamp, int network) // all inputs should be taken from the account data
    public async Task Init(UserAddress userAddress, string accountId, int network) // all inputs should be taken from the account data
    {
        _userAddress = userAddress;
        //Height = lastTxTimeStamp;
        if (UserAddress.ConvertMystSafeNetworkToMonero(_network) != userAddress.Network)
            throw new ArgumentException("Addess network mismatch");
        _userAddress = userAddress;
        _accountId = accountId;
        _network = network;
        await GetBalanceAsync();
    }
 
    public async Task Sync()
    {
        try
        {
            //long lastHeight = 
            int limit = 100; // Define a reasonable limit
            bool moreTransactions = true;
            
            while (moreTransactions)
            {
                var get_tx_request = new GetTransactionsSinceRequest
                {
                    LastHeight = await GetHeightAsync(), //lastHeight,
                    Limit = limit
                };

                var response = await _licenseRelayClientService.GetTransactionsSince(get_tx_request);

                if (response.ResultCode == CryptoLicenseResultCodes.SUCCESS && response.Transactions != null)
                {
                    foreach (TxDTO tx in response.Transactions)
                    {
                        await UpdateLocalStorage(tx);
                        _licenseBalance = await GetBalanceAsync();
                        
                        if (tx.TimeStamp > await GetHeightAsync())
                        {
                            //Height = tx.TimeStamp;
                            await _localWalletStorage.UpdateHeight(tx.TimeStamp, _accountId);

                        }
                    }
                    //lastHeight = response.NewLastHeight;
                    moreTransactions = response.HasMore;
                }
                else
                {
                    _logger.LogWarning("No new transactions found or error occurred.");
                    moreTransactions = false;
                }

            }

        }
        catch (Exception e)
        {
            _logger.LogError("Sync failed: {0}", e.Message);
        }
    }

    private async Task<bool> UpdateLocalStorage(TxDTO tx)
    {
        bool add_tx = false;
        
        // to do: - separate by tokenId; now assuming only single token id per tx
        foreach (OutputDTO output in tx.Outputs)
        {
            if (Output.IsSentToMe(output.StealthAddress, _userAddress, tx.PubKey, output.Index))
            {
                var outputRecord = OutputLocalRecord.FromDTO(output);
                outputRecord.TxPubKey = tx.PubKey;
                outputRecord.Spent = false;
                outputRecord.SpendTxPubKey = String.Empty;
                outputRecord.AccountId = _accountId;
                bool add_output_result = await _localWalletStorage.AddOutput(outputRecord);
                if (!add_output_result)
                    //return false;
                    continue;
                add_tx = true;
            }
        }
        
        // 2. check inputs (to compare key image and find if any of them were spent.
        // When your wallet scans the blockchain, it not only looks for new outputs that belong to you
        // but also checks for key images that match the outputs you own.
        // If it finds a matching key image in a transaction, it knows that the corresponding output has been spent
        
        if (tx is { Fee: > 0, Inputs.Count: > 0 }) // nothing to check if this is mining tx
        {
            // to do: - separate by tokenId; now assuming only single token id per tx
            int tokenId = tx.Inputs[0].TokenId;
            
            var all_outputs = await _localWalletStorage.GetAllUnspentOutputs(_accountId, tokenId);
            foreach (var output_record in all_outputs)
            {
                var output = output_record.ToOutput();
                // I = a * Hp(P)
                var P = PubKey.FromBase58String(output.StealthAddress);
                var x = output.DeriveOutputSpendPrivateKey(_userAddress.ScanKey, _userAddress.ReadKey);
                var I = new byte[32];
                RingSig.generate_key_image(P.ToBytes(), x.ToBytes(), I);
                var key_image = Codecs.FromBytesToBase58(I);

                bool containsKeyImage = tx.Inputs.Any(input => input.KeyImage == key_image);
                if (containsKeyImage)
                {
                    bool update_output_result = await _localWalletStorage.UpdateSpentOutput(output.StealthAddress, tx.PubKey);
                    if (!update_output_result)
                    {
                        //return false;
                        throw new Exception("Failed to update spent output");
                    }

                    add_tx = true;
                }
            }
        }

        if (add_tx)
        {
            var txRecord = TxLocalRecord.FromDTO(tx); //ConvertToTransaction(txDTO);
            bool add_tx_result = await _localWalletStorage.AddTx(txRecord);
            return add_tx_result;
        }

        return true;
    }
    
    public async Task<SendTransactionResponse> SendTransfer(decimal amount, decimal fee, string recipientUserAddress,
        int tokenId)
    {
        var result = new SendTransactionResponse();
        try
        {
            if (fee <= 0)
                throw new ArgumentException("Fee amount must be greater than zero");

            if (await this.GetBalanceAsync() < amount + fee)
                throw new ArgumentException("Insufficient wallet balance.");

            var tx = await GenerateTransferTransaction(recipientUserAddress, amount, fee, tokenId);
            var process_result = await ProcessNewTransaction(tx);
            result.Tx = tx;
            result.ResultCode = process_result.ResultCode;
            result.ResultMessage = process_result.ResultMessage;
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e);
            result.ResultCode = CryptoLicenseResultCodes.VALIDATION_ERROR;
            result.ResultMessage = e.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            result.ResultCode = CryptoLicenseResultCodes.EXCEPTION;
            result.ResultMessage = e.ToString();
        }

        return result;
    }

    public async Task<SendTransactionResponse> SendLicenseFee(decimal fee)
    {
        var result = new SendTransactionResponse();
        try
        {
            if (fee <= 0)
                throw new ArgumentException("Fee amount must be greater than zero");
            
            if (await this.GetBalanceAsync() < fee)
                throw new ArgumentException("Insufficient wallet balance.");

            var tx = await GenerateTransferTransaction(string.Empty, 0, fee, Tokens.LICENSE_TOKEN);
            var process_result = await ProcessNewTransaction(tx);
            result.Tx = tx;
            result.ResultCode = process_result.ResultCode;
            result.ResultMessage = process_result.ResultMessage;
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e);
            result.ResultCode = CryptoLicenseResultCodes.VALIDATION_ERROR;
            result.ResultMessage = e.ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            result.ResultCode = CryptoLicenseResultCodes.EXCEPTION;
            result.ResultMessage = e.ToString();
        }
        return result;
    }

    public async Task<BaseLicenseResponse> ProcessNewTransaction(Transaction tx)
    {
        var txDTO = tx.ToDTO();
            
        var addResult = await _licenseRelayClientService.AddTransaction(txDTO);

        if (addResult.ResultCode != ResultStatusCodes.SUCCESS)
            return addResult;

        var update_storage_result = await UpdateLocalStorage(txDTO);
        _licenseBalance = await GetBalanceAsync();
        if (!update_storage_result)
        {
            throw new Exception("Transaction has been processed by could not update local wallet storage");
        }

        return addResult;
    }
    
    // 
    public async Task<Transaction> GenerateTransferTransaction(string recipient_address, decimal transfer_amount,
        decimal fee, int tokenId)
    {
        var tx = new Transaction
        {
            Fee = fee,
            TimeStamp = UnixDateTime.Now,
            //TokenId = tokenId,
            //FeeTarget = feeTarget
        };
        var txKeyPair = KeyPair.GenerateRandom();
        tx.PubKey = txKeyPair.PublicKey.ToString();

        // select enough unspent outputs to cover total Tx amount and "convert" them into inputs
        var unspent_outputs = await SelectUnspentOutputs(transfer_amount + tx.Fee, tokenId);

        // get up to MAX_RING_SIZE decoy outputs for each unspent output
        var decoy_rings = await BuildOutputRings(unspent_outputs, GlobalConstants.MAX_RING_SIZE);

        uint input_index = 0;
        foreach (var key in decoy_rings.Keys)
        {
            var decoy_outputs = decoy_rings[key];
            var real_unspent_output = unspent_outputs.FirstOrDefault(output =>
                output.Amount == key.amount && output.TxPubKey == key.txPubKey && output.TokenId == tokenId);
            if (real_unspent_output == null)
            {
                throw new Exception("Real unspent output not found.");
            }

            SecKey x = real_unspent_output.DeriveOutputSpendPrivateKey(_userAddress.ScanKey, _userAddress.ReadKey);

            // Insert the real unspent output into the list of decoys at a random index (real_output_index)
            var (pub_keys, real_output_index) = InsertRealOutputAndBuildPubsArray(decoy_outputs, real_unspent_output);

            //var input = new Input(real_unspent_output.Amount, input_index, tokenId);
            //input.CreateRingSignature(pub_keys, pub_keys.Length, x, real_output_index, txKeyPair.PublicKey);
            //tx.Inputs.Insert((int)input_index, input);
            tx.AddInput(real_unspent_output.Amount, tokenId, input_index, pub_keys, x, real_output_index);
            input_index++;
        }

        // total Tx amount = transfer_amount + fee + change_amount = TotalInputsAmount
        var change_amount = tx.TotalInputsAmount - (transfer_amount + fee);
        if (change_amount > 0)
        {
            var denominated_change_outputs = Transaction.DecomposeAmount(change_amount);
            foreach (var output in denominated_change_outputs)
            {
                tx.AddOutput(_userAddress.ToString(), txKeyPair.PrivateKey, tokenId, output);
            }
        }

        if (transfer_amount != 0)
        {
            var denominated_transfer_outputs = Transaction.DecomposeAmount(transfer_amount);
            foreach (var output in denominated_transfer_outputs)
            {
                tx.AddOutput(recipient_address, txKeyPair.PrivateKey, tokenId, output);
            }
        }

        tx.Sign(txKeyPair.PrivateKey);

        return tx;
    }

    // builds the list of pub keys for ring signatures
    private (byte[][] pubs, int realIndex) InsertRealOutputAndBuildPubsArray(List<Output> decoys, Output realOutput)
    {
        var random = new Random();
        int realIndex = random.Next(decoys.Count + 1); // Random index to insert the real output
        decoys.Insert(realIndex, realOutput);
        
        byte[][] pub_keys = new byte[decoys.Count][];
        foreach (var output in decoys)
        {
            int index = decoys.IndexOf(output);
            pub_keys[index] = PubKey.FromBase58String(output.StealthAddress).ToBytes();
        }
        
        return (pub_keys, realIndex);
    }
    
    // build a ring of decoy outputs for each input amount
    public async Task<Dictionary<(decimal amount, string txPubKey), List<Output>>> BuildOutputRings(List<Output> unspentOutputs, int maxRingSize)
    {
        var result = new Dictionary<(decimal, string), List<Output>>();
        var outputAmounts = unspentOutputs.Select(output => output.Amount).ToList();

        // Request decoys for all the output amounts
        FetchDecoyOutputsRequest request = new FetchDecoyOutputsRequest()
        {
            OutputAmounts = outputAmounts,
            NumberOfOutputs = maxRingSize
        };
        var fetchDecoyResponses = await _licenseRelayClientService.FetchDecoyOutputs(request);

        foreach (var unspentOutput in unspentOutputs)
        {
            var decoys = new List<OutputDTO>();
            decoys.AddRange(fetchDecoyResponses.Outputs.Where(decoy => decoy.Amount == unspentOutput.Amount && decoy.StealthAddress != unspentOutput.StealthAddress));

            if (decoys.Count >= maxRingSize)
                decoys.RemoveAt(0);

            var outputList = decoys.Select(d => new Output(d.Amount, d.StealthAddress, d.Index, d.TxPubKey, d.TokenId)).ToList();

            result.Add((unspentOutput.Amount, unspentOutput.TxPubKey), outputList);
        }

        return result;
    }

    private async Task<List<Output>> SelectUnspentOutputs(decimal spend_amount, int tokenId)
    {
        // Get all unspent outputs and sort them descending by amount to use the largest outputs first
        var unspentOutputs = await _localWalletStorage.GetAllUnspentOutputs(_accountId, tokenId);
        var sortedOutputs = unspentOutputs.OrderBy(o => o.Amount).ToList();

        var matchedOutputs = new List<Output>();
        decimal total = 0;

        // First, try to find a single output that satisfies the spend amount
        foreach (var outputLocalRecord in sortedOutputs)
        {
            var output = new Output(outputLocalRecord.Amount, outputLocalRecord.StealthAddress, outputLocalRecord.Index, outputLocalRecord.TxPubKey, outputLocalRecord.TokenId);
            if (output.Amount >= spend_amount)
            {
                matchedOutputs.Add(output);
                total = output.Amount;
                break;
            }
        }

        // If no single output can satisfy the spend amount, sum smaller outputs
        if (total < spend_amount)
        {
            matchedOutputs.Clear();
            total = 0;

            foreach (var outputLocalRecord in sortedOutputs)
            {
                var output = new Output(outputLocalRecord.Amount, outputLocalRecord.StealthAddress, outputLocalRecord.Index, outputLocalRecord.TxPubKey, outputLocalRecord.TokenId);
                matchedOutputs.Add(output);
                total += output.Amount;

                // Stop if the collected outputs cover the spend amount
                if (total >= spend_amount)
                    break;
            }
        }

        // Check if the total collected is enough to cover the spend amount
        if (total >= spend_amount)
        {
            return matchedOutputs; // Return the list of outputs that match the criteria
        }
        else
        {
            throw new Exception("No matching outputs found"); // if no suitable combination is found
        }
    }
   
}




