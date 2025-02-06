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

using MystSafe.Shared.Common;
using MystSafe.Shared.Crypto;
using Microsoft.Extensions.Logging;
using NBitcoin;

namespace MystSafe.Client.Engine;

class ContactScanInfo
{
    public readonly List<ContactBlock> Blocks = new List<ContactBlock>();
    public readonly Dictionary<string, ContactDirections> Directions = new Dictionary<string, ContactDirections>();

    public void AddContact(ContactBlock block, ContactDirections direction)
    {
        Blocks.Add(block);
        Directions.Add(block.Hash, direction);
    }
}

// TO DO: this needs to be modified in teh future to use "long polling"
public class UpdateProcessor
	{

		//private bool _InitBlockNodeIsUp;
    public bool AppIsUp { get; set; }
    public bool ScanIsEnabled { get; set; }

    private readonly IAccountDB _Account_Db;
    private readonly IChatInDB _ChatIn_Db;
    private readonly IChatOutDB _ChatOut_Db;
    private readonly IMessageDB _Message_Db;
    private readonly IContactDB _Contact_Db;
    private readonly ISecretDB _Secret_Db;

    public readonly ILogger<SendProcessor> Logger;
    private readonly ApiClientService _apiClientService;

  //  public bool InitBlockNodeIsUp
		//{
		//	get { return _InitBlockNodeIsUp; }
		//	set
		//	{
		//		if (value != _InitBlockNodeIsUp)
		//		{
		//			_InitBlockNodeIsUp = value;
		//			Console.WriteLine("Init Block Node Connection is {0}", _InitBlockNodeIsUp ? "UP" : "DOWN");
  //          }
		//	}
		//}

    public UpdateProcessor(
        bool enable_scan,
        IAccountDB account_db,
        IContactDB contact_db,
        IChatInDB chatin_db,
        IChatOutDB chatout_db,
        IMessageDB message_db,
        ISecretDB secret_Db,
        ILogger<SendProcessor> logger,
        ApiClientService apiClientService
        )
    { 
        _Account_Db = account_db;
        _Contact_Db = contact_db;
        _ChatIn_Db = chatin_db;
        _ChatOut_Db = chatout_db;
        _Message_Db = message_db;
        _Secret_Db = secret_Db;
			AppIsUp = true;
        ScanIsEnabled = enable_scan;
        Logger = logger;
        _apiClientService = apiClientService;
    }

    #region Accounts

    /*public async Task<bool> UpdateMasterLicensePrivateKey(string master_license_private_key, string master_license_public_key, Account account)
    {
        try
        {
            account.MasterLicensePrivateKey = master_license_private_key;
            account.MasterLicensePubKey = master_license_public_key;
            account.LastUpdated = UnixDateTime.Now;
            await _Account_Db.Update(account);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return false;
        }
    }*/

    #endregion

    #region Chats

    // returns true if GC took place and full refresh is required
    public async Task<bool> ChatGarbageCollector(Contact contact)
    {
        var result = false;
        if (contact.Account.HasLicense) // TO DO implement gc for licensed blocks
            return result;

        var chat_msg_interval = UnixTimeInterval.FromRetentionInterval(contact.Account.Network, typeof(InitBlock));
        var chat_msg_threshold = UnixDateTime.DeletionThreshold(chat_msg_interval);

        // Use LINQ to filter out chats that need to be deleted
        var chatsToDelete = contact.ChatsOut.Where(chatout => chatout.TimeStamp < chat_msg_threshold).ToList();

        foreach (var chatout in chatsToDelete)
        {
            contact.ChatsOut.Remove(chatout);
            await _ChatOut_Db.Delete(chatout.Id, contact.Account.Id); // Adjust to your async delete method
            result = true;
        }

        return result;
    }

    public async Task<bool> LookForChatUpdates(Contact contact)
    {
        var new_chat_or_message = await LookForNewChatRequests(contact);

        foreach (var chatout in contact.ChatsOut)
            if (await LookForOutgoingMessages(chatout))
                new_chat_or_message = true;

        foreach (var chatin in contact.ChatsIn)
            if (await LookForIncomingMessages(chatin))
                new_chat_or_message = true;

        var gc_result = await ChatGarbageCollector(contact);

        return new_chat_or_message || gc_result;
    }

    public async Task<bool> LookForNewChatRequests(Contact contact)
    {
        //ChatIn received_chat = null;
        bool result = false;
        try
        {
           
            //var init_block = await ScanForChatInit(contact);
            var scan_response = await ScanForChatInit(contact);
            if (scan_response.Status == ResultStatusCodes.SUCCESS)
            {
                if (contact.ChatBlockExists(scan_response.Block.Hash))
                {
                    result = false;
                    contact.LastScannedChatBlock = scan_response.Block.TimeStamp + 1;
                    //InitBlockNodeIsUp = true;
                    return result;
                }

                //// *** Reject expired chat blocks
                //var chat_msg_interval = UnixTimeInterval.FromRetentionInterval(contact.Account.Network, typeof(InitBlock));
                //var chat_msg_threshold = UnixDateTime.DeletionThreshold(chat_msg_interval);
                //if (scan_response.Block.TimeStamp < chat_msg_threshold && !string.IsNullOrWhiteSpace(scan_response.Block.License))
                //{
                //    result = false;
                //    contact.LastScannedChatBlock = scan_response.Block.TimeStamp + 1;
                //    InitBlockNodeIsUp = true;
                //    return result;
                //}
                //// ***



                if (!scan_response.IsSelf)
                {
                    var received_chat = await ProcessIncomingChat(contact, scan_response.Block);
                    
                }
                else
                {
                    var sent_chat = await ProcessOutgoingChat(contact, scan_response.Block);
                    
                }
                contact.LastScannedChatBlock = scan_response.Block.TimeStamp + 1;
                //InitBlockNodeIsUp = true;
                await _Contact_Db.Update(contact);
                result = true;
            }
            else if (scan_response.Status == ResultStatusCodes.NOT_FOUND)
            {
                contact.LastScannedChatBlock = UnixDateTime.Now;
                await _Contact_Db.Update(contact);
                //InitBlockNodeIsUp = true;
            }
            else
            {
                throw new Exception("Unknown scan_response Status: " + scan_response.Status);
            }


        }
        catch (Exception e)
        {
            Console.WriteLine("Exception in LookForNewChatRequests: " + e.ToString());
            //InitBlockNodeIsUp = false;
        }

        return result;
    }



    private async Task<ScanChatResult> ScanForChatInit(Contact contact)
    {
        //InitBlock result = null;
        try
        {
            var scan_params = new ChatScanParams();
            scan_params.LastScannedDateTime = contact.LastScannedChatBlock - 1;
            //scan_params.LastScannedDateTime = account.LastScannedInitBlock;
            Console.WriteLine("\nLastScannedDateTime: {0}", scan_params.LastScannedDateTime);
            //scan_params.PageSize = 100;
            scan_params.ScanKey = contact.SenderSecretAddress.ScanKey.ToString();

            //var chat_block = await _initBlockNodeServiceClient.ScanAsync(scan_params);
            //var scan_response = await _initBlockNodeServiceClient.ScanAsync(scan_params);
            var scan_response = await _apiClientService.InitBlockScanAsync(scan_params);

            return scan_response;
        }
        catch (Exception e)
        {
            var result = new ScanChatResult();
            result.Status = -10;
            result.Message = "exception in ScanForChatInit: " + e.Message;
            return result;
        }
    }


    private async Task<ChatIn?> ProcessIncomingChat(Contact contact, InitBlock block)
    {
        try
        {
            var decoder = new ChatBlockDecoder(block);
            decoder.ClientValidate();
            decoder.Decode(contact.SenderSecretAddress.ReadKey);

            var chat = new ChatIn();
            chat.MessageData = decoder.MsgDataUnpacked; //codec.DecodeBlockData(contact.SenderSecretAddress.ReadKeyBase64);
            chat.Height = 0;
            chat.Id = Guid.NewGuid().ToString();
            chat.TimeStamp = block.TimeStamp;
            chat.ChatPubKey = block.ChatPubkey;

            chat.BlockHash = block.Hash;

            contact.AddChatIn(chat);

            await _ChatIn_Db.Add(chat);

            contact.Account.LastUpdated = UnixDateTime.Now;
            await _Account_Db.Update(contact.Account);

            Console.WriteLine("\nNew chat request received!\n");
            Console.WriteLine("From (address)  : {0}", contact.PeerUserAddress);
            Console.WriteLine("From (nickname) : {0}", contact.PeerNickName);

            return chat;
        }
        catch (BlockExpiredWithNoLicenseException ex)
        {
            Console.WriteLine("Block is expired" + ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception in ProcessIncomingChat(): " + ex.ToString());
            return null;
        }
    }


    // this is when syncing with the network and restoring its own chat blocks
    private async Task<ChatOut?> ProcessOutgoingChat(Contact contact, InitBlock block)
    {
        try
        {
            var decoder = new ChatBlockDecoder(block);
            decoder.ClientValidate();

            var chat = new ChatOut();
            chat.Height = 0;

            chat.Id = Guid.NewGuid().ToString();
            chat.TimeStamp = block.TimeStamp;
            chat.ChatPubKey = block.ChatPubkey;
            chat.BlockHash = block.Hash;

            var init_data = decoder.DecodeSelfData(contact.SenderSecretAddress.ReadKey); //init_block.DecodeInitData(contact.SenderSecretAddress.ReadKeyBase64);
            chat.ChatKey = init_data.ChatKeyBase58;

            var peer_secret_address = PublicAddress.RecreateFromAddressString(contact.PeerSecretAddress, block.Network);

            chat.MessageData = decoder.DecodeMsgDataBySelf(SecKey.FromBase58(chat.ChatKey), peer_secret_address.ReadPubKey.ToString());

            contact.AddChatOut(chat);

            await _ChatOut_Db.Add(chat);

            contact.Account.LastUpdated = UnixDateTime.Now;
            await _Account_Db.Update(contact.Account);

            Console.WriteLine("\nNew outcoming chat request found!\n");
            Console.WriteLine("To (address)  : {0}", contact.PeerUserAddress);
            Console.WriteLine("To (nickname) : {0}", contact.PeerNickName);

            return chat;
        }
        catch (BlockExpiredWithNoLicenseException ex)
        {
            Console.WriteLine("Block is expired" + ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception in ProcessOutgoingChat(): " + ex.ToString());
            return null;
        }
    }


    #endregion

    #region Messages


    public async Task<bool> LookForIncomingMessages(ChatIn chat)
    {
        if (chat == null)
            return false;

        var result = false;
        try
        {
            ScanMsgResult scan_response;
            bool has_more = true;
            while (has_more)
            {
                var scan_params = new ScanMsgParams();
                scan_params.LastScannedHeight = chat.Height;
                scan_params.ChatPubKey = chat.ChatPubKey;

                //scan_response = await _msgBlockNodeServiceClient.ScanAsync(scan_params);
                scan_response = await _apiClientService.MsgBlockScanAsync(scan_params);
                has_more = scan_response.HasMore;
                if (scan_response.Block != null)
                {


                    if (!string.IsNullOrEmpty(scan_response.Block.MessageData))
                    {
                        var decoder = new MsgBlockDecoder(scan_response.Block);
                        decoder.ClientValidate();
                        decoder.Decode(chat.Contact.SenderSecretAddress.ReadKey);

                        var message = new Message(MessageDirections.Incoming, decoder.MsgDataUnpacked);

                        message.Id = Guid.NewGuid().ToString();
                        message.Hash = scan_response.Block.Hash;
                        message.TimeStamp = scan_response.Block.TimeStamp;
                        message.Height = scan_response.Block.Height;
                        chat.AddMessage(message);
                        await _Message_Db.Add(message);
                    }

                    if (scan_response.Block.DeleteFlag)
                    {
                        _Message_Db.DeleteByHash(scan_response.Block.DeletionHash);
                        chat.RemoveMessageByHash(scan_response.Block.DeletionHash);
                    }

                    chat.Height = scan_response.Block.Height;

                    await _ChatIn_Db.Update(chat);

                    chat.Contact.Account.LastUpdated = UnixDateTime.Now;
                    await _Account_Db.Update(chat.Contact.Account);
                    result = true;
                }


            }
        }

        catch (Exception e)
        {
            //Console.WriteLine("ScanForMsg() Failed: " + e.Message);
            result = false;
        }

        return result;
    }

    public async Task<bool> LookForOutgoingMessages(ChatOut chat)
    {
        if (chat == null)
            return false;

        var result = false;
        try
        {
            ScanMsgResult scan_response;
            bool has_more = true;
            while (has_more)
            {
                var scan_params = new ScanMsgParams();
                scan_params.LastScannedHeight = chat.Height;
                scan_params.ChatPubKey = chat.ChatPubKey;

                scan_response = await _apiClientService.MsgBlockScanAsync(scan_params);
                has_more = scan_response.HasMore;
                if (scan_response.Block != null)
                {

                    if (!string.IsNullOrEmpty(scan_response.Block.MessageData))
                    {
                        var decoder = new MsgBlockDecoder(scan_response.Block);

                        var peer_secret_address = PublicAddress.RecreateFromAddressString(chat.Contact.PeerSecretAddress, scan_response.Block.Network);

                        var message = new Message(MessageDirections.Outgoing, decoder.DecodeMsgDataBySelf(SecKey.FromBase58(chat.ChatKey), peer_secret_address.ReadPubKey.ToString()));

                        message.Id = Guid.NewGuid().ToString();
                        message.Hash = scan_response.Block.Hash;
                        message.TimeStamp = scan_response.Block.TimeStamp;
                        message.Height = scan_response.Block.Height;

                        chat.AddMessage(message);
                        await _Message_Db.Add(message);
                    }

                    if (scan_response.Block.DeleteFlag)
                    {
                        _Message_Db.DeleteByHash(scan_response.Block.DeletionHash);
                        chat.RemoveMessageByHash(scan_response.Block.DeletionHash);
                    }


                    chat.Height = scan_response.Block.Height;
                    chat.LastOutMessageHash = scan_response.Block.Hash;

                    await _ChatOut_Db.Update(chat);

                    chat.Contact.Account.LastUpdated = UnixDateTime.Now;
                    await _Account_Db.Update(chat.Contact.Account);

                    result = true;
                }


            }
        }

        catch (Exception e)
        {
            //Console.WriteLine("ScanForMsg() Failed: " + e.Message);
            result = false;
        }

        return result;
    }

    #endregion

    

}

