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
using MystSafe.Client.Engine;


namespace MystSafe.Client.Tests
{
    public class InitBlockBuilderFixture
    {
        public ChatBlockEncoder Codec_Sent { get; private set; }
        public ChatBlockDecoder Codec_Received { get; private set; }
        public KeyPair BlockKeys { get; private set; }
        public UserAddress RecipientUserAddress { get; private set; }
        public UserAddress SenderUserAddress { get; private set; }
        public SecretAddress SenderSecretAddress { get; private set; }
        public SecretAddress RecipientSecretAddress { get; private set; }

        public InitBlockBuilderFixture()
        {
            // Arrange
            BlockKeys = KeyPair.GenerateRandom();
            SenderUserAddress = UserAddress.GenerateFromMnemonic(Networks.devnet);
            RecipientUserAddress = UserAddress.GenerateFromMnemonic(Networks.devnet);

            var senderNickName = "UnitTest";
            SenderSecretAddress = SecretAddress.GenerateFromPeerAddress(SenderUserAddress, RecipientUserAddress.ToString(), Networks.devnet); // Initialize with appropriate values
            RecipientSecretAddress = SecretAddress.GenerateFromPeerAddress(RecipientUserAddress, SenderUserAddress.ToString(), Networks.devnet); // Initialize with appropriate values

            // Act
            Codec_Sent = new ChatBlockEncoder(
                0,
                string.Empty,
                BlockKeys,
                RecipientSecretAddress.ToString(),
                SenderSecretAddress.ToString(),
                senderNickName,
                "Hello, World!",
                MessageTypes.TEXT,
                14,
                Networks.devnet,
                Constants.FREE_LICENSE_TYPE
            );

            //var block = Codec_Sent.Encode();
            //Codec_Received = new ChatBlockDecoder(block);
            Task<InitBlock> task = Codec_Sent.Encode();
            var block = task.GetAwaiter().GetResult();
            Codec_Received = new ChatBlockDecoder(block);
        }
    }

    public class InitBlockCodecTests : IClassFixture<InitBlockBuilderFixture>
    {
        private readonly ChatBlockEncoder _codec_Sent;
        private readonly ChatBlockDecoder _codec_Received;
        private readonly KeyPair _blockKeys;
        private readonly UserAddress _recipientUserAddress;
        private readonly UserAddress _senderUserAddress;
        private readonly SecretAddress _senderSecretAddress;
        private readonly SecretAddress _recipientSecretAddress;

        public InitBlockCodecTests(InitBlockBuilderFixture fixture)
        {
            _codec_Sent = fixture.Codec_Sent;
            _codec_Received = fixture.Codec_Received;
            _blockKeys = fixture.BlockKeys;
            _recipientUserAddress = fixture.RecipientUserAddress;
            _senderUserAddress = fixture.SenderUserAddress;
            _senderSecretAddress = fixture.SenderSecretAddress;
            _recipientSecretAddress = fixture.RecipientSecretAddress;
        }

        [Fact]
        public void TestInitBlockSignature()
        {
            Assert.NotNull(_codec_Sent);
            Assert.NotNull(_codec_Received);

            Assert.Equal(_codec_Sent.Hash, _codec_Received.Hash);
            Assert.Equal(_codec_Sent.Signature, _codec_Received.Signature);

            var exception = Record.Exception(() => _codec_Sent.ServerValidate());
            Assert.Null(exception);
            exception = Record.Exception(() => _codec_Received.ClientValidate());
            Assert.Null(exception);
        }

        [Fact]
        public void TestInitBlockData()
        {
            _codec_Received.Decode(_recipientSecretAddress.ReadKey);
            Assert.Equal("UnitTest", _codec_Received.MsgDataUnpacked.GetParam(MsgBlockData.SENDER_NICKNAME));

            Assert.Equal(_codec_Sent.InitData, _codec_Received.InitData);
            Assert.Equal(_codec_Sent.BlockData, _codec_Received.BlockData);

            var init_data = _codec_Received.DecodeSelfData(_senderSecretAddress.ReadKey);
            Assert.Equal(_blockKeys.PrivateKey.ToString(), init_data.ChatKeyBase58);

            var SelfMessageData = _codec_Received.DecodeMsgDataBySelf(_blockKeys.PrivateKey, _recipientSecretAddress.ReadPubKey.ToString());
            Assert.Equal("Hello, World!", SelfMessageData.MsgText);
        }


    }
}
