using System;
using System.Collections.Generic;

namespace Miqo.License {
	/// <summary>
	/// Implementation of the <see cref="IFluentInterface"/>, a fluent API to create new licenses.
	/// </summary>
	public class FluentLicense : ICanSetUniqueIdentifier, ICanSetTypeOrSetExpirationOrSetProduct, ICanSetExpirationOrSetProduct, ICanSetProduct, ICanSetProductFeaturesOrSetAdditionalFeaturesOrSetCustomer, ICanSetAdditionalFeaturesOrSetCustomer, ICanSetCustomer, ICanSignLicense {
		private readonly License _license;

		/// <summary>
		/// Initializes a new instance of the <see cref="FluentLicense"/> class.
		/// </summary>
		private FluentLicense() {
			_license = new License();
		}

		/// <summary>
		/// Creates a new <see cref="FluentLicense"/> instance, and starts the fluent API chain.
		/// </summary>
		/// <remarks>
		/// Suggested follow-up methods: <see cref="WithUniqueIdentifier(Guid)"/>
		/// </remarks>
		/// <returns>A <see cref="FluentLicense"/> that is used to create a new license.</returns>
		public static ICanSetUniqueIdentifier CreateLicense() {
			return new FluentLicense();
		}

		/// <summary>
		/// Adds a unique identifier to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <remarks>
		/// Suggested follow-up methods: <see cref="As(LicenseType)"/>, <see cref="ExpiresAt(DateTime)"/> or <see cref="ForProduct(string)"/>.
		/// </remarks>
		/// <param name="guid">The <see cref="Guid"/>.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		public ICanSetTypeOrSetExpirationOrSetProduct WithUniqueIdentifier(Guid guid) {
			_license.Id = guid;
			return this;
		}

		/// <summary>
		/// Adds a <see cref="LicenseType"/> to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <remarks>
		/// Suggested follow-up methods: <see cref="ExpiresAt(DateTime)"/> or <see cref="ForProduct(string)"/>
		/// </remarks>
		/// <param name="type">The <see cref="LicenseType"/>.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		public ICanSetExpirationOrSetProduct As(LicenseType type) {
			_license.LicenseType = type;
			return this;
		}

		/// <summary>
		/// Adds an expiration date to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <remarks>
		/// Suggested methods: <see cref="ForProduct(string)"/>
		/// </remarks>
		/// <param name="expiresAt">The expiration date.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		public ICanSetProduct ExpiresAt(DateTime expiresAt) {
			_license.ExpiresAt = expiresAt;
			return this;
		}

		/// <summary>
		/// Adds a product name to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <remarks>
		/// Suggested methods: <see cref="WithProductFeatures(Dictionary{string, string})"/>, <see cref="WithAdditionalFeatures(Dictionary{string, string})"/>
		/// or <see cref="LicensedTo(Customer)"/>.
		/// </remarks>
		/// <param name="name">The product name.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		public ICanSetProductFeaturesOrSetAdditionalFeaturesOrSetCustomer ForProduct(string name) {
			_license.Product = name;
			return this;
		}

		/// <summary>
		/// Adds the product features to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <remarks>
		/// Suggested methods: <see cref="WithAdditionalFeatures(Dictionary{string, string})"/>
		/// or <see cref="LicensedTo(Customer)"/>.
		/// </remarks>
		/// <param name="features">The product features.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		public ICanSetAdditionalFeaturesOrSetCustomer WithProductFeatures(Dictionary<string, string> features) {
			_license.ProductFeatures = features;
			return this;
		}

		/// <summary>
		/// Adds the additional features to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <remarks>
		/// Suggested methods: <see cref="LicensedTo(Customer)"/>.
		/// </remarks>
		/// <param name="features">The product features.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		public ICanSetCustomer WithAdditionalFeatures(Dictionary<string, string> features) {
			_license.AdditionalFeatures = features;
			return this;
		}

		/// <summary>
		/// Adds the customer to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <remarks>
		/// Complete the chain with <see cref="SignLicenseWithPrivateKey(byte[])"/>.
		/// </remarks>
		/// <param name="customer">The customer.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		public ICanSignLicense LicensedTo(Customer customer) {
			_license.Customer = customer;
			return this;
		}

		/// <summary>
		/// Adds the customer to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <remarks>
		/// Complete the chain with <see cref="SignLicenseWithPrivateKey(byte[])"/>.
		/// </remarks>
		/// <param name="name">The name of the customer.</param>
		/// <param name="email">The email address of the customer.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		public ICanSignLicense LicensedTo(string name, string email) {
			_license.Customer = new Customer {
				Name = name,
				Email = email
			};
			return this;
		}

		/// <summary>
		/// Adds the customer to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <remarks>
		/// Complete the chain with <see cref="SignLicenseWithPrivateKey(byte[])"/>.
		/// </remarks>
		/// <param name="name">The name of the customer.</param>
		/// <param name="email">The email address of the customer.</param>
		/// <param name="company">The company name of the customer.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		public ICanSignLicense LicensedTo(string name, string email, string company) {
			_license.Customer = new Customer {
				Name = name,
				Email = email,
				Company = company
			};
			return this;
		}

		/// <summary>
		/// Completes the <see cref="FluentLicense"/> chain and creates a <see cref="License"/>.
		/// </summary>
		/// <param name="privateKey">The private key for this product.</param>
		/// <returns>A <see cref="License"/>.</returns>
		public License SignLicenseWithPrivateKey(byte[] privateKey) {
			return _license.Sign(privateKey);
		}
	}
}
