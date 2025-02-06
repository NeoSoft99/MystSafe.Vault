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

namespace MystSafe.Client.Engine;

    public enum ContactDirections
    {
        In, Out
    }

    public enum ContactStatuses : int
    {
        NotFound = 0, // default status - where no in or out requests found

        RequestSent = 1, // this account sent the request but did not received reply yet

        RequestReceived = 2, // this account received the request but did not reply yet

        Established = 3, // both in and out requests are present

        Blocked = 4, // TO DO - the contact is marked to ignore the in reqeusts.
                     // Special contact request out block needs to be created to "memorize" this state in the network
    }


    public class Contact
    {
        // this is just a unique ID for the local client DB
        // 
        public readonly string Id;

        //// required to find all chats in the local client DB that belong to particular account/address
        //public string AccountId { get; set; }

        // this is an in or out address depnding on who's initiated the chat, but always peer address
        public readonly string PeerUserAddress;

        // this are for incoming init blocks
        // this is an in or out address depending on who's initiated the chat, but always peer address
        public string PeerSecretAddress { get; set; }

        public string PeerNickName { get; set; }

        // this is the unique address provided to the peer and used for outgoing chat blocks
        public SecretAddress SenderSecretAddress { get; set; }

        // this determines for how many days the chats messagea are stored.
        // minimum is 1 day, maximum is 30
        // the chats and messages older than this interval will be deleted.
        // new chat will be created every day (when new message is sent),
        // which allows to delete the entire chat init block and its assosiated message chain every day.
        public readonly int ExpirationDays;

        // the timestamp of the last scanned incoming chat block
        public long LastScannedChatBlock { get; set; }


        //public DateTime TimeStampOut { get; set; }
        public long TimeStampOut { get; set; }

        //public DateTime TimeStampIn { get; set; }
        public long TimeStampIn { get; set; }

        public SecKey BlockPrivateKey { get; set; }

        public int Height { get; set; }

        public string BlockHash { get; set; }

        public string PrevHash { get; set; }

        public string BlockPublicKey { get; set; }

        public int LicenseType { get; set; }
        
        public string LicensePubKey { get; set; }  

        #region These are the properties that are NOT stored explicitly in the database

        public ContactStatuses Status
        {
            get
            {
                if (TimeStampOut != 0 && TimeStampIn != 0)
                    return ContactStatuses.Established;

                if (TimeStampOut != 0 && TimeStampIn == 0)
                    return ContactStatuses.RequestSent;

                if (TimeStampOut == 0 && TimeStampIn != 0)
                    return ContactStatuses.RequestReceived;

                return ContactStatuses.NotFound;

            }
        }

        public readonly List<ChatOut> ChatsOut = new List<ChatOut>();
        public readonly List<ChatIn> ChatsIn = new List<ChatIn>();

        public ChatIn CurrentChatIn
        {
            get { return getMostRecentChatIn(); }
        }

        public ChatOut CurrentChatOut
        {
            get { return getMostRecentChatOut(); }
        }

        public Account Account { get; set; }

        public int ColorIndex = -1;

        #endregion



        // this is to use when contact request is coming from a peer, so we know 
        private Contact(
            string Id,
            string peerUserAddress,
            string peerSecretAddress,
            string peerNickName,
            SecretAddress senderSecretAddress,
            int expirationDays,
            long lastScannedChatBlock,
            long time_stamp_out,
            long time_stamp_in,
            SecKey block_private_key,
            int height,
            string block_hash,
            string prev_hash,
            string block_public_key,
            int license_type,
            string license_pub_key)
        {
            this.Id = Id;
            this.PeerUserAddress = peerUserAddress;
            this.PeerSecretAddress = peerSecretAddress;
            this.PeerNickName = peerNickName;
            this.SenderSecretAddress = senderSecretAddress;
            this.ExpirationDays = expirationDays;
            this.LastScannedChatBlock = lastScannedChatBlock;

            this.TimeStampOut = time_stamp_out;

            this.TimeStampIn = time_stamp_in;
            this.BlockPrivateKey = block_private_key;
            this.Height = height;
            this.BlockHash = block_hash;
            this.PrevHash = prev_hash;
            this.BlockPublicKey = block_public_key;
            this.LicenseType = license_type;
            this.LicensePubKey = license_pub_key;
        }

        // this is to use when sending out a new contact request
        public static Contact CreateNewOut(
            string peerUserAddress,
            int expirationDays,
            UserAddress accountAddress,
            SecKey block_private_key,
            int height,
            string block_hash,
            string prev_hash,
            string block_public_key,
            long block_time_stamp,
            int license_type,
            string license_pub_key,
            int network)
        {
            var id = Guid.NewGuid().ToString();
            var secret_address = SecretAddress.GenerateFromPeerAddress(accountAddress, peerUserAddress, network);
            return new Contact(
                id,
                peerUserAddress,
                string.Empty,
                string.Empty,
                secret_address,
                expirationDays,
                block_time_stamp,
                block_time_stamp,
                0,
                block_private_key,
                height,
                block_hash,
                prev_hash,
                block_public_key,
                license_type,
                license_pub_key);
        }

        // this is to use when receiving a new contact request from peer
        public static Contact CreateNewIn(
            string peerUserAddress,
            int expirationDays,
            string peerSecretAddress,
            string peerNickName,
            UserAddress accountAddress,
            long block_time_stamp,
            //int license_type,
            int network
        )
        {
            var id = Guid.NewGuid().ToString();
            var secret_address = SecretAddress.GenerateFromPeerAddress(accountAddress, peerUserAddress, network);
            return new Contact(
                id,
                peerUserAddress,
                peerSecretAddress,
                peerNickName,
                secret_address,
                expirationDays,
                block_time_stamp,
                0,
                block_time_stamp,
                SecKey.CreateEmpty(),
                0,
                string.Empty,
                string.Empty,
                string.Empty,
                0,
                string.Empty);
        }


        public static Contact Restore(
            string Id,
            string peerUserAddress,
            string peerSecretAddress,
            string peerNickName,
            string secretKey,
            string secretScanKey,
            int expirationDays,
            long lastScannedChatBlock,
            long time_stamp_out,
            long time_stamp_in,
            string block_private_key,
            int height,
            string block_hash,
            string prev_hash,
            string block_public_key,
            int license_type,
            string license_pub_key,
            int network)
        {
            var secret_address = SecretAddress.RestoreFromKeys(secretKey, secretScanKey, network);
            return new Contact(
                Id,
                peerUserAddress,
                peerSecretAddress,
                peerNickName,
                secret_address,
                expirationDays,
                lastScannedChatBlock,
                time_stamp_out,
                time_stamp_in,
                SecKey.FromBase58(block_private_key),
                height,
                block_hash,
                prev_hash,
                block_public_key,
                license_type,
                license_pub_key);
        }

        public void AddChatIn(ChatIn chatin)
        {
            chatin.Contact = this;
            ChatsIn.Add(chatin);
        }

        public void AddChatOut(ChatOut chatout)
        {
            chatout.Contact = this;
            ChatsOut.Add(chatout);
        }



        // find the most recent chat session
        private ChatIn getMostRecentChatIn()
        {
            ChatIn result = null;
            foreach (var chat in ChatsIn)
            {
                if (result == null)
                    result = chat;
                else if (chat.TimeStamp > result.TimeStamp)
                    result = chat;
            }

            return result;
        }

        // find the most recent chat session
        private ChatOut getMostRecentChatOut()
        {
            ChatOut result = null;
            foreach (var chat in ChatsOut)
            {
                if (result == null)
                    result = chat;
                else if (chat.TimeStamp > result.TimeStamp)
                    result = chat;
            }

            return result;
        }

        /// <summary>
        /// Get all messages related to current chat (including those from previous chats for the same peer address)
        /// </summary>
        public List<Message> GetAllMessages()
        {
            var all_messages = new List<Message>();

            foreach (var chat in ChatsIn)
            {
                var chat_message = Message.GenerateFromChatIn(chat);
                if (chat_message != null)
                {
                    all_messages.Add(chat_message);
                }

                all_messages.AddRange(chat.Messages);
            }

            foreach (var chat in ChatsOut)
            {
                var chat_message = Message.GenerateFromChatOut(chat);
                if (chat_message != null)
                {
                    all_messages.Add(chat_message);
                }

                all_messages.AddRange(chat.Messages);
            }

            all_messages.Sort();
            return all_messages;
        }

        public string GetLatestMessageText()
        {
            var all_messages = GetAllMessages();
            if (all_messages.Count == 0)
                return string.Empty;
            var message = all_messages[all_messages.Count - 1];

            if (message.MessageData.MessageType == MessageTypes.TEXT)
                return GetFirstChars(message.MessageData.MsgText);
            else if (message.MessageData.MessageType == MessageTypes.SECRET)
                return GetFirstChars(message.SecretData.Title);
            else
                return string.Empty;

        }

        private string GetFirstChars(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return input.Length >= 20 ? input.Substring(0, 20) + "..." : input;
        }


        public void ClearChats()
        {
            foreach (var chat in ChatsIn)
                chat.ClearMessages();
            ChatsIn.Clear();
            foreach (var chat in ChatsOut)
                chat.ClearMessages();
            ChatsOut.Clear();
        }

        public string PeerAddressShort
        {
            get { return UserAddress.AddressShort(PeerUserAddress); }
        }

        public string GetCaption()
        {
            if (!string.IsNullOrWhiteSpace(this.PeerNickName))
                return this.PeerNickName;
            else
                return this.PeerAddressShort;
        }

        public string GetUserNameShort()
        {
            if (!string.IsNullOrWhiteSpace(this.PeerNickName))
                return GetTwoLetters(this.PeerNickName);
            else
                return GetTwoLetters(this.PeerAddressShort);
        }

        private string GetTwoLetters(string Name)
        {
            if (string.IsNullOrWhiteSpace(Name))
                return " ";
            if (Name.Length == 1)
                return Name[0].ToString();
            return Name[0].ToString() + Name[1].ToString();
        }

        public static ContactRequestCommands? ParseToEnum(string command)
        {
            if (System.Enum.TryParse<ContactRequestCommands>(command, true, out var result))
            {
                return result;
            }

            return null;
        }

        public bool ChatBlockExists(string hash)
        {
            foreach (var chat in ChatsIn)
                if (chat.BlockHash == hash)
                    return true;

            foreach (var chat in ChatsOut)
                if (chat.BlockHash == hash)
                    return true;

            return false;
        }
    }

