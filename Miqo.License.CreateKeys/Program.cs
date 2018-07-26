using System;
using System.Text;

namespace Miqo.License.CreateKeys {
	class Program {
		private static ECC.Signer _signer = new ECC.Signer();

		static void Main(string[] args) {
			OutputHeader();

			OutputPrivateKey();
			OutputPublicKey();

			Console.ReadKey();
		}

		private static void OutputHeader() {
			Console.OutputEncoding = Encoding.UTF8;

			Console.Title = "Miqo.License: Key Creation Tool";
			Console.Clear();
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("Key Creation Tool");
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine("Generates a new key pair for use with Miqo.License");
			Console.WriteLine();
			Console.WriteLine();
		}

		private static void OutputPrivateKey() {
			Console.WriteLine($"Private key:");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine($"{ECC.HexExtensions.ToHex(_signer.PrivateKey)}");
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine("--");
		}

		private static void OutputPublicKey() {
			Console.WriteLine($"Public key:");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine($"{ECC.HexExtensions.ToHex(_signer.PublicKey)}");
			Console.WriteLine();
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine("See README for details on how to use the keys: https://github.com/miqo-no/Miqo.License");
		}
	}
}
