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
    public class ContactBlockBuilderFixture
    {
        public ContactBlockEncoder ContactBlockBuilder_Sent { get; private set; }
        public ContactBlockDecoder ContactBlockBuilder_Received { get; private set; }
        public KeyPair BlockKeys { get; private set; }
        public UserAddress RecipientUserAddress { get; private set; }
        public UserAddress SenderUserAddress { get; private set; }

        public ContactBlockBuilderFixture()
        {
            // Arrange
            BlockKeys = KeyPair.GenerateRandom(); 
            SenderUserAddress = UserAddress.GenerateFromMnemonic(Networks.devnet);
            RecipientUserAddress = UserAddress.GenerateFromMnemonic(Networks.devnet);

            var senderNickName = "UnitTest";
            var senderSecretAddress = SecretAddress.GenerateFromPeerAddress(SenderUserAddress, RecipientUserAddress.ToString(), Networks.devnet); // Initialize with appropriate values
            var command = ContactRequestCommands.InitialRequest;

            // Act
            ContactBlockBuilder_Sent = new ContactBlockEncoder(
                BlockKeys,
                RecipientUserAddress.ToString(),
                SenderUserAddress.ToString(),
                senderNickName,
                senderSecretAddress,
                command,
                Networks.devnet,
                false,
                0,
                string.Empty,
                string.Empty,
                Constants.FREE_LICENSE_TYPE
            );

            Task<ContactBlock> task = ContactBlockBuilder_Sent.Encode();
            var block = task.GetAwaiter().GetResult();
            //var block = ContactBlockBuilder_Sent.Encode();

            ContactBlockBuilder_Received = new ContactBlockDecoder(block);

            ContactBlockBuilder_Received.Decode(RecipientUserAddress.ReadKey);

            ContactBlockBuilder_Received.DecodeSelfData(SenderUserAddress.ReadKey);


           
        }
    }

    public class ContactBlockBuilderTests : IClassFixture<ContactBlockBuilderFixture>
    {
        private readonly ContactBlockEncoder _contactBlockBuilder_Sent;
        private readonly ContactBlockDecoder _contactBlockDecoder_Received;
        private readonly KeyPair _blockKeys;
        private readonly UserAddress _recipientUserAddress;
        private readonly UserAddress _senderUserAddress;

        public ContactBlockBuilderTests(ContactBlockBuilderFixture fixture)
        {
            _contactBlockBuilder_Sent = fixture.ContactBlockBuilder_Sent;
            _contactBlockDecoder_Received = fixture.ContactBlockBuilder_Received;
            _blockKeys = fixture.BlockKeys;
            _recipientUserAddress = fixture.RecipientUserAddress;
            _senderUserAddress = fixture.SenderUserAddress;
            
        }

        [Fact]
        public void TestContactBlockBuilderSignature()
        {
            // Assert
            Assert.NotNull(_contactBlockBuilder_Sent);
            Assert.NotNull(_contactBlockDecoder_Received);

            Assert.Equal(_contactBlockBuilder_Sent.Hash, _contactBlockDecoder_Received.Hash);
            Assert.Equal(_contactBlockBuilder_Sent.Signature, _contactBlockDecoder_Received.Signature);

            var exception = Record.Exception(() => _contactBlockBuilder_Sent.ServerValidate());
            Assert.Null(exception);
            exception = Record.Exception(() => _contactBlockDecoder_Received.ClientValidate());
            Assert.Null(exception);
        }

        [Fact]
        public void TestContactBlockBuilderData()
        {

            Assert.Equal(_contactBlockBuilder_Sent.InitData, _contactBlockDecoder_Received.InitData);
            Assert.Equal(_contactBlockBuilder_Sent.BlockData, _contactBlockDecoder_Received.BlockData);

            var init_data = _contactBlockDecoder_Received.DecodeSelfData(_senderUserAddress.ReadKey);
            Assert.Equal(_recipientUserAddress.ToString(), init_data.RecipientUserAddress);

            Assert.Equal("UnitTest", _contactBlockDecoder_Received.MsgDataUnpacked.GetParam(MsgBlockData.SENDER_NICKNAME));
        }


    }
}
