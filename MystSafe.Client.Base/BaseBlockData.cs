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

namespace MystSafe.Client.Base;

public abstract class BaseBlockData
{
    public Dictionary<string, string> Params { get; set; }

    // use this one to create a new msg data to send
    public BaseBlockData()
    {
        Params = new Dictionary<string, string>();
    }

    // use this one to restore the received msg data
    public BaseBlockData(string BlockData)
    {
        if (string.IsNullOrWhiteSpace(BlockData))
            throw new Exception("Bad MessageData");
    }

    public void AddParam(string param_name, string param_value)
    {
        bool added = Params.TryAdd(param_name, param_value);

        if (!added)
        {
            Params[param_name] = param_value;
        }
    }

    public void AddIntParam(string param_name, int param_value)
    {
        AddParam(param_name, param_value.ToString());
    }

    public string GetParam(string param_name)
    {
        if (Params.TryGetValue(param_name, out string? value))
        {
            return value;
        }
        else
        {
            return string.Empty;
        }
    }

    public int GetIntParam(string param_name)
    {
        return int.TryParse(GetParam(param_name), out int result) ? result : 0;
    }

    public override string ToString()
    {
        // need to serialize the content to a string here
        //return JsonSerializer.Serialize(this);
        throw new NotImplementedException("Need to override ToString()");
    }

}

