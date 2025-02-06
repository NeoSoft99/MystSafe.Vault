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

using MystSafe.Client.CryptoLicense;
using MystSafe.Shared.Common;
using MystSafe.Shared.Crypto;

namespace MystSafe.Client.Engine;

  
    public class Account
    {
        #region These are the properties stored in the client DB

        public string Id { get; set; }

        public string Mnemonic { get; set; }
        public long Created { get; set; }
        public long LastUpdated { get; set; }
        public long LastScannedContactBlock { get; set; }
        public long LastScannedSecretBlock { get; set; }

        public string NickName { get; set; }
        public string PasswordHash { get; set; }

        public int ChatExpirationDays { get; set; }

        public string? CurrentContactId { get; set; }

        //public string? CurrentSecretId { get; set; }
        public string? CurrentSecretPubKey { get; set; }

        public bool EULAHasBeenShown { get; set; }

        public int Network { get; set; }

        //public Int64 LicenseExpirationDate { get; set; }
        //public string LicensePrivateKey { get; set; }
        //public string LicensePublicKey { get; set; }
        public int LicenseType { get; set; }
        public string Rewards { get; set; }

        // This is the credentials generated after Authn registration (biometric auth enabled)
        public string PasskeyCredentials { get; set; }

        // this is the symmetric data encryption key ecnrypted by the KEY
        // generated from a seed depending on LocalAuhtnType 
        // this is used to encryp data at rest in the browser's IndexDB
        public string LocalEncryptionKey { get; set; }

        // TO DO - key rotation using differetn key IDs
        public string LocalKeyId { get; set; }

        // One of LocalAuthnType, default is RUNTIME_FINGERPRINT
        public int LocalAuthnType { get; set; }

        // Timeout to lock the app in seconds
        public int LockTimeoutSec { get; set; } = 5 * 60; // 5 minutes default

        // This is needed for license admin access and issuing new licences and key rotations
        //public string MasterLicensePrivateKey { get; set; }

        //public string MasterLicensePubKey { get; set; }

        #endregion

        #region These are the temporary properties that shouldn't be stored in local DB

        public bool SecretEditMode { get; set; }

        //public bool HasLicense {  get { return LicenseExpirationDate > UnixDateTime.Now;  } }
        public bool HasLicense
        {
            get { return CurrentWallet.LicenseBalance > 0; }
        }

        public readonly List<Contact> Contacts = new List<Contact>();

        public readonly List<Secret> Secrets = new List<Secret>();

        // this should be set to trigger the UI (SecretDrawer) to rebuild / refresh the secrets and folders
        public bool SecretUpdateFlag { get; set; }
        
        public Wallet CurrentWallet { get; set; }

        #endregion

        public UserAddress CurrentAddress
        {
            get { return getAddress(); }
            set { _CurrentAddress = value; }
        }

        public void AddContact(Contact contact)
        {
            contact.Account = this;
            Contacts.Add(contact);
            this.LastUpdated = UnixDateTime.Now;
        }

        public void AddSecret(Secret secret)
        {
            secret.Account = this;
            Secrets.Add(secret);
            this.LastUpdated = UnixDateTime.Now;
        }

        public void UpdateSecret(Secret secret)
        {
            RemoveSecretByBlockPubKey(secret.BlockPubKey);
            secret.Account = this;
            Secrets.Add(secret);
            SecretUpdateFlag = true;
            this.LastUpdated = UnixDateTime.Now;
        }

        public Contact GetContact(string peerUserAddress)
        {
            int index = Contacts.FindIndex(contact => contact.PeerUserAddress == peerUserAddress);
            if (index != -1)
            {
                return Contacts[index];
            }

            return null;
        }

        public Secret GetSecretByTitle(string title)
        {
            int index = Secrets.FindIndex(secret => secret.Data.Title == title);
            if (index != -1)
            {
                return Secrets[index];
            }

            return null;
        }
        
        public Secret GetSecretByPubKey(string secret_pub_key)
        {
            int index = Secrets.FindIndex(secret => secret.BlockPubKey == secret_pub_key);
            if (index != -1)
            {
                return Secrets[index];
            }

            return null;
        }

        public bool RemoveContact(string peerUserAddress)
        {
            int indexToRemove = Contacts.FindIndex(contact => contact.PeerUserAddress == peerUserAddress);
            if (indexToRemove != -1)
            {
                Contacts.RemoveAt(indexToRemove);
                this.LastUpdated = UnixDateTime.Now;
                return true;
            }

            return false;
        }

        public bool RemoveContactByBlockPubKey(string block_pub_key)
        {
            int indexToRemove = Contacts.FindIndex(secret => secret.BlockPublicKey == block_pub_key);
            if (indexToRemove != -1)
            {
                Contacts.RemoveAt(indexToRemove);
                this.LastUpdated = UnixDateTime.Now;
                return true;
            }

            return false;
        }

        public Contact CurrentContact
        {
            get { return getContact(); }
        }

        private Contact getContact()
        {
            if (!string.IsNullOrWhiteSpace(CurrentContactId) && Contacts.Count() > 0)
            {
                foreach (var contact in Contacts)
                    if (contact.Id == CurrentContactId)
                        return contact;
            }

            return null;
        }

        public Secret CurrentSecret
        {
            get { return getSecret(); }
        }

        private Secret getSecret()
        {
            if (!string.IsNullOrWhiteSpace(CurrentSecretPubKey) && Secrets.Count() > 0)
            {
                foreach (var secret in Secrets)
                    if (secret.BlockPubKey == CurrentSecretPubKey)
                        return secret;
            }

            return null;
        }

        public bool RemoveSecretByBlockPubKey(string block_pub_key)
        {
            int indexToRemove = Secrets.FindIndex(secret => secret.BlockPubKey == block_pub_key);
            if (indexToRemove != -1)
            {
                Secrets.RemoveAt(indexToRemove);
                this.LastUpdated = UnixDateTime.Now;
                return true;
            }

            return false;
        }
        
        private UserAddress _CurrentAddress;

        private UserAddress getAddress()
        {
            if (_CurrentAddress == null)
                _CurrentAddress = UserAddress.RestoreFromMnemonic(Mnemonic, Network);
            return _CurrentAddress;
        }
        
        public Account()
        {
        }

        public void Clear()
        {
            foreach (var contact in Contacts)
                contact.ClearChats();
            Contacts.Clear();
            Secrets.Clear();
        }
        
        public List<string> FindReferencesToTxPublicKeys(List<string> txPublicKeys)
        {
            var matchingContacts = Contacts
                .Where(contact => txPublicKeys.Contains(contact.LicensePubKey))
                .Select(contact => contact.LicensePubKey)
                .ToList();

            var matchingSecrets = Secrets
                .Where(secret => txPublicKeys.Contains(secret.LicensePubKey))
                .Select(secret => secret.LicensePubKey)
                .ToList();

            var matchingChats = Contacts
                .SelectMany(contact => contact.ChatsOut)
                .Where(chat => txPublicKeys.Contains(chat.LicensePubKey))
                .Select(chat => chat.LicensePubKey)
                .ToList();

            var allMatches = matchingContacts
                .Union(matchingSecrets)
                .Union(matchingChats)
                .Distinct()
                .ToList();

            return allMatches;
        }
    }


    


