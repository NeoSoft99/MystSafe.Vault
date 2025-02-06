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
    public class SecretBlockFixture
    {
        public SecretBlockEncoder secretBlock_Sent { get; private set; }
        public SecretBlockDecoder secretBlock_Received { get; private set; }
        public KeyPair blockKeys { get; private set; }

        public UserAddress userAddress { get; private set; }
        
        //public SecretBlockData secretData_Sent { get; private set; } 
        
        //public SecretBlockData secretData_Received { get; private set; } 
        
        public int secretType_Sent { get; private set; }
        //public int secretType_Received { get; private set; }
        
        public string secretTitle_Sent { get; private set; }
        //public string secretTitle_Received { get; private set; }
        
        public string secretLogin_Sent { get; private set; }
        //public string secretLogin_Received { get; private set; }
        

        public SecretBlockFixture()
        {
            // Arrange
            blockKeys = KeyPair.GenerateRandom(); 
            userAddress = UserAddress.GenerateFromMnemonic(Networks.devnet);

            var senderNickName = "UnitTest";
            secretType_Sent = SecretTypes.Login;
            secretTitle_Sent = "Login Test";
            secretLogin_Sent = "Test Login";
            
            var secretData_Sent = SecretBlockData.New();
            secretData_Sent.SecretType = secretType_Sent;
            secretData_Sent.Title = secretTitle_Sent;
            secretData_Sent.Login = secretLogin_Sent;
         
            // Act
            secretBlock_Sent = new SecretBlockEncoder(
                blockKeys,
                userAddress,
                0,
                string.Empty,
                secretData_Sent,
                null,
                Networks.devnet,
                Constants.FREE_LICENSE_TYPE,
                null);
            
            Task<SecretBlock> task = secretBlock_Sent.Encode();
            var block = task.GetAwaiter().GetResult();

            secretBlock_Received = new SecretBlockDecoder(block);

            secretBlock_Received.Decode(userAddress.ReadKey);
           
        }
    }

    public class SecretBlockTests : IClassFixture<SecretBlockFixture>
    {
        private readonly SecretBlockEncoder _secretBlock_Sent;
        private readonly SecretBlockDecoder _secretBlock_Received;
        private readonly KeyPair _blockKeys;
        private readonly UserAddress _userAddress;
        public int _secretType_Sent;
        public string _secretTitle_Sent;
        public string _secretLogin_Sent;
        
        public SecretBlockTests(SecretBlockFixture fixture)
        {
            _secretBlock_Sent = fixture.secretBlock_Sent;
            _secretBlock_Received = fixture.secretBlock_Received;
            _blockKeys = fixture.blockKeys;
            _userAddress = fixture.userAddress;
            _secretType_Sent = fixture.secretType_Sent;
            _secretTitle_Sent = fixture.secretTitle_Sent;
            _secretLogin_Sent = fixture.secretLogin_Sent;
        }
        
        [Fact]
        public void TestSecretBLockSignature()
        {
            // Assert
            Assert.NotNull(_secretBlock_Sent);
            Assert.NotNull(_secretBlock_Received);

            Assert.Equal(_secretBlock_Sent.Hash, _secretBlock_Received.Hash);
            Assert.Equal(_secretBlock_Sent.Signature, _secretBlock_Received.Signature);

            var exception = Record.Exception(() => _secretBlock_Sent.ServerValidate());
            Assert.Null(exception);
            exception = Record.Exception(() => _secretBlock_Received.ClientValidate());
            Assert.Null(exception);
        }

        [Fact]
        public void TestSecretBlockData()
        {

            Assert.Equal(_secretBlock_Sent.BlockData, _secretBlock_Received.BlockData);

            var secret_data = _secretBlock_Received.Decode(_userAddress.ReadKey);
            //Assert.Equal(_userAddress.ToString(), secret_data.Address);

            Assert.Equal(_secretType_Sent, secret_data.SecretType);
            Assert.Equal(_secretTitle_Sent, secret_data.Title);
            Assert.Equal(_secretLogin_Sent, secret_data.Login);
        }


    }
}
