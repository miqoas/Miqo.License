using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Miqo.License {
	/// <summary>
	/// Represents a software license.
	/// </summary>
	public class License {
		#region Properties
		
		/// <summary>Gets or sets a unique identifier for this <see cref="License"/>.</summary>
		public Guid Id { get; set; }

		/// <summary>Gets or sets the creation date and time of this <see cref="License"/>.</summary>
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		/// <summary>Gets or sets the name of the software this <see cref="License"/> covers.</summary>
		public string Product { get; set; }

		/// <summary>Gets or sets the <see cref="LicenseType"/> of this <see cref="License"/>.</summary>
		[JsonConverter(typeof(StringEnumConverter))]
		public LicenseType LicenseType { get; set; } = LicenseType.Trial;

		/// <summary>Gets or sets the product features of this <see cref="License"/>.</summary>
		public Dictionary<string, string> ProductFeatures { get; set; }

		/// <summary>Gets or sets the additional features of this <see cref="License"/>.</summary>
		public Dictionary<string, string> AdditionalFeatures { get; set; }

		/// <summary>Gets or sets the <see cref="Customer"/> of this <see cref="License"/>.</summary>
		public Customer Customer { get; set; } = new Customer();

		/// <summary>Gets or sets the expiration date of this <see cref="License"/>.</summary>
		public DateTime ExpiresAt { get; set; }

		/// <summary>Gets or sets the signature <see cref="License"/>.</summary>
		[JsonIgnore]
		public byte[] Signature { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Loads a <see cref="License"/> from a file.
		/// </summary>
		/// <param name="file">The file name.</param>
		/// <returns>A software license.</returns>
		public static License Load(string file) {
			var json = File.ReadAllText(file, Encoding.UTF8);
			return LoadFromString(json);
		}

		/// <summary>
		/// Parses a JSON string to create a <see cref="License"/>.
		/// </summary>
		/// <param name="json">A <see cref="string"/> that contains the JSON</param>
		/// <returns>A software license.</returns>
		public static License LoadFromString(string json) {
			var j = JObject.Parse(json);
			var l = j["license"].ToObject<License>();
			l.Signature = (byte[]) j["signature"];

			return l;
		}

		/// <summary>
		/// Saves the <see cref="License"/> to a file.
		/// </summary>
		/// <param name="file">Path and name of file.</param>
		/// <exception cref="ArgumentException">Thrown if the License hasn't been signed.</exception>
		public void Save(string file) {
			if (!IsSigned) throw new ArgumentException($"License hasn't been signed");
			File.WriteAllText(file, ToJsonString(), Encoding.UTF8);
		}

		/// <summary>
		/// Generates a signature and signs the <see cref="License"/> with the provided key.
		/// </summary>
		/// <param name="privateKey">The private key.</param>
		/// <returns>A signed <see cref="License"/>.</returns>
		public License Sign(byte[] privateKey) {
			Signature = null;
			var j = JObject.FromObject(this);
			var json = Encoding.UTF8.GetBytes(j.ToString(Formatting.None));

			var signer = new ECC.Signer();
			Signature = signer.Sign(json, privateKey ?? signer.PrivateKey);
			return this;
		}

		/// <summary>
		/// Checks if the <see cref="License.Signature"/> property can be verified using the provided public key.
		/// </summary>
		/// <param name="publicKey">The public key for this product.</param>
		/// <returns>true if the <see cref="License.Signature"/> is correct; otherwise false.</returns>
		public bool VerifySignature(byte[] publicKey) {
			var sig = Signature;
			Signature = null;

			try {
				var j = JObject.FromObject(this);
				var json = Encoding.UTF8.GetBytes(j.ToString(Formatting.None));

				var signer = new ECC.Signer();
				return signer.Verify(json, sig, publicKey);
			}
			finally {
				Signature = sig;
			}
		}

		/// <summary>Gets a value indicating if the <see cref="License"/> has been signed.</summary>
		private bool IsSigned => !string.IsNullOrEmpty(Signature?.ToString());

		/// <summary>
		/// Serializes this <see cref="License"/> into a JSON formatted <see cref="string"/>.
		/// </summary>
		/// <remarks>Properties that haven't been set will be removed from the JSON <see cref="string"/>.</remarks>
		/// <returns>A JSON formatted <see cref="string"/>.</returns>
		public string ToJsonString() {
			var data = new {
				license = this,
				signature = Signature
			};

			var j = JObject.FromObject(data, new JsonSerializer {
				NullValueHandling = NullValueHandling.Ignore,
				ContractResolver = new DefaultContractResolver {NamingStrategy = new CamelCaseNamingStrategy()}
			});
			return j.ToString();
		}

		#endregion
	}
}
