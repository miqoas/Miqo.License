using System;
using Cryptography.ECDSA;
using UChainDB.BingChain.Engine.Cryptography;

namespace Miqo.License.ECC {
	/// <summary>
	/// Represents a <see cref="License"/> signer.
	/// </summary>
	public class Signer {
		private Secp256k1 _algorithm = new Secp256k1();

		/// <summary>
		/// Gets the private key of this <see cref="Signer"/>.
		/// </summary>
		public byte[] PrivateKey { get; private set; }

		/// <summary>
		/// Gets the public key of this <see cref="Signer"/>.
		/// </summary>
		public byte[] PublicKey { get; private set; }

		/// <summary>
		/// Creates a new <see cref="Signer"/> and generates a <see cref="PrivateKey"/> and a <see cref="PublicKey"/>.
		/// </summary>
		public Signer() {
			PrivateKey = Secp256k1Manager.GenerateRandomKey();
			PublicKey = _algorithm.GetPublicKey(PrivateKey);
		}

		/// <summary>
		/// Creates a new <see cref="Signer"/> and recovers the <see cref="PublicKey"/> for the provided <paramref name="privateKey"/>.
		/// </summary>
		/// <param name="privateKey">The private key.</param>
		public Signer(byte[] privateKey) {
			PrivateKey = privateKey;
			PublicKey = _algorithm.GetPublicKey(PrivateKey);
		}

		/// <summary>
		/// Creates a new <see cref="Signer"/> and recovers the <see cref="PublicKey"/> for the provided <paramref name="privateKey"/>.
		/// </summary>
		/// <param name="privateKey">The private key in string format.</param>
		public Signer(string privateKey) {
			PrivateKey = privateKey.HexToBytes();
			PublicKey = _algorithm.GetPublicKey(PrivateKey);
		}

		/// <summary>
		/// Recovers the <see cref="PublicKey"/> for a given <paramref name="privateKey"/>.
		/// </summary>
		/// <param name="privateKey">The private key.</param>
		/// <returns>The public key.</returns>
		public byte[] GetPublicKeyForPrivateKey(byte[] privateKey) {
			_algorithm = new Secp256k1();
			return _algorithm.GetPublicKey(privateKey);

		}

		/// <summary>
		/// Sign a <paramref name="digest"/> using a <paramref name="privateKey"/>.
		/// </summary>
		/// <param name="digest">The digest to be signed.</param>
		/// <param name="privateKey">The private key.</param>
		/// <returns>The signature.</returns>
		public byte[] Sign(byte[] digest, byte[] privateKey) {
			if (digest == null) throw new ArgumentNullException(nameof(digest));
			if (privateKey == null) throw new ArgumentNullException(nameof(privateKey));

			return _algorithm.Sign(privateKey, digest);
		}

		/// <summary>
		/// Verify that authenticity of the <paramref name="digest"/>'s <paramref name="signature"/> using <paramref name="publicKey"/>.
		/// </summary>
		/// <param name="digest">The digest.</param>
		/// <param name="signature">The signature.</param>
		/// <param name="publicKey">The public key.</param>
		/// <returns>true if the signature is authentic, false otherwise.</returns>
		public bool Verify(byte[] digest, byte[] signature, byte[] publicKey) {
			if (digest == null) throw new ArgumentNullException(nameof(digest));
			if (signature == null) throw new ArgumentNullException(nameof(signature));
			if (publicKey == null) throw new ArgumentNullException(nameof(publicKey));

			return _algorithm.Verify(publicKey, signature, digest);
		}

		/// <summary>
		/// Verify that authenticity of the <paramref name="digest"/>'s <paramref name="signature"/> using <paramref name="publicKey"/>.
		/// </summary>
		/// <param name="digest">The digest.</param>
		/// <param name="signature">The signature.</param>
		/// <param name="publicKey">The public key.</param>
		/// <returns>true if the signature is authentic, false otherwise.</returns>
		public bool Verify(byte[] digest, byte[] signature, string publicKey) {
			if (digest == null) throw new ArgumentNullException(nameof(digest));
			if (signature == null) throw new ArgumentNullException(nameof(signature));
			if (publicKey == null) throw new ArgumentNullException(nameof(publicKey));

			var key = publicKey.HexToBytes();
			return _algorithm.Verify(key, signature, digest);
		}
	}
}
