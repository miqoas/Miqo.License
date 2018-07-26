using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Miqo.License.Tests {
	// ReSharper disable once InconsistentNaming
	public class ECCSignerTests {
		[Fact]
		public void Signer_CanGenerateKeyPairs() {
			var signer = new ECC.Signer();
			Assert.NotNull(signer.PrivateKey);
			Assert.NotNull(signer.PublicKey);
		}

		[Fact]
		public void Signer_CanRecoverPublicKey() {
			var signer = new ECC.Signer();
			var privateKey = signer.PrivateKey;
			var oldKey = signer.PublicKey;

			signer = new ECC.Signer();
			var newKey = signer.GetPublicKeyForPrivateKey(privateKey);

			Assert.Equal(oldKey, newKey);
			Assert.NotEqual(signer.PrivateKey, privateKey);
		}

		[Fact]
		public void Signer_CanSignAndVerifyString() {
			var signer = new ECC.Signer();
			var digest = Encoding.UTF8.GetBytes("Friends are the adventures of life. - MacGyver");
			var signature = signer.Sign(digest, signer.PrivateKey);

			var result = signer.Verify(digest, signature, signer.PublicKey);
			Assert.True(result);
		}

		[Fact]
		public void Signer_WontVerifyInvalidSignature() {
			var signer = new ECC.Signer();
			var digest = Encoding.UTF8.GetBytes("Friends are the adventures of life. - MacGyver");
			var digest2 = Encoding.UTF8.GetBytes("MacGyver is kind of a ... troubleshooter. - Pete");
			var signature = signer.Sign(digest, signer.PrivateKey);

			var result = signer.Verify(digest2, signature, signer.PublicKey);
			Assert.False(result);
		}
	}
}
