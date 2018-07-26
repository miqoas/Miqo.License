using System;
using System.Collections.Generic;

namespace Miqo.License {
	public interface ICanSetUniqueIdentifier : IFluentInterface {
		/// <summary>
		/// Adds a unique identifier to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <param name="guid">The <see cref="Guid"/>.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		ICanSetTypeOrSetExpirationOrSetProduct WithUniqueIdentifier(Guid guid);
	}

	public interface ICanSetTypeOrSetExpirationOrSetProduct : IFluentInterface {
		/// <summary>
		/// Adds a <see cref="LicenseType"/> to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <param name="type">The <see cref="LicenseType"/>.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		ICanSetExpirationOrSetProduct As(LicenseType type);

		/// <summary>
		/// Adds an expiration date to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <param name="expiresAt">The expiration date.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		ICanSetProduct ExpiresAt(DateTime expiresAt);

		/// <summary>
		/// Adds a product name to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <param name="name">The product name.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		ICanSetProductFeaturesOrSetAdditionalFeaturesOrSetCustomer ForProduct(string name);
	}

	public interface ICanSetExpirationOrSetProduct : IFluentInterface {
		/// <summary>
		/// Adds an expiration date to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <param name="expiresAt">The expiration date.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		ICanSetProduct ExpiresAt(DateTime expiresAt);

		/// <summary>
		/// Adds a product name to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <param name="name">The product name.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		ICanSetProductFeaturesOrSetAdditionalFeaturesOrSetCustomer ForProduct(string name);
	}

	public interface ICanSetProduct : IFluentInterface {
		/// <summary>
		/// Adds a product name to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <param name="name">The product name.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		ICanSetProductFeaturesOrSetAdditionalFeaturesOrSetCustomer ForProduct(string name);
	}

	public interface ICanSetProductFeaturesOrSetAdditionalFeaturesOrSetCustomer : IFluentInterface {
		/// <summary>
		/// Adds the product features to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <param name="features">The product features.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		ICanSetAdditionalFeaturesOrSetCustomer WithProductFeatures(Dictionary<string, string> features);

		/// <summary>
		/// Adds the additional features to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <param name="features">The product features.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		ICanSetCustomer WithAdditionalFeatures(Dictionary<string, string> features);

		/// <summary>
		/// Adds the customer to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <param name="customer">The customer.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		ICanSignLicense LicensedTo(Customer customer);

		/// <summary>
		/// Adds the customer to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <param name="name">The name of the customer.</param>
		/// <param name="email">The email address of the customer.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		ICanSignLicense LicensedTo(string name, string email);

		/// <summary>
		/// Adds the customer to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <param name="name">The name of the customer.</param>
		/// <param name="email">The email address of the customer.</param>
		/// <param name="company">The company name of the customer.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		ICanSignLicense LicensedTo(string name, string email, string company);
	}

	public interface ICanSetAdditionalFeaturesOrSetCustomer : IFluentInterface {
		/// <summary>
		/// Adds the additional features to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <param name="features">The product features.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		ICanSetCustomer WithAdditionalFeatures(Dictionary<string, string> features);

		/// <summary>
		/// Adds the customer to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <param name="customer">The customer.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		ICanSignLicense LicensedTo(Customer customer);

		/// <summary>
		/// Adds the customer to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <param name="name">The name of the customer.</param>
		/// <param name="email">The email address of the customer.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		ICanSignLicense LicensedTo(string name, string email);

		/// <summary>
		/// Adds the customer to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <param name="name">The name of the customer.</param>
		/// <param name="email">The email address of the customer.</param>
		/// <param name="company">The company name of the customer.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		ICanSignLicense LicensedTo(string name, string email, string company);
	}

	public interface ICanSetCustomer : IFluentInterface {
		ICanSignLicense LicensedTo(Customer customer);

		/// <summary>
		/// Adds the customer to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <param name="name">The name of the customer.</param>
		/// <param name="email">The email address of the customer.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		ICanSignLicense LicensedTo(string name, string email);

		/// <summary>
		/// Adds the customer to this <see cref="FluentLicense"/> chain.
		/// </summary>
		/// <param name="name">The name of the customer.</param>
		/// <param name="email">The email address of the customer.</param>
		/// <param name="company">The company name of the customer.</param>
		/// <returns>A <see cref="FluentLicense"/>.</returns>
		ICanSignLicense LicensedTo(string name, string email, string company);
	}

	public interface ICanSignLicense : IFluentInterface {
		License SignLicenseWithPrivateKey(byte[] privateKey);
	}
}
