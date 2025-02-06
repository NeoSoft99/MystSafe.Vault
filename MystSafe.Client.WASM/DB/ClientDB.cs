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

using IndexedDB.Blazor;
using Microsoft.JSInterop;
using MystSafe.Client.CryptoLicense;

namespace MystSafe.Client.App;

public class ClientDb : IndexedDb
{
    public ClientDb(IJSRuntime jSRuntime, string name, int version) : base(jSRuntime, name, version)
    {
    }

    public IndexedSet<AccountRecord> Accounts { get; set; }
    public IndexedSet<ContactRecord> Contacts { get; set; }
    public IndexedSet<ChatInRecord> ChatsIn { get; set; }
    public IndexedSet<ChatOutRecord> ChatsOut { get; set; }
    public IndexedSet<MessageRecord> Messages { get; set; }
    public IndexedSet<SecretRecord> Secrets { get; set; }
    
    public IndexedSet<TxLocalRecord> Transactions { get; set; }
    public IndexedSet<OutputLocalRecord> Outputs { get; set; }
    
    public IndexedSet<WalletLocalRecord> Wallets { get; set; }
}