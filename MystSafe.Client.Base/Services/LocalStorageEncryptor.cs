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

using System.Security.Cryptography;
using MystSafe.Shared.Crypto;

namespace MystSafe.Client.Base;

public class LocalStorageEncryptionService

{
    //private string _localEncryptionKey { get; set; }

    //private byte[] _decrypted_local_encryption_key { get; set; }
    private Dictionary<string, byte[]> _decrypted_local_encryption_keys = new Dictionary<string, byte[]>();

    // Method to add a key
    private void AddDecryptedKey(string account_id, byte[] key)
    {
        if (key == null || account_id == null)
            throw new ArgumentNullException();

        ResetLocalEncryptionKey(account_id);

        _decrypted_local_encryption_keys[account_id] = key;
    }

    // Method to retrieve a key
    private byte[] GetDecryptedKey(string account_id)
    {
        if (account_id == null)
            throw new ArgumentNullException();

        _decrypted_local_encryption_keys.TryGetValue(account_id, out byte[] key);
        return key;
    }

    public void ResetLocalEncryptionKey(string account_id)
    {
        if (account_id == null)
            throw new ArgumentNullException();

        if (GetDecryptedKey(account_id) == null)
            return;

        EraseByteArray(_decrypted_local_encryption_keys[account_id]);
        //_decrypted_local_encryption_key = null;
        _decrypted_local_encryption_keys.Remove(account_id);
    }

    public LocalStorageEncryptionService()
	{
	}

	public string EncryptString(string account_id, string clear_text_data, string salt)
	{
        //Console.WriteLine("_decrypted_local_encryption_key: " + GetDecryptedKey(account_id));
        return AES.Encrypt(GetDecryptedKey(account_id), salt, clear_text_data);
    }

    public string EncryptBool(string account_id, bool clear_text_data_bool, string salt)
    {
        string clear_text_data = clear_text_data_bool.ToString();
        return AES.Encrypt(GetDecryptedKey(account_id), salt, clear_text_data);
    }

    public string EncryptInt(string account_id, int clear_text_data_int, string salt)
    {
        string clear_text_data = clear_text_data_int.ToString();
        return AES.Encrypt(GetDecryptedKey(account_id), salt, clear_text_data);
    }

    public string EncryptLong(string account_id, long clear_text_data_long, string salt)
    {
        string clear_text_data = clear_text_data_long.ToString();
        return AES.Encrypt(GetDecryptedKey(account_id), salt, clear_text_data);
    }

    public string DecryptString(string account_id, string cypher_text_data, string salt)
    {
        return AES.Decrypt(GetDecryptedKey(account_id), salt, cypher_text_data);
    }

    public bool DecryptBool(string account_id, string cypher_text_data, string salt)
    {
        string decrypted_text = AES.Decrypt(GetDecryptedKey(account_id), salt, cypher_text_data);
        bool result;
        bool.TryParse(decrypted_text, out result);
        return result;
    }

    public int DecryptInt(string account_id, string cypher_text_data, string salt)
    {
        string decrypted_text = AES.Decrypt(GetDecryptedKey(account_id), salt, cypher_text_data);
        int result;
        int.TryParse(decrypted_text, out result);
        return result;
    }

    public long DecryptLong(string account_id, string cypher_text_data, string salt)
    {
        string decrypted_text = AES.Decrypt(GetDecryptedKey(account_id), salt, cypher_text_data);
        long result;
        long.TryParse(decrypted_text, out result);
        return result;
    }

    // seed should be set to runtime fingerprint value if authn mode is RUNTIME_FINGERPRINT, or
    // secret id retrieved from passkey (biometric) assertion if authn mode is PASSKEY
    public string GenerateLocalEncryptionKey(string account_id, string seed)
    {
        if (string.IsNullOrEmpty(seed) || string.IsNullOrEmpty(account_id))
            throw new Exception("parameters not set");

        //_decrypted_local_encryption_key = GenerateRandomKey();
        AddDecryptedKey(account_id, GenerateRandomKey());
        var kek = ConstructKEK(seed, account_id); // this is stored in memory only
        var local_key_bytes = AES.Encrypt(GetDecryptedKey(account_id), kek, account_id);
        var localEncryptionKey = Codecs.FromBytesToBase58(local_key_bytes);
        return localEncryptionKey;
    
    }

    public void RestoreLocalEncryptionKey(string encrypted_local_encryption_key, string account_id, string seed)
    {
        if (string.IsNullOrEmpty(seed) || string.IsNullOrEmpty(account_id) || string.IsNullOrEmpty(encrypted_local_encryption_key))
            throw new Exception("parameters not set");

        var kek = ConstructKEK(seed, account_id); // this is stored in memory only
        var encrypted_bytes = Codecs.FromBase58ToBytes(encrypted_local_encryption_key);
        //_decrypted_local_encryption_key = AES.Decrypt(encrypted_bytes, kek, account_id);
        AddDecryptedKey(account_id, AES.Decrypt(encrypted_bytes, kek, account_id));
    }

    // Use this method when changing authn mode from passkey to fingerprint and vice versa;
    // the _decrypted_local_encryption_key is assumed to be present already, otherwise, it fails to prevent unauthorized access to data
    public string ReencryptLocalEncryptionKey(string account_id, string new_seed)
    {
        if (string.IsNullOrEmpty(new_seed) || string.IsNullOrEmpty(account_id) || GetDecryptedKey(account_id) is null)
            throw new Exception("parameters not set");

        var kek = ConstructKEK(new_seed, account_id); // this is stored in memory only
        var local_key_bytes = AES.Encrypt(GetDecryptedKey(account_id), kek, account_id);
        var localEncryptionKey = Codecs.FromBytesToBase58(local_key_bytes);
        return localEncryptionKey;
    }

  
    private void EraseByteArray(byte[] byteArray)
    {
        if (byteArray == null)
            return;

        for (int i = 0; i < byteArray.Length; i++)
        {
            byteArray[i] = 0;
        }
    }

    private byte[] GenerateRandomKey()
    {
        byte[] randomKey = new byte[32]; 
        RandomNumberGenerator.Fill(randomKey);
        return randomKey;
    }

    private byte[] ConstructKEK(string seed, string salt)
    {
        return KDF.GetPasswordKey(Codecs.FromASCIIToBytes(seed), salt);
    }




}


