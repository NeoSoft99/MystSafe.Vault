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

using BlazorDownloadFile;
using MystSafe.Shared.Crypto;
using MystSafe.Client.Engine;
using Microsoft.AspNetCore.Components.Forms;

namespace MystSafe.Client.App;

public class SecretExportService
{
    private readonly IBlazorDownloadFileService _blazorDownloadFileService;

    public SecretExportService(IBlazorDownloadFileService blazorDownloadFileService)
    {
        _blazorDownloadFileService = blazorDownloadFileService;
    }

    public async Task<string> ExportSecretsToFileAsync(List<Secret> secrets, string baseFilename, string mnemonicString)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string filename = $"{baseFilename}_{timestamp}.dat";

        string serializedData = string.Empty;
        foreach (var secret in secrets)
            serializedData += secret.Export();

        //byte[] dataBytes = Encoding.UTF8.GetBytes(serializedData);

        //var iv = Hashing.SHA256Bytes(filename);
        var key = Hashing.KeccakBase58(mnemonicString);

        var encrypted_Bytes = AES.Encrypt(key, filename, serializedData);

        //await _blazorDownloadFileService.DownloadFile(filename, encrypted_Bytes, "text/plain");
        await _blazorDownloadFileService.DownloadFile(filename, encrypted_Bytes, "application/octet-stream");
        return filename;
    }

    public async Task<string> DecryptExportFile(IBrowserFile file, string mnemonicString)
    {
        var encrypted_data = await ReadFileContent(file);
        var key = Hashing.KeccakBase58(mnemonicString);
        var decrypted_bytes = AES.Decrypt(encrypted_data, key, file.Name);
        //var decrypted_file_content = Encoding.UTF8.GetString(decrypted_bytes);
        var newFileName = file.Name.Substring(0, file.Name.Length - 4) + ".txt";
        var download_result = await _blazorDownloadFileService.DownloadFile(newFileName, decrypted_bytes, "text/plain");
        return newFileName;
    }

    private async Task<byte[]> ReadFileContent(IBrowserFile file)
    {
        // Define the maximum file size you want to support
        long maxFileSize = 1024 * 1024 * 20; // 20 MB 

        if (file.Size > maxFileSize)
        {
            throw new InvalidOperationException("File is too large.");
        }

        using var stream = file.OpenReadStream(maxFileSize);
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }
}

