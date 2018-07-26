namespace Miqo.License {
	/// <summary>
	/// Defines the type of <see cref="License"/>.
	/// </summary>
	public enum LicenseType {
		/// <summary>
		/// For trial or demo use
		/// </summary>
		Trial = 0,

		/// <summary>
		/// Standard license
		/// </summary>
		Standard = 1,

		/// <summary>
		/// Personal license
		/// </summary>
		Personal = 2,
		
		/// <summary>
		/// Custom license
		/// </summary>
		Custom = 9
	}
}
