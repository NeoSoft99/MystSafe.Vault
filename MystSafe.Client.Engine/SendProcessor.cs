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

using System.Diagnostics;
using MystSafe.Shared.Crypto;
using MystSafe.Shared.Common;
//using MystSafe.License.RingSig;
using System.Numerics;
using Microsoft.Extensions.Logging;
using MystSafe.Client.Base;
using MystSafe.Client.CryptoLicense;
using MystSafe.Shared.CryptoLicense;

namespace MystSafe.Client.Engine;


public class SendProcessor

{
    public readonly IContactDB _Contact_Db;
    public readonly IAccountDB _Account_Db;
    public readonly IChatOutDB _ChatOut_Db;
    public readonly IChatInDB _ChatIn_Db;
    public readonly IMessageDB _Message_Db;
    public readonly ISecretDB _Secret_Db;

    //private GrpcChannel channel;
    //private HttpClient httpClient;
   
    public const string SELF_TEST_SENDER_ACCOUNT = "TEST 1";
    public const string SELF_TEST_RECIPIENT_ACCOUNT = "TEST 2";
    public const string TEST_ACCOUNT_3 = "TEST 3";

    public readonly string BackEndURL;
    private readonly AccessControlService _accessControlService;
    public readonly ILogger<SendProcessor> Logger;
    public readonly ApiClientService _apiClientService;
    
    private readonly Wallet _wallet;

    public SendProcessor(
        string backendUrl,
        IAccountDB account_db,
        IContactDB contact_db,
        IChatInDB chatIn_db,
        IChatOutDB chatOut_db,
        IMessageDB message_db,
        ISecretDB secret_db,
        AccessControlService accessControlService,
        ILogger<SendProcessor> logger,
        ApiClientService apiClientService,
        Wallet wallet)
    {
        _apiClientService = apiClientService;
         _accessControlService = accessControlService;

        _Account_Db = account_db;
        _Contact_Db = contact_db;
        _ChatIn_Db = chatIn_db;
        _ChatOut_Db = chatOut_db;
        _Message_Db = message_db;
        _Secret_Db = secret_db;

     
        BackEndURL = backendUrl;
        Logger = logger;
        _wallet = wallet;
    }

    #region Accounts
    
    public string GetTransferData(Account account)
    {
        // Ensure the account name is no more than 20 characters.
        var name = account.NickName.Length > ClientConstants.MAX_ACCOUNT_NICKNAME_LENGTH_BYTES ? account.NickName.Substring(0, ClientConstants.MAX_ACCOUNT_NICKNAME_LENGTH_BYTES) : account.NickName;
        // Format the data as: securityPhrase|accountName
        return $"{account.Mnemonic}|{name}";
    }
    
    public async Task<Account> CreateNewAccount(string nickname, int network)
    {
        ValidateNickname(nickname);
        var account = new Account();

        Task<UserAddress> generateMnemonicTask = Task.Run(() => UserAddress.GenerateFromMnemonic(network));
        account.CurrentAddress = await generateMnemonicTask;
        //account.CurrentAddress = UserAddress.GenerateFromMnemonic();

        account.Mnemonic = account.CurrentAddress.Mnemonic12String;

        account.Created = UnixDateTime.Now;
        account.LastScannedSecretBlock = account.Created;
        account.LastScannedContactBlock = account.Created;

        await InitializeAccount(account, nickname, network);

        return account;
    }

    public async Task<Account> RestoreAccount(string mnemonic, string nickname, int network)
    {
        ValidateNickname(nickname);
        var account = new Account();

        Task<UserAddress> restoreMnemonicTask = Task.Run(() => UserAddress.RestoreFromMnemonic(mnemonic, network));
        account.CurrentAddress = await restoreMnemonicTask;
        //account.CurrentAddress = UserAddress.RestoreFromMnemonic(mnemonic);

        account.Mnemonic = mnemonic;
    
        account.Created = UnixDateTime.Now;
        account.LastScannedContactBlock = 2;
        account.LastScannedSecretBlock = 2;

        await InitializeAccount(account, nickname, network);

        //await LookForLicense(account);

        await account.CurrentWallet.Sync();

        await LookForSecrets(account);

        await LookForNewContactRequests(account);

        return account;
    }

    public void ValidateNickname(string nickname)
    {
        if (string.IsNullOrWhiteSpace(nickname))
            throw new ApplicationException("Please specify nickname");

        if (nickname.Length > ClientConstants.MAX_ACCOUNT_NICKNAME_LENGTH_BYTES)
            throw new ApplicationException(string.Format("Nickname length cannot exceed {0} characters", ClientConstants.MAX_ACCOUNT_NICKNAME_LENGTH_BYTES));
    }

    // this is used to do common initialization of a new account, either brand new or restored from the network
    private async Task InitializeAccount(Account account, string nickname, int network)
    {
        account.Id = Guid.NewGuid().ToString();
        account.Network = network; //Networks.FromString(network);
        account.NickName = nickname;
        account.PasswordHash = string.Empty;
        account.LastUpdated = account.Created;

        //account.LicensePrivateKey = string.Empty;
        //account.LicensePublicKey = string.Empty;

        var auth_init_result = await _accessControlService.InitializeNewAccount(account.Id);
        account.LocalAuthnType = auth_init_result.LocalAuthnType;
        account.LocalEncryptionKey = auth_init_result.LocalEncryptionKey ?? throw new Exception("Could not generate encryption key");

        //account.MasterLicensePrivateKey = string.Empty;
        //account.MasterLicensePubKey = string.Empty;
        account.Rewards = "0";

        account.CurrentWallet = _wallet;
        account.CurrentWallet.Init(account.CurrentAddress, account.Id, account.Network);

        await _Account_Db.Add(account);
    }

    //public Account GetRecentAccount()
    //{
    //    Task<Account> account_result = Task.Run(() => GetRecentAccountAsync());
    //    var account = account_result.Result;
    //    return account;
    //}

    // this is to retrieve the existing account from the local browser database
    //public async Task<Account> GetRecentAccountAsync()
    public class AccountLookupResult
    {
        public int ResultCode { get; set; }
        public string? ResultMessage { get; set; }
        public Account? ResultAccount { get; set; }
    }


    public async Task<AccountLookupResult> GetRecentAccountAsync()
    {
        var result = new AccountLookupResult();
        try
        {
            var account_authn_data = await _Account_Db.GetMostRecentlyUsedAccountAuthnData();
            //ClientSideLogger.Logger.LogInformation("account_authn_data.ResultCode: " + account_authn_data.ResultCode.ToString());
            if (account_authn_data.ResultCode == ResultStatusCodes.SUCCESS)
            {
                await _accessControlService.InitializeExistingAccount(account_authn_data);

                result.ResultCode = ResultStatusCodes.SUCCESS;
                result.ResultAccount = await _Account_Db.RetrieveAccount(account_authn_data.AccountId);
                result.ResultAccount.CurrentWallet = _wallet;
                await result.ResultAccount.CurrentWallet.Init(result.ResultAccount.CurrentAddress, result.ResultAccount.Id, result.ResultAccount.Network);
                return result;
            }
            else
            {
                result.ResultCode = account_authn_data.ResultCode;
                result.ResultMessage = account_authn_data.ResultMessage;
                return result;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception in GetRecentAccountAsync: " + e.ToString());
            result.ResultCode = ResultStatusCodes.EXCEPTION;
            result.ResultMessage = e.Message;
            return result;
        }
    }

    public async Task UpdateAccountCredentials(AccountAuthnResult accountAuthnResult, Account account)
    {
        try
        {
            account.LocalEncryptionKey = accountAuthnResult.LocalEncryptionKey;
            account.LocalAuthnType = accountAuthnResult.LocalAuthnType;
            account.PasskeyCredentials = accountAuthnResult.PasskeyCredentials;
            await _Account_Db.Update(account);
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception in UpdateAccountCredentials: " + e.Message);
        }
    }

    public async Task UpdateAccountLockTimeout(int lock_timeout_sec, Account? account)
    {
        if (account is not null)
        {
            account.LockTimeoutSec = lock_timeout_sec;
            await _Account_Db.Update(account);
        } 
    }

    public async Task DeleteAccount(Account account)
    {
        await _Account_Db.Delete(account.Id);
    }

    public async Task<string> EraseAccount(Account account)
    {
        var error_message = string.Empty;

        var secrets = new List<Secret>(account.Secrets);
        foreach (var secret in secrets)
        {
            var secret_result = await DeleteSecret(secret);
            if (secret_result.ResultCode != ResultStatusCodes.SUCCESS)
                error_message += secret.Data.Title + "; ";
        }

        var contacts = new List<Contact>(account.Contacts);
        foreach (var contact in contacts)
        {
            var messages = new List<Message>(contact.GetAllMessages());

            foreach (var message in messages)
            {
                var message_result = await DeleteMessage(message);
                if (message_result.ResultCode != ResultStatusCodes.SUCCESS)
                    error_message += contact.PeerNickName + "; ";
            }

            var contact_result = await DeleteContact(contact);
            if (contact_result.ResultCode != ResultStatusCodes.SUCCESS)
                error_message += contact.PeerNickName + "; ";
        }




        await _Account_Db.Delete(account.Id);

        return error_message;
    }

    #endregion Accounts

    #region Messages

    public async Task<SendMessageResult> SendMessage(ChatOut chatout, string MessageText, int message_type)
    {
        var result = new SendMessageResult();
        try
        {

            MsgBlock msg_block;

            var recipient_secret_address = PublicAddress.RecreateFromAddressString(chatout.Contact.PeerSecretAddress, chatout.Contact.Account.Network);
            var encoder = new MsgBlockEncoder(
                   recipient_secret_address.ReadPubKey.ToString(),
                   chatout.Height,
                   //chatout.LastOutMessageHash,
                   !string.IsNullOrEmpty(chatout.LastOutMessageHash) ? chatout.LastOutMessageHash : RandomSeed.GenerateRandomSeed(),
                   chatout.ChatPubKey,
                   SecKey.FromBase58( chatout.ChatKey),
                   MessageText,
                   message_type,
                   chatout.Contact.Account.Network,
                   chatout.Contact.Account.HasLicense);


            msg_block = await encoder.Encode();


            //var transferResponse = await _msgBlockService.BroadcastAsync(msg_block);
            var transferResponse = await _apiClientService.MsgBlockBroadcastAsync(msg_block);
            if (transferResponse.Status != ResultStatusCodes.SUCCESS)
            {
                result.ResultCode = transferResponse.Status;
                result.ResultMessage = transferResponse.Message;
                return result;
            }
         
            var message = new Message(
                MessageDirections.Outgoing,
                encoder.MsgDataUnpacked);
            message.Id = Guid.NewGuid().ToString();
            message.Hash = msg_block.Hash;

            message.Height = msg_block.Height;

            message.TimeStamp = msg_block.TimeStamp;
            chatout.AddMessage(message);

            await _Message_Db.Add(message);
            chatout.Height = message.Height;
            chatout.LastOutMessageHash = message.Hash;
            await _ChatOut_Db.Update(chatout);

            chatout.Contact.Account.LastUpdated = UnixDateTime.Now;
            await _Account_Db.Update(chatout.Contact.Account);

            result.ResultCode = transferResponse.Status;
            result.ResultMessage = transferResponse.Message;
            result.NewMessage = message;

            return result;

        }
        catch (Exception e)
        {
            result.ResultCode = ResultStatusCodes.EXCEPTION;
            result.ResultMessage = "Failed to send message: " + e.Message;
            Console.WriteLine(result.ResultMessage);
            return result;
        }
    }

    public async Task<SendMessageResult> DeleteMessage(Message message_to_delete)
    {
        var result = new SendMessageResult();
        try
        {
            MsgBlock msg_block;
            var chatout = message_to_delete.Chat as ChatOut;

            var recipient_secret_address = PublicAddress.RecreateFromAddressString(chatout.Contact.PeerSecretAddress, chatout.Contact.Account.Network);
            var encoder = new MsgBlockEncoder(
                   recipient_secret_address.ReadPubKey.ToString(),
                   chatout.Height,
                    //chatout.LastOutMessageHash,
                    !string.IsNullOrEmpty(chatout.LastOutMessageHash) ? chatout.LastOutMessageHash : RandomSeed.GenerateRandomSeed(),
                   chatout.ChatPubKey,
                   SecKey.FromBase58(chatout.ChatKey),
                   string.Empty,
                   MessageTypes.EDIT,
                   chatout.Contact.Account.Network,
                   chatout.Contact.Account.HasLicense);


            msg_block = await encoder.EncodeDelete(message_to_delete.Hash);

            var transferResponse = await _apiClientService.MsgBlockBroadcastAsync(msg_block);
            if (transferResponse.Status != ResultStatusCodes.SUCCESS)
            {
                result.ResultCode = transferResponse.Status;
                result.ResultMessage = transferResponse.Message;
                return result;
            }

            if (message_to_delete.Height > 0) 
            {
                chatout.RemoveMessage(message_to_delete.Id);
                await _Message_Db.Delete(message_to_delete.Id);
            }
            else // this is the first message (chat init block itself)
            {
                chatout.MessageData = MsgBlockData.EmptyData;
            }

            chatout.Height = msg_block.Height;
            chatout.LastOutMessageHash = msg_block.Hash;
            await _ChatOut_Db.Update(chatout);

            chatout.Contact.Account.LastUpdated = UnixDateTime.Now;
            await _Account_Db.Update(chatout.Contact.Account);

            result.ResultCode = transferResponse.Status;
            result.ResultMessage = transferResponse.Message;
            result.NewMessage = null;

            return result;

        }
        catch (Exception e)
        {
            result.ResultCode = ResultStatusCodes.EXCEPTION;
            result.ResultMessage = "Failed to delete message: " + e.Message;
            Console.WriteLine(result.ResultMessage);
            return result;
        }
    }

    #endregion Messages


    #region Contacts

    public async Task DeleteContactFromLocalStorage(string contact_id, string account_id)
    {
        await _Contact_Db.Delete(contact_id, account_id);
    }


    public async Task<SendContactRequestResult> DeleteContact(Contact contact)
    {
        var result = new SendContactRequestResult();
        try
        {

            //var contact = account.CurrentContact;
            var contact_id = contact.Id;

            var block_key_pair = KeyPair.FromPrivateKey(contact.BlockPrivateKey);
            var encoder = new ContactBlockEncoder(
                    block_key_pair,
                    contact.PeerUserAddress,
                    contact.Account.CurrentAddress.ToString(),
                    string.Empty,
                    contact.SenderSecretAddress,
                    ContactRequestCommands.Update,
                    contact.Account.Network,
                    true,
                    contact.Height + 1,
                    contact.BlockHash,
                    string.Empty,
                    0
                );
            var block = await encoder.EncodeDelete();
            block.License = await AssignLicense(contact.Account, encoder);

            //var res = await _contactBlockNodeService.BroadcastAsync(block);
            var res = await _apiClientService.ContactBroadcastAsync(block);
            
            if (res.Status != ResultStatusCodes.SUCCESS)

            {

                result.ResultMessage = "DeleteContact() Error: " + res.Message;
                result.ResultCode = ResultStatusCodes.EXCEPTION;
                return result;
            }

            //contact.TimeStampOut = UnixDateTime.ToDateTime(block.TimeStamp);
            //contact.LastScannedChatBlock = block.TimeStamp;
            //account.LastUpdated = UnixDateTime.Now;

            contact.Account.RemoveContact(contact.PeerUserAddress);
            await DeleteContactFromLocalStorage(contact_id, contact.Account.Id);

            result.ResultCode = ResultStatusCodes.SUCCESS;
        }
        catch (Exception e)
        {

            result.ResultCode = -6;
            result.ResultMessage = "DeleteContact() failed: " + e.Message;
        }

        return result;

    }

    public async Task<SendLicenseFeeResult> SendLicenseFee(decimal FeeAmount)
    {
        var result = new SendLicenseFeeResult();
        try
        {
            var send_fee_result = await _wallet.SendLicenseFee(FeeAmount);
            result.ResultCode = send_fee_result.ResultCode;
            if (result.ResultCode == ResultStatusCodes.SUCCESS)
            {
                result.LicensePubKey = send_fee_result.Tx.PubKey;
            }
            else
            {
                result.ResultMessage = send_fee_result.ResultMessage;          
            }
        }
        catch (Exception e)
        {
            result.ResultCode = ResultStatusCodes.EXCEPTION;
            result.ResultMessage = "SendLicenseFee() failed: " + e.Message;
        }

        return result;
    }


    /// <summary>
    /// Creates a new chat with a new peer.
    /// This method should be used only if contact does not exists yet.
    /// In fact, it creates and sends a new contact request, but it does not send the chat init block until
    /// it gets replied by the recipient with the chat initi block using the secret address.
    /// </summary>
    public async Task<SendContactRequestResult> NewContactRequest(Account account, string RecipientAddress)
    {
        var result = new SendContactRequestResult();

        try
        {

            var contact = account.GetContact(RecipientAddress);
            var is_new_contact = false;
            var command = ContactRequestCommands.InitialRequest;
            var block_key_pair = KeyPair.GenerateRandom();

            if (contact != null)
            {
                switch (contact.Status)
                {
                    case ContactStatuses.Established:
                        result.ResultCode = -2;
                        result.ResultMessage = "Contact already established";
                        return result;

                    case ContactStatuses.RequestSent:
                        result.ResultCode = -3;
                        result.ResultMessage = "Contact request already sent";
                        return result;

                    case ContactStatuses.NotFound: // shouldn't happen
                                                   
                        result.ResultCode = -7;
                        result.ResultMessage = "Contact status not found";
                        return result;
                    case ContactStatuses.RequestReceived:
                        command = ContactRequestCommands.Reply;
                        
                        break;

                    default:
                        result.ResultCode = -8;
                        result.ResultMessage = "Contact status is not set";
                        return result;

                }
            }
            else
            {

                is_new_contact = true;
                contact = Contact.CreateNewOut(
                    RecipientAddress,
                    account.ChatExpirationDays,
                    account.CurrentAddress,
                    block_key_pair.PrivateKey,
                    0,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    0,
                    account.LicenseType,
                    string.Empty,
                    account.Network);
            }

            var encoder = new ContactBlockEncoder(
                    block_key_pair,
                    RecipientAddress,
                    account.CurrentAddress.ToString(),
                    account.NickName,
                    contact.SenderSecretAddress,
                    command,
                    account.Network,
                    false,
                    0,
                    string.Empty,
                    string.Empty,
                    account.LicenseType
                );
            var block = await encoder.Encode();
            block.License = await AssignLicense(account, encoder);

            if (encoder.Network != Networks.mainnet)
                encoder.PrintBlock();
            
            var res = await _apiClientService.ContactBroadcastAsync(block);
            if (res.Status != ResultStatusCodes.SUCCESS)

            {

                result.ResultMessage = "NewContactRequest() Error: " + res.Message;
                result.ResultCode = ResultStatusCodes.EXCEPTION;
                return result;
            }

            contact.TimeStampOut = block.TimeStamp;
            contact.LastScannedChatBlock = block.TimeStamp;

            contact.Height = block.Height;
            contact.BlockHash = block.Hash;
            contact.PrevHash = block.PrevHash;
            contact.BlockPrivateKey = block_key_pair.PrivateKey;
            contact.BlockPublicKey = block.PubKey;
            contact.LicenseType = block.LicenseType;
            contact.LicensePubKey = block.License;
            account.LastUpdated = UnixDateTime.Now;

            if (is_new_contact)
            {
                account.AddContact(contact);
                await _Contact_Db.Add(contact);
            }
            else
            {
                await _Contact_Db.Update(contact);
            }

            await _Account_Db.Update(contact.Account);

            //return contact;
            result.ResultCode = ResultStatusCodes.SUCCESS;
            result.NewContact = contact;

        }
        catch (Exception e)
        {

            result.ResultCode = -6;
            result.ResultMessage = "NewContactRequest() Failed to communicate with the node: " + e.Message;
        }
        
        return result;
    }

    /// <summary>
    /// Updates an existing contact
    /// This method should be used only if contact exists already.
    /// </summary>
    public async Task<SendContactRequestResult> UpdateContactName(Contact contact, string new_name)
    {
        var result = new SendContactRequestResult();
        try
        {
            if (contact.Status != ContactStatuses.Established)
            {
                result.ResultMessage = "Contact is not established yet";
                result.ResultCode = ResultStatusCodes.CONTACT_IS_NOT_ESTABLISHED;
                return result;
            }

            contact.PeerNickName = new_name;

            var command = ContactRequestCommands.InitialRequest;


            var block_key_pair = KeyPair.FromPrivateKey(contact.BlockPrivateKey);

            command = ContactRequestCommands.Update;

            var encoder = new ContactBlockEncoder(
                    block_key_pair,
                    contact.PeerUserAddress,
                    contact.Account.CurrentAddress.ToString(),
                    contact.Account.NickName,
                    contact.SenderSecretAddress,
                    command,
                    contact.Account.Network,
                    true,
                    contact.Height + 1,
                    contact.BlockHash,
                    contact.PeerNickName,
                    contact.Account.LicenseType
                );
            var block = await encoder.Encode();
            block.License = await AssignLicense(contact.Account, encoder);

            //var res = await _contactBlockNodeService.BroadcastAsync(block);
            var res = await _apiClientService.ContactBroadcastAsync(block);
            if (res.Status != ResultStatusCodes.SUCCESS)
            {

                result.ResultMessage = "UpdateContactName() Error: " + res.Message;
                result.ResultCode = ResultStatusCodes.EXCEPTION;
                return result;
            }

            contact.TimeStampOut = block.TimeStamp;
            contact.Height = block.Height;
            contact.BlockHash = block.Hash;
            contact.PrevHash = block.PrevHash;


            //contact.LastScannedChatBlock = block.TimeStamp;
            contact.Account.LastUpdated = UnixDateTime.Now;

            await _Contact_Db.Update(contact);
            await _Account_Db.Update(contact.Account);

            result.ResultCode = ResultStatusCodes.SUCCESS;
            result.NewContact = contact;

        }
        catch (Exception e)
        {

            result.ResultCode = -6;
            result.ResultMessage = "UpdateContactName() failed: " + e.Message;
        }

        return result;
    }

    public async Task<bool> ContactGarbageCollector(Account account)
    {
        var result = false;
        if (account.HasLicense) // TO DO implement gc for licensed blocks
            return result;

        var contact_interval = UnixTimeInterval.FromRetentionInterval(account.Network, typeof(ContactBlock));
        var contact_threshold = UnixDateTime.DeletionThreshold(contact_interval);

        // Use LINQ to identify contacts that should be deleted
        var contactsToDelete = account.Contacts
            .Where(contact => (contact.TimeStampOut != 0 && contact.TimeStampOut < contact_threshold) ||
                              (contact.TimeStampIn != 0 && contact.TimeStampIn < contact_threshold))
            .ToList();

        foreach (var contact in contactsToDelete)
        {
            account.RemoveContact(contact.PeerUserAddress);
            await _Contact_Db.Delete(contact.Id, account.Id);
            result = true;
        }

        return result;
    }


    public async Task<List<Contact>> LookForNewContactRequests(Account account)
    {
        //Contact new_contact = null;
        List<Contact> result = new List<Contact>();
        try
        {
            var blocks_info = await ScanForContactRequest(account);
            if (blocks_info.Blocks.Count > 0)
            {
                foreach (var block_data in blocks_info.Blocks)
                {
                    Contact? new_contact;
                    if (blocks_info.Directions[block_data.Hash] == ContactDirections.In)
                        new_contact = await ProcessIncomingContactRequest(account, block_data);
                    else
                        new_contact = await ProcessSelfContactRequest(account, block_data);
                    if (new_contact != null)
                        result.Add(new_contact);
                    if (block_data.TimeStamp > account.LastScannedContactBlock)
                        account.LastScannedContactBlock = block_data.TimeStamp + 1;
                }
            }
            else
            {
                account.LastScannedContactBlock = UnixDateTime.Now;
            }

            //InitBlockNodeIsUp = true;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //InitBlockNodeIsUp = false;
        }

        await _Account_Db.Update(account);

        return result; //account.LastScannedContactBlock;
    }


    private async Task<ContactScanInfo> ScanForContactRequest(Account account)
    {
        //List<ContactBlock> result = new List<ContactBlock>();
        var result = new ContactScanInfo();
        try
        {
            while (true)
            {
                var scan_params = new ScanContactParams();
                scan_params.LastScannedDateTime = account.LastScannedContactBlock;
                //scan_params.LastScannedDateTime = account.LastScannedInitBlock;
                Console.WriteLine("\nLastScannedDateTime: {0}", scan_params.LastScannedDateTime);
                scan_params.PageSize = ClientConstants.CONTEXT_SCAN_PAGE_SIZE_BLOCKS;

                //var scan_response = await _contactBlockNodeServiceClient.ScanAsync(scan_params);
                var scan_response = await _apiClientService.ContactScanAsync(scan_params);

                if (scan_response != null && scan_response.Blocks.Count > 0)
                {
                    foreach (var block in scan_response.Blocks)
                    {
                        var validator = new ContactBlockValidator(block);
                        var stealth_address = StealthAddress.Restore(account.CurrentAddress.ScanKey.ToString(), block.PubKey, validator.CalculateBlockSalt());
                        
                       // if (Debugger.IsAttached)
                        {
                            Console.WriteLine("SendProcessor ScanForContactRequest() ----------->");
                            Console.WriteLine("block_salt: " + validator.CalculateBlockSalt());
                            Console.WriteLine("account.CurrentAddress.ScanKey: " + account.CurrentAddress.ScanKey.ToString());
                            Console.WriteLine("block.PubKey: " + block.PubKey.ToString());
                        }
                        
                        if (stealth_address.IsMatch(block.RecipientStealthAddress))
                        {
                            result.Blocks.Add(block);
                            result.Directions.Add(block.Hash, ContactDirections.In);
                        }
                        else
                        if (stealth_address.IsMatch(block.SenderStealthAddress))
                        {
                            result.Blocks.Add(block);
                            result.Directions.Add(block.Hash, ContactDirections.Out);
                        }

                        if (account.LastScannedContactBlock < block.TimeStamp)
                            account.LastScannedContactBlock = block.TimeStamp;
                    }


                    if (scan_response.Blocks.Count < ClientConstants.CONTEXT_SCAN_PAGE_SIZE_BLOCKS)
                        break;
                }
                else
                {
                    break;
                }
            }
        }
        catch (Exception e)
        {

            throw new Exception("ScanForContactRequest() Failed", e);
        }

        return result;
    }


    private async Task<Contact?> ProcessIncomingContactRequest(Account account, ContactBlock block)
    {
        // TO DO - process delete and update blocks

        if (string.IsNullOrEmpty(block.MessageData))
            return null;

        try
        {

            var codec = new ContactBlockDecoder(block);

            codec.ClientValidate();
            codec.Decode(account.CurrentAddress.ReadKey);

            var received_message_data = codec.MsgDataUnpacked; //codec.DecodeMsgData(account.CurrentAddress.ReadKeyBase64);
            var peerUserAddress = received_message_data.GetParam(MsgBlockData.SENDER_ADDRESS);
            var peerSecretAddress = received_message_data.GetParam(MsgBlockData.SENDER_SECRET_ADDRESS);
            var peer_nickname = received_message_data.GetParam(MsgBlockData.SENDER_NICKNAME);
            var command_string = received_message_data.GetParam(MsgBlockData.CONTACT_REQUEST_COMMAND);

            ContactRequestCommands? cmd = Contact.ParseToEnum(command_string);
            if (!cmd.HasValue)
                throw new Exception("Bad contact request - failed to parse contact request command.");

            var contact = account.GetContact(peerUserAddress);
            if (contact != null)
            {
                switch (contact.Status)
                {
                    case ContactStatuses.Established:
                        switch (cmd)
                        {
                            case ContactRequestCommands.Update:
                                contact.PeerSecretAddress = peerSecretAddress;
                                contact.PeerNickName = peer_nickname;
                                contact.TimeStampIn = block.TimeStamp;
                                await _Contact_Db.Update(contact);
                                break;

                            case ContactRequestCommands.InitialRequest:
                                // someone is trying again, so probably connection was not successful;
                                // let's reset the status to RequestReceived to trigger Reply???
                                contact.PeerSecretAddress = peerSecretAddress;
                                contact.TimeStampOut = 0;
                                contact.TimeStampIn = block.TimeStamp;
                                await _Contact_Db.Update(contact);
                                break;

                            default:
                                // just ignore for now
                                break;

                        }

                        break;

                    case ContactStatuses.RequestSent:
                        // no matter what command is, let's treat it as a reply; no need to answer
                        contact.PeerSecretAddress = peerSecretAddress;
                        contact.PeerNickName = peer_nickname;
                        // note that this will automatically change the status to Established
                        // which should trigger the UI to enable chat init sending
                        contact.TimeStampIn = block.TimeStamp;
                        await _Contact_Db.Update(contact);
                        break;

                    case ContactStatuses.RequestReceived:
                        // this is a repeated request, probably previous one was ignored, let's treat it as new one
                        contact.PeerNickName = peer_nickname;
                        contact.TimeStampIn = block.TimeStamp;
                        await _Contact_Db.Update(contact);
                        break;

                    default:
                        throw new Exception("Unknown contact state");
                }


            }
            else
            {
                // no matter what command is we'll treat it as initial request

                contact = Contact.CreateNewIn(
                    peerUserAddress, 
                    account.ChatExpirationDays, peerSecretAddress, 
                    peer_nickname, 
                    account.CurrentAddress, 
                    block.TimeStamp, 
                    //block.LicenseType,
                    account.Network);

                // note that this will automatically change the status to RequestReceived
                // which should trigger the UI to prompt for ACCEPT / IGNORE input 
                //contact.TimeStampIn = UnixDateTime.ToDateTime(block.TimeStamp);
                account.AddContact(contact);
                await _Contact_Db.Add(contact);

                account.LastUpdated = UnixDateTime.Now;
                await _Account_Db.Update(account);
            }
            return contact;
        }

        catch (BlockExpiredWithNoLicenseException ex)
        {
            Console.WriteLine("Block is expired" + ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception in ProcessIncomingContactRequest(): " + ex.ToString());
            return null;
        }
    }

    private async Task<Contact?> ProcessSelfContactRequest(Account account, ContactBlock block)
    {
        try
        {
            if (block.DeleteFlag && string.IsNullOrEmpty(block.InitData))
            {
                // this is "delete" block (not update), this contact has been deleted in the network, so remove the corresponding contact
                var existing_contact = await _Contact_Db.GetByBlockPubKey(block.PubKey, account.Id);
                if (existing_contact != null)
                {
                    if (existing_contact.TimeStampOut < block.TimeStamp)
                    {
                        account.RemoveContactByBlockPubKey(block.PubKey);
                        await _Contact_Db.DeleteByBlockPubKey(block.PubKey, account.Id);
                    }
                }
            }

            if (string.IsNullOrEmpty(block.InitData))
                return null;

            var decoder = new ContactBlockDecoder(block);

            var self_init_data = decoder.DecodeSelfData(account.CurrentAddress.ReadKey);

            var peerUserAddress = self_init_data.RecipientUserAddress;
            var selfSecretAddress = self_init_data.SecretAddress;

            var contact = account.GetContact(peerUserAddress);
            if (contact != null)
            {
                switch (contact.Status)
                {
                    case ContactStatuses.Established:
                    case ContactStatuses.RequestSent:
                        // thjis is update block, processs updated peer nickname
                        contact.PeerNickName = self_init_data.PeerNickname;

                        contact.TimeStampOut = block.TimeStamp;
                        contact.Height = block.Height;
                        contact.BlockHash = block.Hash;
                        contact.PrevHash = block.PrevHash;
                        contact.LicenseType = block.LicenseType;

                        await _Contact_Db.Update(contact);
                        account.LastUpdated = UnixDateTime.Now;
                        await _Account_Db.Update(account);
                        break;

                    case ContactStatuses.RequestReceived:
                        contact.SenderSecretAddress = selfSecretAddress;

                        contact.TimeStampOut = block.TimeStamp;
                        contact.Height = block.Height;
                        contact.BlockHash = block.Hash;
                        contact.PrevHash = block.PrevHash;
                        contact.LicenseType = block.LicenseType;
                        contact.LastScannedChatBlock = block.TimeStamp;
                        await _Contact_Db.Update(contact);
                        account.LastUpdated = UnixDateTime.Now;
                        await _Account_Db.Update(account);
                        break;

                    default:
                        throw new Exception("Unknown contact state");
                }

            }
            else
            {
                contact = Contact.CreateNewOut(
                    peerUserAddress,
                    account.ChatExpirationDays,
                    account.CurrentAddress,
                    SecKey.FromBase58(self_init_data.BlockPrivateKey),
                    block.Height,
                    block.Hash,
                    block.PrevHash,
                    block.PubKey,
                    block.TimeStamp,
                    block.LicenseType,
                    block.License,
                    block.Network
                    );

                contact.SenderSecretAddress = selfSecretAddress;

                contact.LastScannedChatBlock = block.TimeStamp;

                account.AddContact(contact);
                await _Contact_Db.Add(contact);
                account.LastUpdated = UnixDateTime.Now;
                await _Account_Db.Update(account);

            }
            return contact;
        }
        catch (BlockExpiredWithNoLicenseException ex)
        {
            Console.WriteLine("Block is expired" + ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception in ProcessSelfContactRequest(): " + ex.ToString());
            return null;
        }
    }


    #endregion Contacts

    #region Secrets 

    public string GenerateServerKey()
    {
       // return Hashing.SHA256Base58(Guid.NewGuid().ToString());
       return Hashing.KeccakBase58(Guid.NewGuid().ToString());
    }

    public string GenerateClientKey()
    {
        //return Hashing.SHA256Base58(Guid.NewGuid().ToString());
        return Hashing.KeccakBase58(Guid.NewGuid().ToString());
    }

    public async Task<SendSecretResult> DeleteSecret(
      Secret secret)
    {
        var result = new SendSecretResult();
        var account = secret.Account;
        //var secret_id = secret.Id;
        try
        {


            var block_key_pair = KeyPair.FromPrivateKeyBase58(secret.Data.BlockPrivateKey);

            var codec = new SecretBlockEncoder(
                block_key_pair,
                account.CurrentAddress,
                secret.Height,
                secret.BlockHash,
                null,
                null,
                account.Network,
                account.LicenseType,
                Logger);
            var block = await codec.EncodeDelete();
            block.License = await AssignLicense(account, codec);

            //var res = await _secretBlockNodeService.BroadcastAsync(block);
            var res = await _apiClientService.SecretBroadcastAsync(block);

            if (res.Status == ResultStatusCodes.SUCCESS)
            {

                account.RemoveSecretByBlockPubKey(secret.BlockPubKey);

                await _Secret_Db.DeleteByBlockPubKey(secret.BlockPubKey, secret.Account.Id);

                account.CurrentSecretPubKey = null;
                account.LastUpdated = UnixDateTime.Now;
                await _Account_Db.Update(account);

                result.ResultCode = ResultStatusCodes.SUCCESS;
            }
            else
            {
                result.ResultMessage = "DeleteSecret() Error: " + res.Message;
                result.ResultCode = res.Status;
            }
        }
        catch (Exception e)
        {
            result.ResultCode = ResultStatusCodes.EXCEPTION;
            result.ResultMessage = "DeleteSecret() failed: " + e.Message;
        }
        return result;
    }

    /*private string[]? SetSecretVariables(SecretBlockData block_data)
    {
        string[]? variables = null;
        if (block_data.SecretType == SecretTypes.Application)
        {
            if (block_data.RuntimeType == RuntimeTypes.Panther_Detection)
            {
                variables = new string[4];
                variables[0] = block_data.GetRuntimeParam(RuntimeVariables.CLOUD_ACCOUNT);
                variables[1] = block_data.GetRuntimeParam(RuntimeVariables.CLOUD_ACCESS_KEY).Substring(0, 12);

                variables[2] = block_data.GetRuntimeParam(RuntimeVariables.CLOUD_ASSUMED_ROLE);
                variables[3] = block_data.GetRuntimeParam(RuntimeVariables.CLIENT_KEY);
            }
            else
            if (block_data.RuntimeType == RuntimeTypes.Mac_MacOS)
            {

                variables = new string[6];

                variables[0] = block_data.GetRuntimeParam(RuntimeVariables.HOST_NAME);
                variables[1] = block_data.GetRuntimeParam(RuntimeVariables.MAC_ADDRESS);
                variables[2] = block_data.GetRuntimeParam(RuntimeVariables.MOTHERBOARD);
                variables[3] = block_data.GetRuntimeParam(RuntimeVariables.LOCAL_IP_ADDRESS);
                variables[4] = block_data.GetRuntimeParam(RuntimeVariables.USER_NAME);
                variables[5] = block_data.GetRuntimeParam(RuntimeVariables.CLIENT_KEY);

            }
            else
                throw new Exception("Runtime is not implemented");
        }
        return variables;
    }
    */

  
    public async Task<SendSecretResult> UpdateSecret(
        //Account account,
        Secret old_secret,
        SecretBlockData updated_block_data)
    {
        var result = new SendSecretResult();
        var account = old_secret.Account;
        try
        {
            //block_data.SecretId = account.CurrentSecret.Data.SecretId;
            updated_block_data.BlockPrivateKey = old_secret.Data.BlockPrivateKey;

            var prev_hash = old_secret.BlockHash;
            
            //var variables = SetSecretVariables(updated_block_data);

            var block_key_pair = KeyPair.FromPrivateKeyBase58(old_secret.Data.BlockPrivateKey);

            var codec = new SecretBlockEncoder(
                block_key_pair,
                account.CurrentAddress,
                old_secret.Height,
                prev_hash,
                updated_block_data,
                null, //variables,
                account.Network,
                account.LicenseType,
                Logger);
            var block = await codec.EncodeUpdate();

           
            block.License = await AssignLicense(account, codec);

            //var res = await _secretBlockNodeService.BroadcastAsync(block);
            var res = await _apiClientService.SecretBroadcastAsync(block);
            if (res.Status == ResultStatusCodes.SUCCESS)
            {
                //var updated_secret = new Secret();
                var updated_secret = Secret.FromBlock(codec, updated_block_data);
                updated_secret.Id = old_secret.Id;
                
                account.UpdateSecret(updated_secret);
                await _Secret_Db.Update(updated_secret);
                await _Account_Db.Update(account);

                result.ResultCode = ResultStatusCodes.SUCCESS;
                result.NewSecret = updated_secret;
            }
            else
            {
                result.ResultMessage = "EditSecret() Error: " + res.Message;
                result.ResultCode = res.Status;
            }

        }
        catch (Exception e)
        {

            result.ResultCode = ResultStatusCodes.EXCEPTION;
            result.ResultMessage = "EditSecret() exception: " + e.Message;
        }

        return result;
    }


    public async Task<SendSecretResult> AddNewSecret(
        Account account,
        SecretBlockData block_data)
    {
        var result = new SendSecretResult();

        try
        {
            if (account.GetSecretByTitle(block_data.Title) != null)
            {
                result.ResultCode = ResultStatusCodes.BLOCK_ALREADY_EXISTS;
                result.ResultMessage = "Secret with the same title already exists";
                return result;
            }

            //block_data.SecretId = Guid.NewGuid().ToString();

            if (block_data.SecretType == SecretTypes.Folder)
                block_data.FolderId = Guid.NewGuid().ToString();

            var block_key_pair = KeyPair.GenerateRandom();
            block_data.BlockPrivateKey = block_key_pair.PrivateKey.ToString();

            //var variables = SetSecretVariables(block_data);

            var codec = new SecretBlockEncoder(
                block_key_pair,
                account.CurrentAddress,
                0,
                string.Empty,
                block_data,
                null,
                account.Network,
                account.LicenseType,
                Logger
                );
            //var block = codec.Encode();
            var block = await Task.Run(() => codec.Encode());

            block.License = await AssignLicense(account, codec);

            //var res = await _secretBlockNodeService.BroadcastAsync(block);
            var res = await _apiClientService.SecretBroadcastAsync(block);
            if (res.Status == ResultStatusCodes.SUCCESS)
            {
                var secret = Secret.FromBlock(codec, block_data);
                secret.Id = Guid.NewGuid().ToString();
                secret.LicensePubKey = block.License; 
                account.AddSecret(secret);

                await _Secret_Db.Add(secret);
                await _Account_Db.Update(account);

                result.ResultCode = ResultStatusCodes.SUCCESS;
                result.NewSecret = secret;
            }
            else
            {
                result.ResultMessage = "NewSecret() Error: " + res.Message;
                result.ResultCode = res.Status;
            }
        }
        catch (Exception e)
        {

            result.ResultCode = ResultStatusCodes.EXCEPTION;
            result.ResultMessage = "AddNewSecret() exception: " + e.Message;
        }

        return result;
    }

    public async Task<SendSecretResult> AddInstantShareSecret(
        Account account,
        SecretBlockData block_data,
        int expiration)
        //string baseUri)
    {
        var result = new SendSecretResult();

        try
        {

            var block_key_pair = KeyPair.GenerateRandom();
            block_data.BlockPrivateKey = block_key_pair.PrivateKey.ToString();

            var instant_key_bytes = AES.GenerateRandomAES256Key();
            block_data.InstantKey = Codecs.FromBytesToBase58(instant_key_bytes);

            //var variables = SetSecretVariables(block_data);

            var encoder = new SecretBlockEncoder(
                block_key_pair,
                account.CurrentAddress,
                0,
                string.Empty,
                block_data,
                null, //variables,
                account.Network,
                account.LicenseType,
                Logger
                );
            var block = await encoder.EncodeInstantShare(expiration);

            block.License = await AssignLicense(account, encoder);

            //var res = await _secretBlockNodeService.BroadcastAsync(block);
            var res = await _apiClientService.SecretBroadcastAsync(block);
            if (res.Status == ResultStatusCodes.SUCCESS)
            {

                var secret = Secret.FromBlock(encoder, block_data);
                secret.Id = Guid.NewGuid().ToString();
                
                account.AddSecret(secret);

                await _Secret_Db.Add(secret);
                await _Account_Db.Update(account);

                result.ResultCode = ResultStatusCodes.SUCCESS;
                result.NewSecret = secret;
            }
            else
            {
                result.ResultMessage = "InstantShareSecret() Error: " + res.Message;
                result.ResultCode = res.Status;
            }
        }
        catch (Exception e)
        {

            result.ResultCode = ResultStatusCodes.EXCEPTION;
            result.ResultMessage = "InstantShareSecret() exception: " + e.Message;
        }

        return result;
    }

    // returns true if GC took place and full refresh is required
    public async Task<bool> SecretGarbageCollector(Account account)
    {
        var result = false;
        if (account.HasLicense) // TO DO implement gc for licensed blocks
            return result;

        var interval = UnixTimeInterval.FromRetentionInterval(account.Network, typeof(SecretBlock));
        var threshold = UnixDateTime.DeletionThreshold(interval);

        // Use LINQ to filter out chats that need to be deleted
        var blocksToDelete = account.Secrets.Where(chatout => chatout.TimeStamp < threshold).ToList();

        foreach (var secret in blocksToDelete)
        {
            account.Secrets.Remove(secret);
            await _Secret_Db.DeleteByBlockPubKey(secret.BlockPubKey, account.Id);
            result = true;
        }

        return result;
    }

    public async Task<bool> LookForSecrets(Account account)
    {
        // *** debug - remove afterwards:
        // var HiddenScanKey = string.Empty;
        // var PubKey = string.Empty;
        // var block_salt = string.Empty;
        // ***
        var result = false;
        List<SecretBlock> blocks = new List<SecretBlock>();
        try
        {
            //const int PAGE_SIZE = 100;
            while (true)
            {
                var scan_params = new ScanSecretBlockParams();
                scan_params.LastScannedDateTime = account.LastScannedSecretBlock;
                scan_params.PageSize = ClientConstants.SECRET_SCAN_PAGE_SIZE_BLOCKS;

                //var scan_response = await _secretBlockNodeServiceClient.ScanAsync(scan_params);
                var scan_response = await _apiClientService.SecretScanAsync(scan_params);
                if (scan_response != null && scan_response.Blocks.Count > 0)
                {
                    foreach (var block in scan_response.Blocks)
                    {
                        var decoder = new SecretBlockValidator(block);
                        // *** debug - remove afterwards:
                        //HiddenScanKey = account.CurrentAddress.HiddenScanKey.ToString();
                        //PubKey = block.PubKey;
                        //block_salt = decoder.CalculateBlockSalt();
                        // ***
                        
                        var stealth_address = StealthAddress.Restore(
                            account.CurrentAddress.HiddenScanKey.ToString(),
                            block.PubKey,
                            decoder.CalculateBlockSalt());

                        if (stealth_address.IsMatch(block.StealthAddress))
                        {
                            blocks.Add(block);
                        }
                        if (account.LastScannedSecretBlock < block.TimeStamp)
                            account.LastScannedSecretBlock = block.TimeStamp;
                    }
                    if (scan_response.Blocks.Count < ClientConstants.SECRET_SCAN_PAGE_SIZE_BLOCKS)
                        break;
                }
                else
                {
                    break;
                }
            }
            var new_result = false;
            foreach (var block in blocks)
            {
                if (await ProcessSecretBlock(account, block))
                    new_result = true;
            }
            var gc_result = await SecretGarbageCollector(account);
            result = new_result || gc_result;
        }
        catch (Exception e)
        {
            throw new Exception(
                //$"Scan for secrets failed: {e} HiddenScanKey: {HiddenScanKey} PubKey: {PubKey} block_salt: {block_salt}");
                $"Scan for secrets failed: {e}");
            
        }

        return result;
    }

    public async Task<Secret> GetInstantShareSecret(string blockHash, byte[] encryptionKey)
    {
        Secret secret = null;
        try
        {

            var get_params = new SecretBlockParams();
            get_params.BlockHash = blockHash;
            //var get_result = await _secretBlockNodeServiceClient.GetSecretBlockAsync(get_params);
            var get_result = await _apiClientService.SecretGetByHashAsync(get_params);
            if (get_result.Status == ResultStatusCodes.SUCCESS)
            {
                var decoder = new SecretBlockDecoder(get_result.Block);
                var block_data = decoder.DecodeInstantShareData(encryptionKey);
                secret = Secret.FromBlock(decoder, block_data);
                secret.Id = Guid.NewGuid().ToString();
            }
        }
        catch (Exception e)
        {
            throw new Exception("GetInstantShareSecret() failed: ", e);
        }

        return secret;
    }


    private async Task<bool> ProcessSecretBlock(Account account, SecretBlock block)
    {
        try
        {
            if (block.DeleteFlag || string.IsNullOrEmpty(block.SecretData))
            {
                // this is either "delete" block, this secret has been deleted in the network, so remove the corresponding secret
                var existing_secret = await _Secret_Db.GetByBlockPubKey(block.PubKey, account.Id);
                if (existing_secret != null)
                {
                    if (existing_secret.TimeStamp < block.TimeStamp)
                    {
                        account.RemoveSecretByBlockPubKey(block.PubKey);
                        await _Secret_Db.DeleteByBlockPubKey(block.PubKey, account.Id);
                    }
                }
            }

            if (!string.IsNullOrEmpty(block.SecretData))
            {
                var decoder = new SecretBlockDecoder(block);

                //var secret = Secret.FromBlock(block, decoder.Decode(account.CurrentAddress.ReadKey));
                var secret = Secret.FromBlock(decoder, decoder.Decode(account.CurrentAddress.ReadKey));
                secret.Id = Guid.NewGuid().ToString();

                var existing_secret = await _Secret_Db.GetByBlockPubKey(secret.BlockPubKey, account.Id);
                if (existing_secret != null)
                {
                    if (existing_secret.TimeStamp < secret.TimeStamp)
                    {
                        account.UpdateSecret(secret);
                        await _Secret_Db.Update(secret);
                        account.LastUpdated = UnixDateTime.Now;
                        await _Account_Db.Update(account);
                    }
                }
                else
                {
                    account.AddSecret(secret);
                    await _Secret_Db.Add(secret);
                    account.LastUpdated = UnixDateTime.Now;
                    await _Account_Db.Update(account);
                }

            }
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }


    #endregion Secrets


    #region Chats

    public async Task<SendChatOutResult> SendChatOut(Contact contact, string MessageText, int message_type)
    {
        var result = new SendChatOutResult();
        try
        {
            var chat_key_pair = KeyPair.GenerateRandom();
            var encoder = new ChatBlockEncoder(
                0,
                string.Empty,
                chat_key_pair,
                contact.PeerSecretAddress,
                contact.SenderSecretAddress.ToString(),
                contact.Account.NickName,
                MessageText,
                message_type,
                contact.ExpirationDays,
                contact.Account.Network,
                contact.Account.LicenseType);

            var block = await encoder.Encode();
            block.License = await AssignLicense(contact.Account, encoder);

            //var res = await _initBlockNodeService.BroadcastAsync(block);
            var res = await _apiClientService.InitBlockBroadcastAsync(block);
            result.ResultCode = res.Status;
            result.ResultMessage = res.Message;

            if (res.Status != ResultStatusCodes.SUCCESS)
            {
                result.ResultMessage = res.Message;
                return result;
            }
            //add block to local client db

            var chat = new ChatOut();
            chat.ChatKey = chat_key_pair.PrivateKey.ToString();
            chat.ChatPubKey = chat_key_pair.PublicKey.ToString();

            chat.Height = 0;
            chat.BlockHash = block.Hash;
            chat.LastOutMessageHash = "";
            chat.MessageData = encoder.MsgDataUnpacked;


            chat.Id = Guid.NewGuid().ToString();

            chat.TimeStamp = block.TimeStamp;

            contact.AddChatOut(chat);
            contact.Account.LastUpdated = UnixDateTime.Now;

            await _ChatOut_Db.Add(chat);
            await _Account_Db.Update(contact.Account);

            result.NewChatOut = chat;
            return result;

        }
        catch (Exception e)
        {
            result.ResultCode = ResultStatusCodes.EXCEPTION;
            result.ResultMessage = "SendChatOut() failed to send chat out: " + e.Message;
            return result;
        }
    }

    #endregion Chats

    public async Task Exit()
    {
        //_DbService.Stop();
        await Task.Delay(100);
        Environment.Exit(0);
    }

    #region Licenses

    private int GetRandomIndex(int limit)
    {
        Random random = new Random();
        int randomNumber = random.Next(0, limit);
        return randomNumber;
    }

   
    //private async Task<string> AssignLicense<BlockType, DataType>(Account account, LicensableBlockCodec<BlockType, DataType> block)
    private async Task<string> AssignLicense(Account account, ILicensableBlock block)
    {
        if (account.HasLicense)
        {
            decimal FeeAmount = LicenseFeeCalculator.CalculateFee(block.DataSizeInBytes());
            var result = await _wallet.SendLicenseFee(FeeAmount);
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new LicenseValidationException("Could not process llicense payment");
            return result.Tx.PubKey;
        }
        return string.Empty;
    }


    //public async Task<string> GenerateLicenseProof(Account account, string block_hash, Int64 block_time_stamp, int block_height, string block_pub_key)
    /*public async Task<string> GenerateLicenseProof(Account account, ILicensableBlock block)
    {

        try
        {
            if (account.LicenseExpirationDate == 0 ||
                string.IsNullOrWhiteSpace(account.LicensePrivateKey) ||
                string.IsNullOrWhiteSpace(account.LicensePublicKey))
                return string.Empty;

            var salt_1 = Hashing.KeccakBase58(block.TimeStamp + block.BlockPubKey + block.Height);
            var salt_2 = Hashing.KeccakBase58(block.Height + block.BlockPubKey + block.TimeStamp);

            var license_keys_params = new LicenseKeysParams();
            license_keys_params.ExpirationDate = account.LicenseExpirationDate;
            license_keys_params.Limit = Constants.MAX_RING_SIGNATURE_LIMIT;

            var license_keys_response = await _apiClientService.GetLicenseKeysAsync(license_keys_params);

            if (license_keys_response == null || license_keys_response.Status != ResultStatusCodes.SUCCESS ||
                license_keys_response.Keys == null || license_keys_response.Keys.Count == 0)
            {
                throw new ApplicationException("Could not fetch keys");
            }

            var lsag = new Liu2005(salt_1, salt_2);
            var participants_count = license_keys_response.Keys.Count;
            //var identity = 0;

            var messageBytes = Codecs.FromASCIIToBytes(block.Hash);

            List<BigInteger> public_keys = new List<BigInteger>();
            foreach (var key in license_keys_response.Keys)
            {
                //var key_bytes = Liu2005.Signature.GetBytesFromBase64String(key);
                var key_bytes = Codecs.FromBase58ToBytes(key);
                
                public_keys.Add(new BigInteger(key_bytes));
            }

            var real_private_key = new BigInteger(Liu2005.Signature.GetBytesFromBase64String(account.LicensePrivateKey));
            //var real_public_key = new BigInteger(Liu2005.Signature.GetBytesFromBase64String(account.LicensePublicKey));
            var real_public_key = new BigInteger(Codecs.FromBase58ToBytes(account.LicensePublicKey));

            var identity_index = public_keys.FindIndex(x => x == real_public_key);
            if (identity_index == -1)
            {
                if (public_keys.Count == Constants.MAX_RING_SIGNATURE_LIMIT)
                    public_keys.RemoveAt(GetRandomIndex(public_keys.Count));
                identity_index = GetRandomIndex(public_keys.Count);
            }

            public_keys.Insert(identity_index, real_public_key);

            var public_keys_bigint = public_keys.ToArray();

            var signature = lsag.GenerateSignature(messageBytes, public_keys_bigint, real_private_key, identity_index);
            var signature_str = Liu2005.Signature.ToBase64String2(signature);

            return signature_str;

        }
        catch (Exception e)
        {
            throw new ApplicationException("ComposeLicenseProof() failed: " + e.Message, e);
        }
    }*/

    /*public async Task<string> LookForLicense(Account account)
    {
        var result = string.Empty;

        try
        {
            long last_scanned_license_block = 0;
            decimal rewards = 0m;
            while (true)
            {
                var scan_params = new ScanLicenseBlockParams();
                scan_params.LastScannedDateTime = last_scanned_license_block;
                scan_params.PageSize = ClientConstants.LICENSE_SCAN_PAGE_SIZE_BLOCKS;

                var scan_response = await _apiClientService.LicenseBlockScanAsync(scan_params);
                if (scan_response != null && scan_response.Blocks.Count > 0)
                {
                    foreach (var block in scan_response.Blocks)
                    {
                        var decoder = new LicenseBlockDecoder(block);
                        var stealth_address = StealthAddress.Restore(account.CurrentAddress.ScanKey.ToString(), block.MasterLicensePubKey, decoder.CalculateBlockSalt());
                        if (stealth_address.IsMatch(block.StealthAddress))
                        {

                            decoder.ClientValidate();
                            var license_private_key = decoder.Decode(account.CurrentAddress.ReadKey);
                            if (decoder.Version == 2)
                            {
                                rewards += decoder.Rewards;
                                account.Rewards = rewards.ToString();
                                account.LastUpdated = UnixDateTime.Now;
                                await _Account_Db.Update(account);
                            }
                            
                            if (block.ExpirationDate >= account.LicenseExpirationDate)
                            {
                                //var license_private_key = decoder.Decode(account.CurrentAddress.ReadKeyBase64);
                                account.LicensePrivateKey = license_private_key;
                                account.LicensePublicKey = block.PubKey;
                                account.LicenseExpirationDate = block.ExpirationDate;
                                account.LicenseType = block.LicenseType;
                                account.LastUpdated = UnixDateTime.Now;
 
                                await _Account_Db.Update(account);
                                result = "Premium until " + UnixDateTime.ToDateTime(account.LicenseExpirationDate).ToLongDateString();
                            }
                        }

                        if (last_scanned_license_block < block.TimeStamp)
                            last_scanned_license_block = block.TimeStamp;
                    }
                    if (scan_response.Blocks.Count < ClientConstants.LICENSE_SCAN_PAGE_SIZE_BLOCKS)
                        break;
                }
                else
                {
                    break;
                }
            }

        }

        catch (Exception e)
        {
            Console.WriteLine(e);
            return result;
        }
        if (result == string.Empty)
        {
            account.LicensePrivateKey = string.Empty;
            account.LicensePublicKey = string.Empty;
            account.LicenseExpirationDate = 0;
            account.LicenseType = 0;
            account.LastUpdated = UnixDateTime.Now;
            //account.Rewards = 0;
            await _Account_Db.Update(account);
            result = "Free";
        }
        return result;
    }*/


    #endregion Licenses

}