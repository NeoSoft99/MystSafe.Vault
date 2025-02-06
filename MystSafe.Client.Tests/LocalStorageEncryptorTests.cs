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

namespace MystSafe.Client.Tests
{
    public class LocalStorageEncryptorTests
    {
        [Fact]
        public void EncryptDecryptTest()
        {
           
            
            string plainText = "Hello World";
            string account_id = Guid.NewGuid().ToString();
            string fingerprint = "just a random string is fine";

            var local_storage_encryptor = new LocalStorageEncryptionService();

            var local_key = local_storage_encryptor.GenerateLocalEncryptionKey(account_id, fingerprint);

            Assert.NotNull(local_key);
            Assert.NotEmpty(local_key);

            var encrypted_text = local_storage_encryptor.EncryptString(account_id, plainText, account_id);
            var decrypted_text = local_storage_encryptor.DecryptString(account_id, encrypted_text, account_id);
            Assert.Equal(plainText, decrypted_text);

            var encrypted_bool = local_storage_encryptor.EncryptBool(account_id, true, account_id);
            var decrypted_bool = local_storage_encryptor.DecryptBool(account_id, encrypted_bool, account_id);
            Assert.True(decrypted_bool);

            var encrypted_int = local_storage_encryptor.EncryptInt(account_id, 123, account_id);
            var decrypted_int = local_storage_encryptor.DecryptInt(account_id, encrypted_int, account_id);
            Assert.Equal(123, decrypted_int);

            var encrypted_long = local_storage_encryptor.EncryptLong(account_id, 123456789L, account_id);
            var decrypted_long = local_storage_encryptor.DecryptLong(account_id, encrypted_long, account_id);
            Assert.Equal(123456789L, decrypted_long);

            var local_storage_encryptor_2 = new LocalStorageEncryptionService();

            local_storage_encryptor_2.RestoreLocalEncryptionKey(local_key, account_id, fingerprint);

            var decrypted_text_2 = local_storage_encryptor_2.DecryptString(account_id, encrypted_text, account_id);
            Assert.Equal(plainText, decrypted_text_2);
        }
    }
}

