using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Miqo.License.Validations;

namespace Miqo.License.Tests {
	public class LicenseValidationTests {
		private byte[] _privateKey;
		private byte[] _publicKey;

		public LicenseValidationTests() {
			var signer = new ECC.Signer();
			_privateKey = signer.PrivateKey;
			_publicKey = signer.PublicKey;
		}

		[Fact]
		public void VerifySignature_CanDetectLicenseTampering() {
			var license = new License {
				Id = Guid.NewGuid(),
				LicenseType = LicenseType.Trial,
				ExpiresAt = DateTime.Today.AddDays(-7),
				Product = "Chewing Gum",
				AdditionalFeatures = new Dictionary<string, string> {
					{"bob-omb", "disarmed"}
				},
				Customer = new Customer {
					Name = @"Angus 'Mac' MacGyver",
					Email = "macgyver@phoenixfoundation.org",
					Company = "Phoenix Foundation"
				}
			};

			license.Sign(_privateKey);
			Assert.True(license.VerifySignature(_publicKey));

			var tampered = License.LoadFromString(license.ToJsonString());
			tampered.LicenseType = LicenseType.Standard;
			tampered.AdditionalFeatures["bob-omb"] = "armed";
			tampered.ExpiresAt = DateTime.Today.AddYears(20);

			var validationFailures = tampered.Validate()
				.ProductName("Beep Boop")
				.And()
				.ExpirationDate()
				.And()
				.Signature(_publicKey)
				.AssertValidLicense();

			Assert.True(validationFailures.Any());
		}

		[Fact]
		public void VerifySignature_CanDetectWrongPublicKey() {
			var wrongKey = new ECC.Signer().PublicKey;
			var l = new License();
			l.Sign(_privateKey);
			Assert.False(l.VerifySignature(wrongKey));
		}
	}
}
