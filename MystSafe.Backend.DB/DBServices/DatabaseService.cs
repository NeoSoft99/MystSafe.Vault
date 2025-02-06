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

using MongoDB.Driver;
using MystSafe.Shared.Crypto;

namespace MystSafe.Backend.DB;

public class DatabaseService
{
    public const string NODE_NETWORK_CONFIG_NAME = "NODE_NETWORK";
    public const string MONGODB_CONNECTION_STRING_CONFIG_NAME = "MONGODB_CONNECTION_STRING";
    public const string MONGODB_DATABASE_NAME_CONFIG_NAME = "MONGODB_DATABASE_NAME";


    public readonly IMongoDatabase MystSafeDB;
    public readonly int Network;

    public DatabaseService(string network, string connection_string, string database_name)
    {
        try
        {
            Network = Networks.FromString(network);
            var mongoClient = new MongoClient(connection_string);
            MystSafeDB = mongoClient.GetDatabase(database_name);
        }
        catch (Exception ex)
        {
            Console.WriteLine("DatabaseService init exception: " + ex);
            throw new ApplicationException("Failed to open database: " + ex);
        }
    }
}