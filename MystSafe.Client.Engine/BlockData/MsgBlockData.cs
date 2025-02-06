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

using MystSafe.Client.Base;
using Newtonsoft.Json;

namespace MystSafe.Client.Engine;


    public class MessageTypes
    {
        public const int TEXT = 0;
        public const int SECRET = 1;

        // ** future functionality
        public const int EDIT = 2;
        public const int REPLY = 3;
        public const int CONFIRMATION = 4;
        // **
    }

    public class MsgBlockData : BaseBlockData
    {


        //[JsonIgnore]
        //public const string MSG_PUB_KEY = "MPK";
        [JsonIgnore] public const string SENDER_ADDRESS = "SA";
        [JsonIgnore] public const string SENDER_NICKNAME = "SN";
        [JsonIgnore] public const string SENDER_SECRET_ADDRESS = "SSA";
        [JsonIgnore] public const string CONTACT_REQUEST_COMMAND = "CRC";
        [JsonIgnore] public const string SECRET_DATA = "SD";

        public int MessageType { get; set; }

        public string MsgText { get; set; }

        // use this one to create a new msg data to send
        public MsgBlockData() : base()
        {
        }

        // use this one to restore the received msg data
        public MsgBlockData(string MessageData) : base(MessageData)
        {
            MsgBlockData data = JsonConvert.DeserializeObject<MsgBlockData>(MessageData);
            MsgText = data.MsgText;
            MessageType = data.MessageType;

            Params = data.Params;
        }

        public override string ToString()
        {
            // need to serialize the content to a string here
            return JsonConvert.SerializeObject(this);
        }

        public static MsgBlockData EmptyData
        {
            get
            {
                var empty_data = new MsgBlockData();
                empty_data.MsgText = string.Empty;
                return empty_data;
            }
        }
    }


