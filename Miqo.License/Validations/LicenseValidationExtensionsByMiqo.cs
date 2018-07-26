using System;
using System.Collections.Generic;
using System.Text;

namespace Miqo.License.Validations {
	public static class LicenseValidationExtensionsByMiqo {
		/// <summary>
		/// Validates if the license covers the correct product
		/// </summary>
		/// <param name="validationChain">The current <see cref="IStartValidationChain"/>.</param>
		/// <param name="product">The product name.</param>
		/// <returns>An instance of <see cref="IStartValidationChain"/>.</returns>
		public static IValidationChain ProductName(this IStartValidationChain validationChain, string product) {
			var validationChainBuilder = (validationChain as ValidationChainBuilder);
			var validator = validationChainBuilder.StartValidatorChain();

			validator.Validate = license => license.Product.Equals(product);

			validator.FailureResult = new ProductNotLicensedFailure {
				Message = $"{product} is not covered by this license.",
				HowToResolve = @"Your license does not cover this license. Please contact your distributor/vendor to get the correct license."
			};

			return validationChainBuilder;
		}
	}
}
