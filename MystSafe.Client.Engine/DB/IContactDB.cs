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

namespace MystSafe.Client.Engine;

public interface IContactDB
{
    /// <summary>
    /// Delete Account 
    /// </summary>
    Task Delete(string contact_id, string account_id);

    /// <summary>
    /// Delete all chats that belong to Account 
    /// </summary>
    Task DeleteAll(string AccountId);

    /// <summary>
    /// Delete using pub key 
    /// </summary>
    Task DeleteByBlockPubKey(string block_pub_key, string account_id);

    /// <summary>
    /// Store a new chat
    /// </summary>
    Task Add(Contact contact);

    /// <summary>
    /// Retrive all the contacts that belong to Account 
    /// </summary>
    Task<List<Contact>> GetAll(string AccountId);

    /// <summary>
    /// update an Account 
    /// </summary>
    Task Update(Contact contact);

    /// <summary>
    /// Get a single secret by block pub key which is unique secret identifier 
    /// </summary>
    Task<Contact> GetByBlockPubKey(string block_pub_key, string account_id);
}


