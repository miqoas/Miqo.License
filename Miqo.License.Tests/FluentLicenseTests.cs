using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Xunit;
using Miqo.License;

namespace Miqo.License.Tests {
	public class FluentLicenseTests {
		private byte[] _privateKey;
		private byte[] _publicKey;

		public FluentLicenseTests() {
			var signer = new ECC.Signer();
			_privateKey = signer.PrivateKey;
			_publicKey = signer.PublicKey;
		}

		[Fact]
		public void FluentLicense_CanGenerateValidLicense() {
			var guid = Guid.NewGuid();
			var createdAt = DateTime.UtcNow;
			var licenseType = LicenseType.Standard;
			var expiresAt = DateTime.Today.AddYears(1);
			var features = new Dictionary<string, string> {
				{"Version", "1"},
			};
			var product = "Beep Boop";
			var customer = new Customer {
				Name = @"Angus ""Mac"" MacGyver",
				Email = "macgyver@phoenixfoundation.org",
				Company = "Phoenix Foundation"
			};

			var license = FluentLicense
				.CreateLicense()
				.WithUniqueIdentifier(guid)
				.As(licenseType)
				.ExpiresAt(expiresAt)
				.ForProduct(product)
				.WithProductFeatures(features)
				.LicensedTo(customer)
				.SignLicenseWithPrivateKey(_privateKey);

			Assert.True(license.VerifySignature(_publicKey));
			license.CreatedAt = createdAt;

			Assert.NotNull(license);
			Assert.NotNull(license.Signature);
			var o = JObject.Parse(license.ToJsonString());
			Assert.NotNull(o);
			Assert.Equal(guid, license.Id);
			Assert.Equal(createdAt, license.CreatedAt);
			Assert.Equal(licenseType, license.LicenseType);
			Assert.Equal(expiresAt, license.ExpiresAt);
			Assert.Equal(product, license.Product);
			Assert.Equal(features, license.ProductFeatures);
			Assert.Null(license.AdditionalFeatures);
			Assert.Equal(customer.Name, license.Customer.Name);
			Assert.Equal(customer.Email, license.Customer.Email);
			Assert.Equal(customer.Company, license.Customer.Company);
			Assert.NotEqual(license.ProductFeatures, license.AdditionalFeatures);
		}
	}
}
