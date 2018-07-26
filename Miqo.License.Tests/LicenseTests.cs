using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using Miqo.License;

namespace Miqo.License.Tests {
	public class LicenseTests {
		private byte[] _privateKey;
		private byte[] _publicKey;
		private readonly ITestOutputHelper _output;

		public LicenseTests(ITestOutputHelper output) {
			_output = output;
			var signer = new ECC.Signer();
			_privateKey = signer.PrivateKey;
			_publicKey = signer.PublicKey;
		}

		[Fact]
		public void Save_ThrowsExceptionIfUnsignedLicense() {
			var l = new License {
				Id = new Guid()
			};

			Assert.Throws<ArgumentException>(() => l.Save("Miqo.License"));
		}

		[Fact]
		public void VerifySignature_CanVerifyEmptyLicense() {
			var l = new License();
			l.Sign(_privateKey);
			Assert.True(l.VerifySignature(_publicKey));
		}
	}
}
