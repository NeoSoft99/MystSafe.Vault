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

namespace MystSafe.Client.Engine;

public interface IAccountDB
{
    /// <summary>
    /// Store a new Account
    /// </summary>
    Task Add(Account account);

    /// <summary>
    /// Get the authn data for the most recent account
    /// so we could decrypt and retrieve its data from the local database
    /// </summary>
    Task<AccountAuthnResult> GetMostRecentlyUsedAccountAuthnData();

    /// <summary>
    /// load the account data from the local database into the memory
    /// </summary>
    Task<Account> RetrieveAccount(string account_id);

    /// <summary>
    /// update an Account 
    /// </summary>
    Task Update(Account account);


    /// <summary>
    /// Delete Account 
    /// </summary>
    Task Delete(string Id);
}



