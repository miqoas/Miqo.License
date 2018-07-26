using System;

namespace Cryptography.ECDSA {
	internal class Callback : EventArgs {
		public Callback() {
		}

		public Callback(string message) {
			Message = message;
		}

		public string Message;
	}
}
