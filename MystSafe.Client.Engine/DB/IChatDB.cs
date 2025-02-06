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

public interface IChatInDB
{
    /// <summary>
    /// Store a new chat
    /// </summary>
    Task Add(ChatIn chat);

    /// <summary>
    /// Retrive all the chats that belong to Contact 
    /// </summary>
    Task<List<ChatIn>> GetAll(string ContactId, string account_id);

    /// <summary>
    /// update an Account 
    /// </summary>
    Task Update(ChatIn chat);

    /// <summary>
    /// Delete Account 
    /// </summary>
    Task Delete(string chat_id, string account_id);

    /// <summary>
    /// Delete all chats that belong to Account 
    /// </summary>
    Task DeleteAll(string ContactId, string account_id);
}

public interface IChatOutDB
{
    /// <summary>
    /// Store a new chat
    /// </summary>
    Task Add(ChatOut chat);

    /// <summary>
    /// Retrive all the chats that belong to Account 
    /// </summary>
    Task<List<ChatOut>> GetAll(string ContactId, string account_id);

    /// <summary>
    /// update an Account 
    /// </summary>
    Task Update(ChatOut chat);

    /// <summary>
    /// Delete a single chat 
    /// </summary>
    Task Delete(string chat_id, string account_id);

    /// <summary>
    /// Delete all chats that belong to contact 
    /// </summary>
    Task DeleteAll(string ContactId, string account_id);
}


