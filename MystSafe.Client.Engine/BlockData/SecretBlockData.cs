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
using System.Text;
using Newtonsoft.Json;

namespace MystSafe.Client.Engine;

public class SecretTypes
{
    public const int Folder = -1;
    public const int Other = 0;
    public const int Login = 1;
    public const int PaymentCard = 2;
    public const int CryptoWallet = 3;
    public const int DatabaseConnectionString = 4;
    public const int SSLCertificateKey = 5;
    public const int SSHKey = 6;
    public const int APIKey = 7;
    public const int Computer = 8;
    public const int BankAccount = 9;
    public const int Application = 10;
}

public class SecretTypeLabels
{
    public const string SECRET_OTHER = "Other";
    public const string SECRET_LOGIN = "Login";
    public const string SECRET_PAYMENT_CARD = "Payment Card";
    public const string SECRET_WALLET = "Crypto wallet";
    public const string SECRET_COMPUTER = "Computer";
    public const string SECRET_SSL = "SSL certificate";
    public const string SECRET_SSH = "SSH key";
    public const string SECRET_API = "API Key";
    public const string SECRET_DATABASE = "Database";
    public const string SECRET_BANK = "Bank Account";
    public const string SECRET_APPLICATION = "Application";

    public static int GetType(string secret_type_label)
    {
        switch (secret_type_label)
        {
            case SECRET_LOGIN: return SecretTypes.Login;
            case SECRET_PAYMENT_CARD: return SecretTypes.PaymentCard;
            case SECRET_WALLET: return SecretTypes.CryptoWallet;
            case SECRET_COMPUTER: return SecretTypes.Computer;
            case SECRET_SSL: return SecretTypes.SSLCertificateKey;
            case SECRET_SSH: return SecretTypes.SSHKey;
            case SECRET_API: return SecretTypes.APIKey;
            case SECRET_DATABASE: return SecretTypes.DatabaseConnectionString;
            case SECRET_BANK: return SecretTypes.BankAccount;
            case SECRET_APPLICATION: return SecretTypes.Application;
            default: return SecretTypes.Other;
        }
    }

    public static string GetLabel(int secret_type)
    {
        switch (secret_type)
        {
            case SecretTypes.Login: return SECRET_LOGIN;
            case SecretTypes.PaymentCard: return SECRET_PAYMENT_CARD;
            case SecretTypes.CryptoWallet: return SECRET_WALLET;
            case SecretTypes.Computer: return SECRET_COMPUTER;
            case SecretTypes.SSLCertificateKey: return SECRET_SSL;
            case SecretTypes.SSHKey: return SECRET_SSH;
            case SecretTypes.APIKey: return SECRET_API;
            case SecretTypes.DatabaseConnectionString: return SECRET_DATABASE;
            case SecretTypes.BankAccount: return SECRET_BANK;
            case SecretTypes.Application: return SECRET_APPLICATION;
            default: return SECRET_OTHER;
        }
    }
}

public class RuntimeVariables
{
    public const string CLOUD_ACCOUNT = "CLOUD_ACCOUNT";
    public const string CLOUD_SECRET_ACCESS_KEY = "CLOUD_SECRET_ACCESS_KEY";
    public const string CLOUD_ACCESS_KEY = "CLOUD_ACCESS_KEY";
    public const string CLIENT_KEY = "CLIENT_KEY";
    public const string HOST_NAME = "HOST_NAME";
    public const string USER_NAME = "USER_NAME";
    public const string MOTHERBOARD = "MOTHERBOARD";
    public const string MAC_ADDRESS = "MAC_ADDRESS";
    public const string LOCAL_IP_ADDRESS = "LOCAL_IP_ADDRESS";
    public const string SERVER_KEY = "SERVER_KEY";
    public const string CLOUD_ASSUMED_ROLE = "CLOUD_ASSUMED_ROLE";

}

public class SecretBlockData: BaseBlockData
{
    #region these are the standard param names

    private const string _NOTE = "N";
    private const string _FOLDER_ID = "F";
    private const string _LOGIN = "L";
    private const string _PASSWORD = "P";
    private const string _MNEMONIC = "M";
    private const string _PRIVATE_KEY = "PR";
    private const string _PUBLIC_KEY = "PU";
    private const string _URL = "UR";
    private const string _ADDRESS = "A";
    private const string _PAN = "PA";
    private const string _EXP_DATE = "EX";
    private const string _CARDHOLDER_NAME = "CH";
    private const string _COMPUTER_NAME = "CN";
    private const string _DOMAIN_NAME = "DN";
    private const string _CVV = "CV";
    private const string _DB_CONNECTION_STRING = "DB";
    private const string _ROUTING_NUMBER = "RN";
    private const string _GROUP_PRIVATE_KEY = "GP";
    private const string _RUNTIME_TYPE = "RT";
    //private const string _INSTANT_SHARE_LINK = "IL";
    private const string _INSTANT_KEY = "IK";

    #endregion

    //// this is the permanent id of the secret, it will secretly link between different versions of secret blocks
    //public string SecretId { get; set; }

    #region these are the fields that always should be set

    // this is the block private key
    // the block PUBLIC key is the permanent id of the secret, it will secretly link between different versions of secret blocks
    public string BlockPrivateKey { get; set; }

    // this is always title
    public string Title { get; set; }

    public int SecretType { get; set; }

    // this is the hidden id that links between different verson of secret and also between the orginal secrets and its instant share links
    public string GlobalId { get; set; }

    public Dictionary<string, string> UserParams { get; set; }

    public Dictionary<string, string> RuntimeParams { get; set; }


    #endregion


    #region these are all optional fields
    [JsonIgnore]
    public string FolderId { get { return GetParam(_FOLDER_ID); } set { AddParam(_FOLDER_ID, value); } }

    // this is always notes
    [JsonIgnore]
    public string Notes { get { return GetParam(_NOTE); } set { AddParam(_NOTE, value); } }

    // this is parameter name for application secret type
    [JsonIgnore]
    public string Login { get { return GetParam(_LOGIN); } set { AddParam(_LOGIN, value); } }

    // this is parameter value for application secret type
    [JsonIgnore]
    public string Password { get { return GetParam(_PASSWORD); } set { AddParam(_PASSWORD, value); } }
    [JsonIgnore]
    public string Mnemonic { get { return GetParam(_MNEMONIC); } set { AddParam(_MNEMONIC, value); } }
    [JsonIgnore]
    public string PrivateKey { get { return GetParam(_PRIVATE_KEY); } set { AddParam(_PRIVATE_KEY, value); } }
    [JsonIgnore]
    public string PublicKey { get { return GetParam(_PUBLIC_KEY); } set { AddParam(_PUBLIC_KEY, value); } }
    [JsonIgnore]
    public string URL { get { return GetParam(_URL); } set { AddParam(_URL, value); } }
    [JsonIgnore]
    public string Address { get { return GetParam(_ADDRESS); } set { AddParam(_ADDRESS, value); } }
    [JsonIgnore]
    public string PAN { get { return GetParam(_PAN); } set { AddParam(_PAN, value); } }
    [JsonIgnore]
    public string ExpDate { get { return GetParam(_EXP_DATE); } set { AddParam(_EXP_DATE, value); } }
    [JsonIgnore]
    public string CardholderName { get { return GetParam(_CARDHOLDER_NAME); } set { AddParam(_CARDHOLDER_NAME, value); } }
    [JsonIgnore]
    public string ComputerName { get { return GetParam(_COMPUTER_NAME); } set { AddParam(_COMPUTER_NAME, value); } }
    [JsonIgnore]
    public string DomainName { get { return GetParam(_DOMAIN_NAME); } set { AddParam(_DOMAIN_NAME, value); } }
    [JsonIgnore]
    public string CVV { get { return GetParam(_CVV); } set { AddParam(_CVV, value); } }
    [JsonIgnore]
    public string DatabaseConnectionString { get { return GetParam(_DB_CONNECTION_STRING); } set { AddParam(_DB_CONNECTION_STRING, value); } }
    [JsonIgnore]
    public string RoutingNumber { get { return GetParam(_ROUTING_NUMBER); } set { AddParam(_ROUTING_NUMBER, value); } }

    // this is the group private key generated from the environmental variables
    [JsonIgnore]
    public string GroupPrivateKey { get { return GetParam(_GROUP_PRIVATE_KEY); } set { AddParam(_GROUP_PRIVATE_KEY, value); } }
    [JsonIgnore]
    public int RuntimeType { get { return GetIntParam(_RUNTIME_TYPE); } set { AddIntParam(_RUNTIME_TYPE, value); } }
    //[JsonIgnore]
    //public string InstantShareLink { get { return GetParam(_INSTANT_SHARE_LINK); } set { AddParam(_INSTANT_SHARE_LINK, value); } }
    [JsonIgnore]
    public string InstantKey { get { return GetParam(_INSTANT_KEY); } set { AddParam(_INSTANT_KEY, value); } }

    //private readonly ILogger _logger;

    #endregion

    // this must be public to allow deserializer access
    public SecretBlockData() : base()
    {
        UserParams = new Dictionary<string, string>();
        RuntimeParams = new Dictionary<string, string>();
    }

    public static SecretBlockData New()
    {
        var data = new SecretBlockData();
        data.GlobalId = Guid.NewGuid().ToString();
        return data;
    }

    // use this one to restore the received data
    public SecretBlockData(string serialized_data): base(serialized_data)
    {
        SecretBlockData data = JsonConvert.DeserializeObject<SecretBlockData>(serialized_data);

        BlockPrivateKey = data.BlockPrivateKey;
        Title = data.Title;
        SecretType = data.SecretType;
        GlobalId = data.GlobalId;

        Params = data.Params;
        UserParams = data.UserParams;
        RuntimeParams = data.RuntimeParams;
        //_logger = logger;
    }

    public SecretBlockData Clone()
    {
        SecretBlockData data = new SecretBlockData();

        data.BlockPrivateKey = BlockPrivateKey;
        data.Params = new Dictionary<string, string>(Params);
        data.Title = Title;
        data.SecretType = SecretType;
        data.GlobalId = GlobalId;
        data.UserParams = new Dictionary<string, string>(UserParams);
        data.RuntimeParams = new Dictionary<string, string>(RuntimeParams);

        return data;
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static SecretBlockData ClientDecrypt(string encrypted_data)
    {
        //ClientSideLogger.Logger.LogInformation("encrypted_data: " + encrypted_data);
        return JsonConvert.DeserializeObject<SecretBlockData>(encrypted_data);
    }

    public string ClientEncrypt()
    {
        return ToString();
    }

    public void AddUserParam(string param_name, string param_value)
    {
        bool added = UserParams.TryAdd(param_name, param_value);

        if (!added)
        {
            UserParams[param_name] = param_value;
        }
    }

    public string GetUserParam(string param_name)
    {
        if (UserParams.TryGetValue(param_name, out string? value))
        {
            return value;
        }
        else
        {
            return string.Empty;
        }
    }

    public void AddRuntimeParam(string param_name, string param_value)
    {
        bool added = RuntimeParams.TryAdd(param_name, param_value);

        if (!added)
        {
            RuntimeParams[param_name] = param_value;
        }
    }

    public string GetRuntimeParam(string param_name)
    {
        if (RuntimeParams.TryGetValue(param_name, out string? value))
        {
            return value;
        }
        else
        {
            return string.Empty;
        }
    }

    public string Export()
    {
        var result = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(GlobalId))
            result.AppendLine("Global Id: " + GlobalId);

        if (!string.IsNullOrWhiteSpace(FolderId))
            result.AppendLine("Folder Id: " + FolderId);

        if (!string.IsNullOrWhiteSpace(Title))
            result.AppendLine("Title: " + Title);

        if (!string.IsNullOrWhiteSpace(Login))
            result.AppendLine("Login: " + Login);

        if (!string.IsNullOrWhiteSpace(Password))
            result.AppendLine("Password: " + Password);

        if (!string.IsNullOrWhiteSpace(Mnemonic))
            result.AppendLine("Mnemonic: " + Mnemonic);

        if (!string.IsNullOrWhiteSpace(PrivateKey))
            result.AppendLine("PrivateKey: " + PrivateKey);

        if (!string.IsNullOrWhiteSpace(PublicKey))
            result.AppendLine("PublicKey: " + PublicKey);

        if (!string.IsNullOrWhiteSpace(URL))
            result.AppendLine("URL: " + URL);

        if (!string.IsNullOrWhiteSpace(Address))
            result.AppendLine("Address: " + Address);

        if (!string.IsNullOrWhiteSpace(PAN))
            result.AppendLine("PAN: " + PAN);

        if (!string.IsNullOrWhiteSpace(ExpDate))
            result.AppendLine("ExpDate: " + ExpDate);

        if (!string.IsNullOrWhiteSpace(CardholderName))
            result.AppendLine("Cardholder Name: " + CardholderName);

        if (!string.IsNullOrWhiteSpace(ComputerName))
            result.AppendLine("Computer Name: " + ComputerName);

        if (!string.IsNullOrWhiteSpace(DomainName))
            result.AppendLine("Domain Name: " + DomainName);

        if (!string.IsNullOrWhiteSpace(CVV))
            result.AppendLine("CVV: " + CVV);

        if (!string.IsNullOrWhiteSpace(DatabaseConnectionString))
            result.AppendLine("Database Connection String: " + DatabaseConnectionString);

        if (!string.IsNullOrWhiteSpace(RoutingNumber))
            result.AppendLine("Routing Number: " + RoutingNumber);

        if (!string.IsNullOrWhiteSpace(Notes))
            result.AppendLine("Notes: " + Notes);

        if (RuntimeType != 0)
            result.AppendLine("RuntimeType: " + RuntimeType);

        foreach (var param in UserParams)
            result.AppendLine(param.Key + ": " + param.Value);

        foreach (var param in RuntimeParams)
            result.AppendLine(param.Key + ": " + param.Value);

        return result.ToString();
    }

}

