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

public interface ISecretDB
{
    /// <summary>
    /// Store a new secret
    /// </summary>
    Task Add(Secret secret);

    /// <summary>
    /// Retrive all the secrets that belong to Account 
    /// </summary>
    Task<List<Secret>> GetAll(string account_id);

    /// <summary>
    /// update an secret 
    /// </summary>
    Task Update(Secret secret);

    ///// <summary>using secret id 
    ///// </summary>
    //Task DeleteById(string id);

    ///// <summary>
    ///// Delete using hash 
    ///// </summary>
    //Task DeleteByBlockHash(string block_hash);

    /// <summary>
    /// Delete using pub key 
    /// </summary>
    Task DeleteByBlockPubKey(string block_pub_key, string account_id);

    /// <summary>
    /// Delete all secrets that belong to Account 
    /// </summary>
    Task DeleteAll(string account_id);

    ///// <summary>
    ///// Get a single secret by ID 
    ///// </summary>
    //Task<Secret> Get(string id, string account_id);

    /// <summary>
    /// Get a single secret by block pub key which is unique secret identifier 
    /// </summary>
    Task<Secret> GetByBlockPubKey(string block_pub_key, string account_id);
}


