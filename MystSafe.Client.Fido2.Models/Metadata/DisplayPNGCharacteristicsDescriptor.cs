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

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fido2NetLib;

/// <summary>
/// The DisplayPNGCharacteristicsDescriptor describes a PNG image characteristics as defined in the PNG [PNG] spec for IHDR (image header) and PLTE (palette table)
/// </summary>
/// <remarks>
/// <see href="https://fidoalliance.org/specs/fido-v2.0-rd-20180702/fido-metadata-statement-v2.0-rd-20180702.html#displaypngcharacteristicsdescriptor-dictionary"/>
/// </remarks>
public sealed class DisplayPNGCharacteristicsDescriptor
{
    /// <summary>
    /// Gets or sets the image width.
    /// </summary>
    [JsonPropertyName("width"), Required]
    public ulong Width { get; set; }

    /// <summary>
    /// Gets or sets the image height.
    /// </summary>
    [JsonPropertyName("height"), Required]
    public ulong Height { get; set; }

    /// <summary>
    /// Gets or sets the bit depth - bits per sample or per palette index.
    /// </summary>
    [JsonPropertyName("bitDepth"), Required]
    public byte BitDepth { get; set; }

    /// <summary>
    /// Gets or sets the color type defines the PNG image type.
    /// </summary>
    [JsonPropertyName("colorType"), Required]
    public byte ColorType { get; set; }

    /// <summary>
    /// Gets or sets the compression method used to compress the image data.
    /// </summary>
    [JsonPropertyName("compression"), Required]
    public byte Compression { get; set; }

    /// <summary>
    /// Gets or sets the filter method is the preprocessing method applied to the image data before compression.
    /// </summary>
    [JsonPropertyName("filter"), Required]
    public byte Filter { get; set; }

    /// <summary>
    /// Gets or sets the interlace method is the transmission order of the image data.
    /// </summary>
    [JsonPropertyName("interlace"), Required]
    public byte Interlace { get; set; }

    /// <summary>
    /// Gets or sets the palette (1 to 256 palette entries).
    /// </summary>
    [JsonPropertyName("plte")]
    public RgbPaletteEntry[] Plte { get; set; }
}
