using Cryptography.ECDSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UChainDB.BingChain.Engine.Cryptography.EC;

namespace UChainDB.BingChain.Engine.Cryptography {
	internal class Secp256k1 {
		public ECCurve SelectedCurve { get; } = ECCurve.Secp256k1;

		public Secp256k1() {
		}

		public byte[] GetPublicKey(byte[] privateKey) {
			var publicKey = this.SelectedCurve.G * privateKey;
			return publicKey.EncodePoint(true);
		}

		public byte[] Sign(byte[] privateKey, byte[] data) {
			return this.Sign(privateKey, new[] {data});
		}

		public byte[] Sign(byte[] privateKey, IEnumerable<byte[]> data) {
			var dataHash = HashBytes(data);
			var signature = Secp256k1Manager.SignCompressedCompact(dataHash, privateKey);
			var r = signature.Skip(1).Take(32).ToArray();
			var s = signature.Skip(33).Take(32).ToArray();
			var sig = r.Concat(s).ToArray();
			return sig;
		}

		public bool Verify(byte[] publicKey, byte[] sig, byte[] data) {
			return this.Verify(publicKey, sig, new[] {data});
		}

		public bool Verify(byte[] publicKey, byte[] sig, IEnumerable<byte[]> data) {
			var r = new BigInteger(((byte[]) sig).Take(32).Reverse().Concat(new byte[1]).ToArray());
			var s = new BigInteger(((byte[]) sig).Skip(32).Reverse().Concat(new byte[1]).ToArray());
			var pubKey = ECPoint.DecodePoint(publicKey, this.SelectedCurve);
			var dsa = new ECDsa(pubKey);
			var dataHash = HashBytes(data);
			return dsa.VerifySignature(dataHash, r, s);
		}

		private byte[] HashBytes(IEnumerable<byte[]> bytesArray) {
			if (bytesArray == null) {
				throw new ArgumentNullException(nameof(bytesArray));
			}

			using (var hash = System.Security.Cryptography.SHA256.Create()) {
				var e = bytesArray.GetEnumerator();
				while (e.MoveNext()) {
					var arr = e.Current;
					hash.TransformBlock(arr, 0, arr.Length, null, 0);
				}

				hash.TransformFinalBlock(new byte[] { }, 0, 0);
				return hash.Hash;
			}
		}
	}
}
