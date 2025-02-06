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

using System.Security;
using MystSafe.Shared.Crypto;
using MystSafe.Shared.Common;
using MystSafe.Client.Base;

namespace MystSafe.Client.Engine;

public class SelfTest
{
    const string TEST_MSG_1_OUT = "[1.OUT] Hello, World!";
    const string TEST_MSG_2_OUT = "[2.OUT] How are you?";


    const string TEST_MSG_THIRD_TEXT = "[1.3] Is everything OK?";

    const string TEST_MSG_REPLY_TEXT = "[2.1] Hey, what's up?";
    const string TEST_MSG_REPLY_SECOND_TEXT = "[2.2] How are you doing?";
    const string TEST_MSG_REPLY_THIRD_TEXT = "[2.3] I am fine! and you?";

    //const string test_account_id_1 = "11111111-1111-1111-1111-111111111111";
    string test_account_id_1;
    string test_account_mnemonic_1;


    string test_account_id_2;
    string test_account_mnemonic_2;

    string test_account_id_3;
    string test_account_mnemonic_3;

    string test_account_id_5;
    string test_account_mnemonic_5;

    string QUICK_TEST_ADDRESS_2;
    string QUICK_TEST_ADDRESS_4;

    private SendProcessor _sp;
    public Account test_sender_account;
    public Account test_recipient_account;
    public Account account_3;
    public Account account_5;
    private UpdateProcessor _up;

    private string instant_share_link = string.Empty;

    public SelfTest(SendProcessor sp)
    {
        _sp = sp;

    }

    public async Task<Account> InitializeQuickTest(Account current_account)
    {

        if (current_account == null)
        {
            string nickname =
                TrimAccountNickname(SendProcessor.SELF_TEST_SENDER_ACCOUNT + " " + DateTime.Now.ToShortTimeString());

            test_sender_account = await _sp.CreateNewAccount(nickname, Networks.devnet);
            test_account_id_1 = test_sender_account.Id;

            QUICK_TEST_ADDRESS_2 = UserAddress.GenerateFromMnemonic(Networks.devnet).ToString();
            QUICK_TEST_ADDRESS_4 = UserAddress.GenerateFromMnemonic(Networks.devnet).ToString();

        }
        else
        {
            test_sender_account = current_account;
        }

        return test_sender_account;
    }

    private string TrimAccountNickname(string nickname)
    {
        if (nickname.Length > ClientConstants.MAX_ACCOUNT_NICKNAME_LENGTH_BYTES)
            return nickname.Substring(0, ClientConstants.MAX_ACCOUNT_NICKNAME_LENGTH_BYTES);
        else
            return nickname;
    }

    public async Task<Account> InitializeStandardTest(Account current_account)
    {
        await InitializeQuickTest(current_account);
        string nickname = TrimAccountNickname(SendProcessor.TEST_ACCOUNT_3 + " " + DateTime.Now.ToShortTimeString());

        test_recipient_account = await _sp.CreateNewAccount(nickname, Networks.devnet);
        test_account_id_3 = test_recipient_account.Id;

        _up = new UpdateProcessor(
            false,
            _sp._Account_Db,
            _sp._Contact_Db,
            _sp._ChatIn_Db,
            _sp._ChatOut_Db,
            _sp._Message_Db,
            _sp._Secret_Db,
            _sp.Logger,
            _sp._apiClientService
        );

        return test_sender_account;
    }


    public async Task<Account> RestoreFromNetworkTest(Account current_account)
    {
        Account restored_account;
        try
        {
            string mnemonic;
            int network;
            string nickname;

            if (current_account == null)
            {
                //current_account = await InitializeQuickTest(current_account);
                //await QuickTest();
                throw new Exception("No account");
            }

            mnemonic = current_account.Mnemonic.ToString();
            network = current_account.Network;
            nickname = TrimAccountNickname("[Restored] " + current_account.NickName);


            await _sp.DeleteAccount(current_account);

            restored_account = await _sp.RestoreAccount(mnemonic, nickname, network);
            if (restored_account == null)
                throw new Exception("Unknown Error");

            //var secrets_found = await _sp.LookForSecrets(restored_account);
            //if (!secrets_found)
            //    throw new Exception("No secrets restored");
            if (restored_account.Secrets.Count == 0)
                throw new Exception("No secrets restored");

            //var contacts = await _sp.LookForNewContactRequests(restored_account);
            //if (contacts.Count == 0)
            //    throw new Exception("No contacts restored");
            if (restored_account.Contacts.Count == 0)
                throw new Exception("No contacts restored");

            return restored_account;
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("RestoreFromNetworkTest() exception: " + e.Message), e);
        }
    }

    public async Task<Account> RetrieveTest()
    {
        //Account account;
        try
        {

            var result = await _sp.GetRecentAccountAsync();

            if (result.ResultCode != ResultStatusCodes.SUCCESS)
            {
                throw new Exception(result.ResultMessage);
            }

            return result.ResultAccount ?? throw new Exception("Account is null");
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("Failed to retrieve account: " + e.Message), e);
        }
    }


    public async Task QuickTest()
    {

        try
        {
            SendContactRequestResult contact_result =
                await _sp.NewContactRequest(test_sender_account, QUICK_TEST_ADDRESS_2);
            if (contact_result.ResultCode != ResultStatusCodes.SUCCESS)
            {
                throw new Exception("Failed to create contact QUICK_TEST_ADDRESS_2: " + contact_result.ResultMessage);
            }

            contact_result = await _sp.NewContactRequest(test_sender_account, QUICK_TEST_ADDRESS_4);
            if (contact_result.ResultCode != ResultStatusCodes.SUCCESS)
            {
                throw new Exception("Failed to create contact QUICK_TEST_ADDRESS_4: " + contact_result.ResultMessage);
            }

            contact_result = await _sp.DeleteContact(contact_result.NewContact);
            if (contact_result.ResultCode != ResultStatusCodes.SUCCESS)
            {
                throw new Exception("Failed to delete contact QUICK_TEST_ADDRESS_4: " + contact_result.ResultMessage);
            }

            contact_result = await _sp.NewContactRequest(test_sender_account, QUICK_TEST_ADDRESS_4);
            if (contact_result.ResultCode != ResultStatusCodes.SUCCESS)
            {
                throw new Exception("Failed to create contact QUICK_TEST_ADDRESS_4: " + contact_result.ResultMessage);
            }

            var secret_data = SecretBlockData.New();
            secret_data.SecretType = SecretTypes.Login;
            secret_data.Title = "Netfix";
            secret_data.Notes = "Family account";
            secret_data.Login = "netflix-admin";
            secret_data.Password = "12345678";
            secret_data.URL = "https://netflix.com";

            var secret_result = await _sp.AddNewSecret(test_sender_account, secret_data);
            if (secret_result.ResultCode != ResultStatusCodes.SUCCESS)
            {
                throw new Exception(secret_result.ResultMessage);
            }

            //var secret_id = secret_result.NewSecret.Data.SecretId;
            //test_sender_account.CurrentSecretId = secret_id;
            test_sender_account.CurrentSecretPubKey = secret_result.NewSecret.BlockPubKey;

            // update secret
            secret_data = SecretBlockData.New();
            secret_data.GlobalId = secret_result.NewSecret.Data.GlobalId;
            secret_data.SecretType = SecretTypes.Login;
            secret_data.Title = "Netfix Updated";
            secret_data.Notes = "Family account";
            secret_data.Login = "netflix-admin";
            secret_data.Password = "12345678";
            secret_data.URL = "https://netflix.com";
            //secret_data.SecretId = secret_id;

            secret_result = await _sp.UpdateSecret(secret_result.NewSecret, secret_data);
            if (secret_result.ResultCode != 0)
            {
                throw new Exception(secret_result.ResultMessage);
            }

/* To Do next phase
secret_data = SecretBlockData.New();
secret_data.SecretType = SecretTypes.Application;
secret_data.RuntimeType = RuntimeTypes.Mac_MacOS;
secret_data.Title = "API Key for MAC";
secret_data.Notes = "MacOS";
secret_data.Login = "param 1";
secret_data.Password = "00000000";

secret_data.AddRuntimeParam(RuntimeVariables.HOST_NAME, "mycomputer");
secret_data.AddRuntimeParam(RuntimeVariables.MAC_ADDRESS, "00:00:00:00:00:00");
secret_data.AddRuntimeParam(RuntimeVariables.MOTHERBOARD, "\"1234567890\"");
secret_data.AddRuntimeParam(RuntimeVariables.LOCAL_IP_ADDRESS, "192.168.1.112");
secret_data.AddRuntimeParam(RuntimeVariables.USER_NAME, "admin");
secret_data.AddRuntimeParam(RuntimeVariables.CLIENT_KEY, "CDeGDeiV3VY47ZmMAKg8Wayd1aofPpDhm");

secret_result = await _sp.AddNewSecret(test_sender_account, secret_data);
if (secret_result.ResultCode != ResultStatusCodes.SUCCESS)
{
throw new Exception(secret_result.ResultMessage);
}
*/

            secret_data = SecretBlockData.New();
            secret_data.SecretType = SecretTypes.BankAccount;
            secret_data.Title = "Third Secret";
            secret_data.Notes = "Bank checking account";
            secret_data.Login = "mybank";
            secret_data.Password = "12345678abcd";
            secret_data.URL = "https://bank.com";
            secret_data.PAN = "1234567890";
            secret_data.RoutingNumber = "abcd1234567";

            secret_result = await _sp.AddNewSecret(test_sender_account, secret_data);
            if (secret_result.ResultCode != ResultStatusCodes.SUCCESS)
            {
                throw new Exception(secret_result.ResultMessage);
            }

            secret_data = SecretBlockData.New();
            secret_data.SecretType = SecretTypes.BankAccount;
            secret_data.Title = "You should not see this one";
            secret_data.Notes = "It must be deleted";
            secret_data.Login = "login";
            secret_data.Password = "password";
            secret_data.URL = "https://chase.com";
            secret_data.PAN = "1234567890";
            secret_data.RoutingNumber = "abcd1234567";

            secret_result = await _sp.AddNewSecret(test_sender_account, secret_data);
            if (secret_result.ResultCode != ResultStatusCodes.SUCCESS)
            {
                throw new Exception(secret_result.ResultMessage);
            }

            secret_result = await _sp.DeleteSecret(secret_result.NewSecret);
            if (secret_result.ResultCode != ResultStatusCodes.SUCCESS)
            {
                throw new Exception(secret_result.ResultMessage);
            }

            secret_data = SecretBlockData.New();
            secret_data.SecretType = SecretTypes.Folder;
            secret_data.Title = "You should not see this folder";


            secret_result = await _sp.AddNewSecret(test_sender_account, secret_data);
            if (secret_result.ResultCode != ResultStatusCodes.SUCCESS)
            {
                throw new Exception(secret_result.ResultMessage);
            }

            secret_result = await _sp.DeleteSecret(secret_result.NewSecret);
            if (secret_result.ResultCode != ResultStatusCodes.SUCCESS)
            {
                throw new Exception(secret_result.ResultMessage);
            }

            secret_data = SecretBlockData.New();
            secret_data.SecretType = SecretTypes.Folder;
            secret_data.Title = "The Folder";


            secret_result = await _sp.AddNewSecret(test_sender_account, secret_data);
            if (secret_result.ResultCode != ResultStatusCodes.SUCCESS)
            {
                throw new Exception(secret_result.ResultMessage);
            }

        }
        catch (Exception e)
        {
            throw new Exception(string.Format("Exception in QuickTest: " + e.Message), e);
        }
    }

    public async Task StandardTestStep1()
    {
//Contact new_contact = null;
        SendContactRequestResult result;
        try
        {
            //if (Debugger.IsAttached)
            {
                Console.WriteLine("NewContactRequest() ----------->");
                Console.WriteLine("test_sender_account.CurrentAddress.ScanPubKey: " + test_sender_account.CurrentAddress.ScanPubKey.ToString());
                Console.WriteLine("test_sender_account.CurrentAddress.ScanKey: " + test_sender_account.CurrentAddress.ScanKey.ToString());
                Console.WriteLine("test_recipient_account.CurrentAddress.ScanPubKey: " + test_recipient_account.CurrentAddress.ScanPubKey.ToString());
                Console.WriteLine("test_recipient_account.CurrentAddress.ScanKey: " + test_recipient_account.CurrentAddress.ScanKey.ToString());
              
            }
            result = await _sp.NewContactRequest(test_sender_account, test_recipient_account.CurrentAddress.ToString());
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new Exception("StandardTestStep1() failed: " + result.ResultMessage);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("StandardTestStep1() exception: " + e.Message), e);
        }
    }

    public async Task StandardTestStep2()
    {
//Contact new_contact = null;

        try
        {
            var contacts = await _sp.LookForNewContactRequests(test_recipient_account);
            if (contacts.Count == 0)
                throw new Exception("contacts.Count == 0");
            test_recipient_account.CurrentContactId = contacts[0].Id;
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("StandardTestStep2() exception: " + e.Message), e);
        }
    }

    public async Task StandardTestStep3()
    {
//Contact new_contact = null;
        SendContactRequestResult result;
        try
        {
            result = await _sp.NewContactRequest(test_recipient_account, test_sender_account.CurrentAddress.ToString());
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new Exception(result.ResultMessage);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("StandardTestStep3() exception: " + e.Message), e);
        }
    }

    public async Task StandardTestStep4()
    {
//Contact new_contact = null;
        try
        {
//await Task.Delay(500);
            var contacts = await _sp.LookForNewContactRequests(test_sender_account);

            if (contacts.Count == 0)
                throw new Exception("contacts.Count == 0");
            test_sender_account.CurrentContactId = contacts[contacts.Count - 1].Id;
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("StandardTestStep4() exception: " + e.Message), e);
        }
    }

    public async Task StandardTestStep5()
    {
//ChatOut new_chat_out = null;
        try
        {
            var result = await _sp.SendChatOut(test_sender_account.CurrentContact, TEST_MSG_1_OUT, MessageTypes.TEXT);
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new Exception(result.ResultMessage);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("StandardTestStep5() exception: " + e.Message), e);
        }
    }

    public async Task StandardTestStep6()
    {
        try
        {
            var result = await _sp.SendMessage(test_sender_account.CurrentContact.CurrentChatOut, TEST_MSG_2_OUT,
                MessageTypes.TEXT);
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new Exception(result.ResultMessage);

        }
        catch (Exception e)
        {
            throw new Exception(string.Format("StandardTestStep6() exception: " + e.Message), e);
        }
    }

    public async Task StandardTestStep7()
    {
        try
        {
            var new_chat_in = await _up.LookForNewChatRequests(test_recipient_account.CurrentContact);
            if (!new_chat_in)
                throw new Exception("false result");
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("MiniTestStep7() exception: " + e.Message), e);
        }
    }

    public async Task StandardTestStep8()
    {
        try
        {
            var new_message = await _up.LookForIncomingMessages(test_recipient_account.CurrentContact.CurrentChatIn);
            if (!new_message)
                throw new Exception("false result");
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("StandardTestStep8() exception: " + e.Message), e);
        }
    }



    public async Task StandardTestStep9()
    {

        try
        {
            var result = await _sp.SendChatOut(test_recipient_account.CurrentContact, TEST_MSG_REPLY_TEXT,
                MessageTypes.TEXT);
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new Exception(result.ResultMessage);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("StandardTestStep9() exception: " + e.Message), e);
        }
    }

    public async Task StandardTestStep10()
    {

        try
        {
            var result = await _sp.SendMessage(test_recipient_account.CurrentContact.CurrentChatOut,
                TEST_MSG_REPLY_SECOND_TEXT, MessageTypes.TEXT);
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new Exception(result.ResultMessage);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("StandardTestStep10() exception: " + e.Message), e);
        }
    }

    public async Task StandardTestStep11()
    {
//Message new_message = null;
        try
        {
//new_message = await _sp.SendMessage(test_sender_account.CurrentContact.CurrentChatOut, TEST_MSG_THIRD_TEXT, MessageTypes.TEXT);
            var result = await _sp.SendMessage(test_sender_account.CurrentContact.CurrentChatOut, TEST_MSG_THIRD_TEXT,
                MessageTypes.TEXT);
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new Exception(result.ResultMessage);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("StandardTestStep11() exception: " + e.Message), e);
        }
    }

    public async Task StandardTestStep12()
    {
        try
        {

            var new_message = await _up.LookForIncomingMessages(test_recipient_account.CurrentContact.CurrentChatIn);
            if (!new_message)
                throw new Exception("false result");
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("StandardTestStep12() exception: " + e.Message), e);
        }
    }

// just send another message
    public async Task StandardTestStep13()
    {
//Message new_message = null;
        try
        {
            var result = await _sp.SendMessage(test_recipient_account.CurrentContact.CurrentChatOut,
                TEST_MSG_REPLY_THIRD_TEXT, MessageTypes.TEXT);
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new Exception(result.ResultMessage);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("StandardTestStep13() exception: " + e.Message), e);
        }
    }

// send a large message without license to fail
    public async Task StandardTestStep14()
    {
        try
        {
            var message_text = string.Empty;
            for (int i = 0; i <= 100; i++)
                message_text += TEST_MSG_REPLY_THIRD_TEXT;
            var result = await _sp.SendMessage(test_sender_account.CurrentContact.CurrentChatOut, message_text,
                MessageTypes.TEXT);
            if (result.ResultCode != ResultStatusCodes.LICENSE_VIOLATION)
                throw new Exception("License validator did not recognize the message size");
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("MiniTestStep14() exception: " + e.Message), e);
        }
    }

// share secret to teh recipient
    public async Task StandardTestStep15()
    {
//Message new_message = null;
        try
        {
            var result = await _sp.SendMessage(test_sender_account.CurrentContact.CurrentChatOut,
                test_sender_account.Secrets[0].Data.ToString(), MessageTypes.SECRET);
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new Exception(result.ResultMessage);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("StandardTestStep15() exception: " + e.Message), e);
        }
    }



// create secret by the recipient
    public async Task<Secret> StandardTestStep16()
    {
        try
        {
            var secret_data = SecretBlockData.New();
            secret_data.SecretType = SecretTypes.CryptoWallet;
            secret_data.Title = "MystSafe";
            secret_data.Address =
                "WtZQKxH9boX23b4QvgecESTFigPVWfJdZZbmAKZbBVoxejL7WKEKDxB6j2GoJQVPWy6tXsxkRSzrdtuYJgNbFs8ZX22m";
            secret_data.Mnemonic = "absent where worry amateur liquid gown pen curve chef record uphold clap";
            secret_data.URL = "https://mystsafe.com";

            var secret_result = await _sp.AddNewSecret(test_recipient_account, secret_data);
            if (secret_result.ResultCode != 0 || secret_result.NewSecret == null)
            {
                throw new Exception(secret_result.ResultMessage);
            }

            return secret_result.NewSecret;
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("StandardTestStep16() exception: " + e.Message), e);
        }
    }

// share secret to teh recipient
    public async Task StandardTestStep17()
    {
//Message new_message = null;
        try
        {
            var result = await _sp.SendMessage(test_recipient_account.CurrentContact.CurrentChatOut,
                test_recipient_account.Secrets[0].Data.ToString(), MessageTypes.SECRET);
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new Exception(result.ResultMessage);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("StandardTestStep16() exception: " + e.Message), e);
        }
    }

// create long secret that should fail witout license
    public async Task StandardTestStep18()
    {
        try
        {
            var secret_data = SecretBlockData.New();
            secret_data.SecretType = SecretTypes.CryptoWallet;
            secret_data.Title = "Big Secret to fail";
            secret_data.Address =
                "WtZQKxH9boX23b4QvgecESTFigPVWfJdZZbmAKZbBVoxejL7WKEKDxB6j2GoJQVPWy6tXsxkRSzrdtuYJgNbFs8ZX22m";
            secret_data.Mnemonic = "absent where worry amateur liquid gown pen curve chef record uphold clap";
            secret_data.URL = "https://mystsafe.com";

            for (int i = 0; i < 100; i++)
            {
                secret_data.Notes +=
                    "Notes Notes Notes Notes Notes Notes Notes Notes Notes Notes Notes Notes Notes Notes Notes ";
            }

            var secret_result = await _sp.AddNewSecret(test_sender_account, secret_data);
            if (secret_result.ResultCode != ResultStatusCodes.LICENSE_VIOLATION)
            {
                throw new Exception(secret_result.ResultMessage);
            }
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("StandardTestStep18() exception: " + e.Message), e);
        }
    }

// create another secret just to make the sender account the last one updated to support retrieve test
    public async Task StandardTestStep19()
    {
        try
        {
            var secret_data = SecretBlockData.New();
            secret_data.SecretType = SecretTypes.PaymentCard;
            secret_data.Title = "My Payment Card";
            secret_data.PAN = "1234567890123456";
            secret_data.ExpDate = "12/24";
            secret_data.CVV = "123";
            secret_data.CardholderName = "John Smith";

            var secret_result = await _sp.AddNewSecret(test_sender_account, secret_data);
            if (secret_result.ResultCode != ResultStatusCodes.SUCCESS)
            {
                throw new Exception(secret_result.ResultMessage);
            }
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("StandardTestStep19() exception: " + e.Message), e);
        }
    }

// Update contact name
    public async Task StandardTestStep20()
    {
//Contact new_contact = null;
        SendContactRequestResult result;
        try
        {
            var contact = test_sender_account.Contacts[test_sender_account.Contacts.Count - 1];
            result = await _sp.UpdateContactName(contact, "Updated Name");
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new Exception(result.ResultMessage);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("Step 20 (updating contact name) exception: " + e.Message), e);
        }
    }

// delete second message (first message block)
    public async Task StandardTestStep21()
    {
        try
        {
            var messages = test_sender_account.CurrentContact.GetAllMessages();
            var message = messages[1];
            var result = await _sp.DeleteMessage(message);
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new Exception(result.ResultMessage);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("Step 21 (delete second message) exception: " + e.Message), e);
        }
    }

// delete first message (first chatout block)
    public async Task StandardTestStep22()
    {
        try
        {
            var messages = test_sender_account.CurrentContact.GetAllMessages();
            var message = messages[0];
            var result = await _sp.DeleteMessage(message);
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new Exception(result.ResultMessage);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("Step 22 (delete first message/chatout) exception: " + e.Message), e);
        }
    }


// create instant share link
    public async Task StandardTestStep23()
    {
        try
        {
            var secret_data = test_sender_account.Secrets[0].Data.Clone();
            var secret_result = await _sp.AddInstantShareSecret(test_sender_account, secret_data, 300);

            if (secret_result.ResultCode != ResultStatusCodes.SUCCESS)
            {
                throw new Exception(secret_result.ResultMessage);
            }

            instant_share_link = secret_result.NewSecret.GetInstantShareLink(_sp.BackEndURL);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("StandardTestStep23() exception: " + e.Message), e);
        }
    }

// "view" (retrieve) instant shared secret
    public async Task StandardTestStep24()
    {
        try
        {

            var link_info = new InstantShareLinkInfo(instant_share_link);
            var secret = await _sp.GetInstantShareSecret(link_info.BlockHash, link_info.EncryptionKeyBytes);

            if (secret == null)
            {
                throw new Exception("Shared secret not found");

            }
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("Step 24 (retrieve instant shared secret) exception: " + e.Message), e);
        }
    }



    public async Task<Account> InitializeLicenseTest(Account current_account)
    {
        test_sender_account = await InitializeQuickTest(current_account);
        string nickname = "TEST LIC. RCP. " + DateTime.Now.ToShortDateString();
        if (nickname.Length > 25)
        {
            nickname = nickname.Substring(0, 25);
        }

        account_5 = await _sp.CreateNewAccount(nickname, Networks.devnet);
        test_account_id_5 = account_5.Id;

        _up = new UpdateProcessor(
//_sp._contactBlockNodeService,
//_sp._initBlockNodeService,
//_sp._msgBlockService,
//_sp._secretBlockNodeService,
//_sp._licenseBlockNodeService,
            false,
            _sp._Account_Db,
            _sp._Contact_Db,
            _sp._ChatIn_Db,
            _sp._ChatOut_Db,
            _sp._Message_Db,
            _sp._Secret_Db,
            _sp.Logger,
            _sp._apiClientService);

        return test_sender_account;
    }


/*
// try to generate a new master license key;
// it can be generated only if there are no master keys in the database (when database is new)
// therefore, this test can only be performed in devnet or testnet after deleting master keys.
    public async Task LicenseTestStep0()
    {
        Message new_message = null;
        try
        {
            var new_master_license_key_pair = BlockKeyPairEx.GenerateRandom();
            var new_key_params = new NewMasterLicenseKeyParams();
            new_key_params.NewMasterLicensePubKey = new_master_license_key_pair.PubKeyBase58;
            new_key_params.TimeStamp = UnixDateTime.Now;
            if (!string.IsNullOrEmpty(test_sender_account.MasterLicensePrivateKey))
            {
                new_key_params.Sign(test_sender_account.MasterLicensePrivateKey);
            }
            else
            {
                new_key_params.Signature = string.Empty;
            }

            var result = await _sp._apiClientService.NewMasterLicenseKey(new_key_params);
            if (result.ResultCode == ResultStatusCodes.SUCCESS)
            {
                _up.UpdateMasterLicensePrivateKey(new_master_license_key_pair.KeyBase64,
                    new_master_license_key_pair.PubKeyBase58, test_sender_account);
            }
            else
                throw new Exception(result.ResultMessage);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("LicenseTestStep0() exception: " + e.Message), e);
        }
    }
    */

/*
// issue premium license to test_sender_account (itself)
    public async Task LicenseTestStep1()
    {
        Message new_message = null;
        try
        {
            var master_license_key_pair =
                BlockKeyPairEx.GenerateFromPrivateKey(test_sender_account.MasterLicensePrivateKey);
            var network = test_sender_account.Network;

            var encoder = new LicenseBlockEncoder(
                UnixDateTime.Now,
                master_license_key_pair,
                Constants.PREMIUM_LICENSE,
                test_sender_account.CurrentAddress.ToString(),
                network,
                12,
                49.99m,
                12 * 49.99m
            );

            var block = await encoder.Encode();

            var license_params = new GenerateLicenseParams();
            license_params.AccountAddress = test_sender_account.CurrentAddress.ToString();
            license_params.OrderNumber = "1001";
            license_params.OrderAmount = 49.99m;
            license_params.Active = true;
            license_params.Expiration = block.ExpirationDate;
            license_params.DurationMonths = 12;
            license_params.OrderTimeStamp = UnixDateTime.Now;
            license_params.Block = block;
            license_params.TimeStamp = UnixDateTime.Now;
            license_params.Notes = "Some notes";
            license_params.Reserved = string.Empty;
            license_params.LicenseType = block.LicenseType;
            license_params.Sign(test_sender_account.MasterLicensePrivateKey);

            var broadcast_result = await _sp._apiClientService.GenerateLicense(license_params);

            if (broadcast_result == null)
                throw new Exception("Null broadcast_result");
            if (broadcast_result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new Exception(broadcast_result.ResultMessage);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("LicenseTestStep1() exception: " + e.Message), e);
        }
    }
    */

// assign the license to the account
    /*public async Task LicenseTestStep2()
    {
        try
        {

            var license_text = await _sp.LookForLicense(test_sender_account);

            if (string.IsNullOrWhiteSpace(license_text))
            {
                throw new Exception("Could not find license");
            }

        }
        catch (Exception e)
        {
            throw new Exception(string.Format("LicenseTestStep2() exception: " + e.Message), e);
        }
    }*/

//// generate the license proof
//public async Task LicenseTestStep3(Secret secret)
//{
//    try
//    {
//        var ring_signature = await _sp.GenerateLicenseProof(
//            test_sender_account,
//            secret.BlockHash,
//            UnixDateTime.FromDateTime(secret.TimeStamp),
//            secret.PubKey);

//        if (string.IsNullOrWhiteSpace(ring_signature))
//        {
//            throw new Exception("Could not generate ring signature");
//        }

//    }
//    catch (Exception e)
//    {
//        throw new Exception(string.Format("LicenseTestStep3() exception: " + e.Message), e);
//    }
//}

// create secret with pro license and big size
    public async Task<Secret> LicenseTestStep3()
    {
        try
        {
            var secret_data = SecretBlockData.New();
            secret_data.SecretType = SecretTypes.CryptoWallet;
            secret_data.Title = "Big Secret";
            secret_data.Address =
                "WtZQKxH9boX23b4QvgecESTFigPVWfJdZZbmAKZbBVoxejL7WKEKDxB6j2GoJQVPWy6tXsxkRSzrdtuYJgNbFs8ZX22m";
            secret_data.Mnemonic = "absent where worry amateur liquid gown pen curve chef record uphold clap";
            secret_data.URL = "https://mystsafe.com";

            for (int i = 0; i < 100; i++)
            {
                secret_data.Notes +=
                    "Notes Notes Notes Notes Notes Notes Notes Notes Notes Notes Notes Notes Notes Notes Notes ";
            }

            var secret_result = await _sp.AddNewSecret(test_sender_account, secret_data);
            if (secret_result.ResultCode != 0 || secret_result.NewSecret == null)
            {
                throw new Exception(secret_result.ResultMessage);
            }

            return secret_result.NewSecret;
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("LicenseTestStep3() exception: " + e.Message), e);
        }
    }

    public async Task LicenseTestStep4()
    {
//Contact new_contact = null;
        SendContactRequestResult result;
        try
        {
            result = await _sp.NewContactRequest(test_sender_account, account_5.CurrentAddress.ToString());
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new Exception(result.ResultMessage);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("LicenseTestStep4() exception: " + e.Message), e);
        }
    }

    public async Task LicenseTestStep5()
    {
        try
        {
            var contacts = await _sp.LookForNewContactRequests(account_5);
            if (contacts.Count == 0)
                throw new Exception("contacts.Count == 0");
            account_5.CurrentContactId = contacts[0].Id;
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("LicenseTestStep5() exception: " + e.Message), e);
        }
    }

    public async Task LicenseTestStep6()
    {
        try
        {
            var result = await _sp.NewContactRequest(account_5, test_sender_account.CurrentAddress.ToString());
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new Exception(result.ResultMessage);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("LicenseTestStep6() exception: " + e.Message), e);
        }
    }

    public async Task LicenseTestStep7()
    {
        try
        {
            var contacts = await _sp.LookForNewContactRequests(test_sender_account);

            if (contacts.Count == 0)
                throw new Exception("contacts.Count == 0");
            test_sender_account.CurrentContactId = contacts[contacts.Count - 1].Id;
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("LicenseTestStep7() exception: " + e.Message), e);
        }
    }

    public async Task LicenseTestStep8()
    {
        try
        {
            var message_text = "";
            for (int i = 0; i <= 1000; i++)
                message_text += "BIG CHAT INIT ";
            message_text += "THIS MESSAGE LENGTH IS " + message_text.Length + " BYTES! ";
            var result = await _sp.SendChatOut(test_sender_account.CurrentContact, message_text, MessageTypes.TEXT);
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new Exception(result.ResultMessage);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("LicenseTestStep8() exception: " + e.Message), e);
        }
    }

// send a large message with license
    public async Task LicenseTestStep9()
    {
        try
        {
            var message_text = "";
            for (int i = 0; i <= 1000; i++)
                message_text += "BIG MESSAGE ";
            message_text += "THIS MESSAGE LENGTH IS " + message_text.Length + " BYTES! ";

            var result = await _sp.SendMessage(test_sender_account.CurrentContact.CurrentChatOut, message_text,
                MessageTypes.TEXT);
            if (result.ResultCode != ResultStatusCodes.SUCCESS)
                throw new Exception(result.ResultMessage);
        }
        catch (Exception e)
        {
            throw new Exception(string.Format("LicenseTestStep9() exception: " + e.Message), e);
        }
    }


    public async Task TestDiffieHelman()
    {

        try
        {

            await Task.Run(() =>
            {

                var msg_data = new MsgBlockData();
                msg_data.MsgText = TEST_MSG_1_OUT;
//msg_data.AddParam(MsgData.MSG_PUB_KEY, test_sender_account.CurrentAddress.ReadPubKeyStr);
                msg_data.AddParam(MsgBlockData.SENDER_ADDRESS, test_sender_account.CurrentAddress.ToString());
                string msg_data_str = msg_data.ToString();

                var MessageData =
                    DiffieHellman.Encrypt(
                        test_sender_account.CurrentAddress.ReadKey,
                        test_recipient_account.CurrentAddress.ReadPubKey.ToString(),
                        "just a random string for test here",
                        msg_data_str);
                var msg_data_decrypted =
                    DiffieHellman.Decrypt(
                        test_recipient_account.CurrentAddress.ReadKey,
                        test_sender_account.CurrentAddress.ReadPubKey.ToString(),
                        "just a random string for test here",
                        MessageData);

                if (TEST_MSG_1_OUT != new MsgBlockData(msg_data_decrypted).MsgText)
                    throw new Exception("\nSelf-test for Diffie-Hellman failed.");

                Console.WriteLine("\nSelf-test for Diffie-Hellman passed.");
            });

        }
        catch (Exception e)
        {
            throw new Exception(string.Format("exception in diffie-hellman self test: " + e.Message), e);

        }

    }

}

