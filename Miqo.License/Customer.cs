namespace Miqo.License {
	/// <summary>
	/// The customer the <see cref="License"/> belongs to
	/// </summary>
	public class Customer {
		/// <summary>
		/// Gets or sets the Name of this <see cref="Customer"/>
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Company of this <see cref="Customer"/>
		/// </summary>
		public string Company { get; set; }

		/// <summary>
		/// Gets or sets the Email address of this <see cref="Customer"/>
		/// </summary>
		public string Email { get; set; }
	}
}