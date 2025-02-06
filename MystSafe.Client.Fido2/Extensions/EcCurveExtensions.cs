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

using System;
using System.Security.Cryptography;

using Fido2NetLib.Objects;

namespace Fido2NetLib;

internal static class EcCurveExtensions
{
    public static COSE.EllipticCurve ToCoseCurve(this ECCurve curve)
    {
        if (curve.Oid.FriendlyName is "secP256k1") // OID = 1.3.132.0.10
            return COSE.EllipticCurve.P256K;

        if (curve.Oid.Value!.Equals(ECCurve.NamedCurves.nistP256.Oid.Value, StringComparison.Ordinal))
            return COSE.EllipticCurve.P256;

        else if (curve.Oid.Value.Equals(ECCurve.NamedCurves.nistP384.Oid.Value, StringComparison.Ordinal))
            return COSE.EllipticCurve.P384;

        else if (curve.Oid.Value.Equals(ECCurve.NamedCurves.nistP521.Oid.Value, StringComparison.Ordinal))
            return COSE.EllipticCurve.P521;

        throw new Exception($"Invalid ECCurve. Was {curve.Oid}");
    }
}
