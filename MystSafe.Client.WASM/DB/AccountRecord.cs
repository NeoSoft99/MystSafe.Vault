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
using MystSafe.Client.Engine;

namespace MystSafe.Client.App;

public class AccountRecord
{
    [System.ComponentModel.DataAnnotations.Key]
    public string Id { get; set; }

    public string Mnemonic { get; set; }
    public long Created { get; set; }
    public long LastUpdated { get; set; }
    public long LastScannedContactBlock { get; set; }
    public long LastScannedSecretBlock { get; set; }

    public string NickName { get; set; }
    public string? PasswordHash { get; set; }
    //public bool IsTest { get; set; }

    public int ChatExpirationDays { get; set; }

    public string? CurrentContactId { get; set; }

    public string CurrentSecretId { get; set; }

    //public bool SecretEditMode { get; set; }

    public bool EULAHasBeenShown { get; set; }

    public int Network { get; set; }

    public Int64 LicenseExpirationDate { get; set; }
    public string LicensePrivateKey { get; set; }
    public string LicensePublicKey { get; set; }
    public string LicenseType { get; set; }
    public string Rewards { get; set; }

    // This is the creentials generated after Authn registration (biometric auth enabled)
    public string? PasskeyCredentials { get; set; }

    // this is the symmetric data encryption key ecnrypted by Authn secret (biometric auth)
    // this is used to encryp data at rest in the browser's IndexDB
    public string LocalEncryptionKey { get; set; }

    public string LocalKeyId { get; set; }

    // One of LocalAuthnType, default is RUNTIME_FINGERPRINT
    public int LocalAuthnType { get; set; }

    // Timeout to lock the app in secondsm default 1 minute
    public int LockTimeoutSec { get; set; }

    // This is needed for license admin access and issuing new licences and key rotations
    public string MasterLicensePrivateKey { get; set; }

    public string MasterLicensePubKey { get; set; }

    public AccountRecord()
    {

    }

    //public AccountRecord(Account account)
    //{

    //}

    public static AccountRecord AccountToRecord(Account account, LocalStorageEncryptionService local_storage_encryptor)
    {
        var account_record = new AccountRecord();
        account_record.Id = account.Id;

        account_record.Mnemonic =
            local_storage_encryptor.EncryptString(account.Id, account.Mnemonic, account.Id + nameof(account.Mnemonic));

        account_record.Created = account.Created;
        account_record.LastUpdated = account.LastUpdated;
        account_record.LastScannedContactBlock = account.LastScannedContactBlock;
        account_record.LastScannedSecretBlock = account.LastScannedSecretBlock;
        account_record.NickName = account.NickName;
        account_record.PasswordHash = account.PasswordHash;
        account_record.ChatExpirationDays = account.ChatExpirationDays;
        account_record.CurrentContactId = account.CurrentContactId;
        account_record.EULAHasBeenShown = account.EULAHasBeenShown;
        account_record.Network = account.Network;

        // account_record.LicenseExpirationDate = account.LicenseExpirationDate;
        // account_record.LicensePrivateKey = local_storage_encryptor.EncryptString(account.Id, account.LicensePrivateKey,
        //     account.Id + nameof(account.LicensePrivateKey));
        // account_record.LicensePublicKey = local_storage_encryptor.EncryptString(account.Id, account.LicensePublicKey,
        //     account.Id + nameof(account.LicensePublicKey));
         account_record.LicenseType = local_storage_encryptor.EncryptInt(account.Id, account.LicenseType,
            account.Id + nameof(account.LicenseType));
        account_record.Rewards =
            local_storage_encryptor.EncryptString(account.Id, account.Rewards, account.Id + nameof(account.Rewards));

        account_record.PasskeyCredentials = account.PasskeyCredentials;
        account_record.LocalEncryptionKey = account.LocalEncryptionKey;
        account_record.LocalKeyId = account.LocalKeyId;
        account_record.LocalAuthnType = account.LocalAuthnType;

        account_record.LockTimeoutSec = account.LockTimeoutSec;

        // account_record.MasterLicensePrivateKey = local_storage_encryptor.EncryptString(account.Id,
        //     account.MasterLicensePrivateKey, account.Id + nameof(account.MasterLicensePrivateKey));
        // account_record.MasterLicensePubKey = local_storage_encryptor.EncryptString(account.Id,
        //     account.MasterLicensePubKey, account.Id + nameof(account.MasterLicensePubKey));


        return account_record;
    }

    public static Account RecordToAccount(AccountRecord account_record,
        LocalStorageEncryptionService local_storage_encryptor)
    {
        var account = new Account();
        account.Id = account_record.Id;

        account.Mnemonic = local_storage_encryptor.DecryptString(account.Id, account_record.Mnemonic,
            account_record.Id + nameof(account.Mnemonic));

        account.Created = account_record.Created;
        account.LastUpdated = account_record.LastUpdated;
        account.LastScannedContactBlock = account_record.LastScannedContactBlock;
        account.LastScannedSecretBlock = account_record.LastScannedSecretBlock;
        account.NickName = account_record.NickName;
        account.PasswordHash = account_record.PasswordHash;
        account.ChatExpirationDays = account_record.ChatExpirationDays;
        account.CurrentContactId = account_record.CurrentContactId;
        account.EULAHasBeenShown = account_record.EULAHasBeenShown;
        account.Network = account_record.Network;

        // account.LicenseExpirationDate = account_record.LicenseExpirationDate;
        // account.LicensePrivateKey = local_storage_encryptor.DecryptString(account.Id, account_record.LicensePrivateKey,
        //     account_record.Id + nameof(account.LicensePrivateKey));
        // account.LicensePublicKey = local_storage_encryptor.DecryptString(account.Id, account_record.LicensePublicKey,
        //     account_record.Id + nameof(account.LicensePublicKey));
        account.LicenseType = local_storage_encryptor.DecryptInt(account.Id, account_record.LicenseType,
            account_record.Id + nameof(account.LicenseType));
        account.Rewards = local_storage_encryptor.DecryptString(account.Id, account_record.Rewards,
            account_record.Id + nameof(account.Rewards));

        account.PasskeyCredentials = account_record.PasskeyCredentials;
        account.LocalEncryptionKey = account_record.LocalEncryptionKey;
        account.LocalKeyId = account_record.LocalKeyId;
        account.LocalAuthnType = account_record.LocalAuthnType;

        account.LockTimeoutSec = account_record.LockTimeoutSec;

        // account.MasterLicensePrivateKey = local_storage_encryptor.DecryptString(account.Id,
        //     account_record.MasterLicensePrivateKey, account_record.Id + nameof(account.MasterLicensePrivateKey));
        // account.MasterLicensePubKey = local_storage_encryptor.DecryptString(account.Id,
        //     account_record.MasterLicensePubKey, account_record.Id + nameof(account.MasterLicensePubKey));

        return account;
    }

    public void Update(AccountRecord new_record)
    {
        if (this.Id != new_record.Id)
            throw new Exception("Account Id does not match");
        this.Mnemonic = new_record.Mnemonic;
        this.Created = new_record.Created;
        this.LastUpdated = new_record.LastUpdated;
        this.LastScannedContactBlock = new_record.LastScannedContactBlock;
        this.LastScannedSecretBlock = new_record.LastScannedSecretBlock;
        this.NickName = new_record.NickName;
        this.PasswordHash = new_record.PasswordHash;
        this.ChatExpirationDays = new_record.ChatExpirationDays;
        this.CurrentContactId = new_record.CurrentContactId;
        this.EULAHasBeenShown = new_record.EULAHasBeenShown;
        this.Network = new_record.Network;
        this.LicenseExpirationDate = new_record.LicenseExpirationDate;
        this.LicensePrivateKey = new_record.LicensePrivateKey;
        this.LicensePublicKey = new_record.LicensePublicKey;
        this.LicenseType = new_record.LicenseType;
        this.Rewards = new_record.Rewards;
        this.PasskeyCredentials = new_record.PasskeyCredentials;
        this.LocalEncryptionKey = new_record.LocalEncryptionKey;
        this.LocalKeyId = new_record.LocalKeyId;
        this.LocalAuthnType = new_record.LocalAuthnType;
        this.LockTimeoutSec = new_record.LockTimeoutSec;
        this.MasterLicensePrivateKey = new_record.MasterLicensePrivateKey;
        this.MasterLicensePubKey = new_record.MasterLicensePubKey;
    }
}

