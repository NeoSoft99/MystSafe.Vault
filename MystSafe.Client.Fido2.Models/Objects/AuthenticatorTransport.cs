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

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Fido2NetLib.Objects;

/// <summary>
/// Authenticators may implement various transports for communicating with clients.
/// This enumeration defines hints as to how clients might communicate with a particular
/// authenticator in order to obtain an assertion for a specific credential.
/// Note that these hints represent the WebAuthn Relying Party's best belief as to how an authenticator may be reached. 
/// A Relying Party will typically learn of the supported transports for a public key credential via getTransports().
/// https://www.w3.org/TR/webauthn-2/#enum-transport
/// </summary>
[JsonConverter(typeof(FidoEnumConverter<AuthenticatorTransport>))]
public enum AuthenticatorTransport
{
    /// <summary>
    /// Indicates the respective authenticator can be contacted over removable USB.
    /// </summary>
    [EnumMember(Value = "usb")]
    Usb,

    /// <summary>
    /// Indicates the respective authenticator can be contacted over Near Field Communication (NFC).
    /// </summary>
    [EnumMember(Value = "nfc")]
    Nfc,

    /// <summary>
    /// Indicates the respective authenticator can be contacted over Bluetooth Smart (Bluetooth Low Energy / BLE).
    /// </summary>
    [EnumMember(Value = "ble")]
    Ble,

    /// <summary>
    /// Indicates the respective authenticator can be contacted over over ISO/IEC 7816 smart card with contacts.
    /// </summary>
    [EnumMember(Value = "smart-card")]
    SmartCard,

    /// <summary>
    /// Indicates the respective authenticator can be contacted using a combination of (often separate) data-transport
    /// and proximity mechanisms. This supports, for example, authentication on a desktop computer using a smartphone.
    /// </summary>
    [EnumMember(Value = "hybrid")]
    Hybrid,

    /// <summary>
    /// Indicates the respective authenticator is contacted using a client device-specific transport, i.e., it is a platform authenticator. 
    /// These authenticators are not removable from the client device.
    /// </summary>
    [EnumMember(Value = "internal")]
    Internal,
}
