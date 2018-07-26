using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;

namespace UChainDB.BingChain.Engine.Cryptography.EC {
	internal class ECPoint : IComparable<ECPoint>, IEquatable<ECPoint> {
		internal readonly ECFieldElement X;
		internal readonly ECFieldElement Y;
		internal readonly ECCurve Curve;

		/// <summary>
		/// 判断是否为无穷远点
		/// </summary>
		private bool IsInfinity => X == null && Y == null;

		public int Size => IsInfinity ? 1 : 33;

		internal ECPoint(ECFieldElement x, ECFieldElement y, ECCurve curve) {
			if ((x != null && y == null) || (x == null && y != null))
				throw new ArgumentException("Exactly one of the field elements is null");
			X = x;
			Y = y;
			Curve = curve;
		}

		/// <summary>
		/// 与另一对象进行比较
		/// </summary>
		/// <param name="other">另一对象</param>
		/// <returns>返回比较的结果</returns>
		public int CompareTo(ECPoint other) {
			if (ReferenceEquals(this, other)) return 0;
			var result = X.CompareTo(other.X);
			return result != 0 ? result : Y.CompareTo(other.Y);
		}

		/// <summary>
		/// 从字节数组中解码
		/// </summary>
		/// <param name="encoded">要解码的字节数组</param>
		/// <param name="curve">曲线参数</param>
		/// <returns></returns>
		public static ECPoint DecodePoint(byte[] encoded, ECCurve curve) {
			ECPoint p = null;
			var expectedLength = (curve.Q.GetBitLength() + 7) / 8;
			switch (encoded[0]) {
				case 0x00: // infinity
				{
					if (encoded.Length != 1)
						throw new ArgumentException(@"Incorrect length for infinity encoding", nameof(encoded));
					p = curve.Infinity;
					break;
				}
				case 0x02: // compressed
				case 0x03: // compressed
				{
					if (encoded.Length != (expectedLength + 1))
						throw new ArgumentException(@"Incorrect length for compressed encoding", nameof(encoded));
					var yTilde = encoded[0] & 1;
					var x1 = new BigInteger(encoded.Skip(1).Reverse().Concat(new byte[1]).ToArray());
					p = DecompressPoint(yTilde, x1, curve);
					break;
				}
				case 0x04: // uncompressed
				case 0x06: // hybrid
				case 0x07: // hybrid
				{
					if (encoded.Length != (2 * expectedLength + 1))
						throw new ArgumentException(@"Incorrect length for uncompressed/hybrid encoding", nameof(encoded));
					var x1 = new BigInteger(encoded.Skip(1).Take(expectedLength).Reverse().Concat(new byte[1]).ToArray());
					var y1 = new BigInteger(encoded.Skip(1 + expectedLength).Reverse().Concat(new byte[1]).ToArray());
					p = new ECPoint(new ECFieldElement(x1, curve), new ECFieldElement(y1, curve), curve);
					break;
				}
				default:
					throw new FormatException("Invalid point encoding " + encoded[0]);
			}

			return p;
		}

		private static ECPoint DecompressPoint(int yTilde, BigInteger X1, ECCurve curve) {
			var x = new ECFieldElement(X1, curve);
			var alpha = x * (x.Square() + curve.A) + curve.B;
			var beta = alpha.Sqrt();

			//
			// if we can't find a sqrt we haven't got a point on the
			// curve - run!
			//
			if (beta == null)
				throw new ArithmeticException("Invalid point compression");

			var betaValue = beta.Value;
			var bit0 = betaValue.IsEven ? 0 : 1;

			if (bit0 != yTilde) {
				// Use the other root
				beta = new ECFieldElement(curve.Q - betaValue, curve);
			}

			return new ECPoint(x, beta, curve);
		}

		/// <summary>
		/// 反序列化
		/// </summary>
		/// <param name="reader">数据来源</param>
		/// <param name="curve">椭圆曲线参数</param>
		/// <returns></returns>
		public static ECPoint DeserializeFrom(BinaryReader reader, ECCurve curve) {
			var expectedLength = (curve.Q.GetBitLength() + 7) / 8;
			var buffer = new byte[1 + expectedLength * 2];
			buffer[0] = reader.ReadByte();
			switch (buffer[0]) {
				case 0x00:
					return curve.Infinity;
				case 0x02:
				case 0x03:
					reader.Read(buffer, 1, expectedLength);
					return DecodePoint(buffer.Take(1 + expectedLength).ToArray(), curve);
				case 0x04:
				case 0x06:
				case 0x07:
					reader.Read(buffer, 1, expectedLength * 2);
					return DecodePoint(buffer, curve);
				default:
					throw new FormatException("Invalid point encoding " + buffer[0]);
			}
		}

		/// <summary>
		/// 将对象编码到字节数组
		/// </summary>
		/// <param name="commpressed">是否为压缩格式的编码</param>
		/// <returns>返回编码后的字节数组</returns>
		public byte[] EncodePoint(bool commpressed) {
			if (IsInfinity) return new byte[1];
			byte[] data;
			if (commpressed) {
				data = new byte[33];
			}
			else {
				data = new byte[65];
				var yBytes = Y.Value.ToByteArray().Reverse().ToArray();
				Buffer.BlockCopy(yBytes, 0, data, 65 - yBytes.Length, yBytes.Length);
			}

			var xBytes = X.Value.ToByteArray().Reverse().ToArray();
			Buffer.BlockCopy(xBytes, 0, data, 33 - xBytes.Length, xBytes.Length);
			data[0] = commpressed ? Y.Value.IsEven ? (byte) 0x02 : (byte) 0x03 : (byte) 0x04;
			return data;
		}

		/// <summary>
		/// 比较与另一个对象是否相等
		/// </summary>
		/// <param name="other">另一个对象</param>
		/// <returns>返回比较的结果</returns>
		public bool Equals(ECPoint other) {
			if (ReferenceEquals(this, other)) return true;
			if (other is null) return false;
			if (IsInfinity && other.IsInfinity) return true;
			if (IsInfinity || other.IsInfinity) return false;
			return X.Equals(other.X) && Y.Equals(other.Y);
		}

		/// <summary>
		/// 比较与另一个对象是否相等
		/// </summary>
		/// <param name="obj">另一个对象</param>
		/// <returns>返回比较的结果</returns>
		public override bool Equals(object obj) => Equals(obj as ECPoint);

		/// <summary>   
		/// 从指定的字节数组中解析出公钥，这个字节数组可以是任意形式的公钥编码、或者包含私钥的内容
		/// </summary>
		/// <param name="pubkey">要解析的字节数组</param>
		/// <param name="curve">椭圆曲线参数</param>
		/// <returns>返回解析出的公钥</returns>
		public static ECPoint FromBytes(byte[] pubkey, ECCurve curve) {
			switch (pubkey.Length) {
				case 33:
				case 65:
					return DecodePoint(pubkey, curve);
				case 64:
				case 72:
					return DecodePoint(new byte[] {0x04}.Concat(pubkey.Skip(pubkey.Length - 64)).ToArray(), curve);
				case 96:
				case 104:
					return DecodePoint(new byte[] {0x04}.Concat(pubkey.Skip(pubkey.Length - 96).Take(64)).ToArray(), curve);
				default:
					throw new FormatException();
			}
		}

		/// <summary>
		/// 获取HashCode
		/// </summary>
		/// <returns>返回HashCode</returns>
		public override int GetHashCode() => X.GetHashCode() + Y.GetHashCode();

		internal static ECPoint Multiply(ECPoint p, BigInteger k) {
			// floor(log2(k))
			var m = k.GetBitLength();

			// width of the Window NAF
			sbyte width;

			// Required length of precomputation array
			int reqPreCompLen;

			// Determine optimal width and corresponding length of precomputation
			// array based on literature values
			if (m < 13) {
				width = 2;
				reqPreCompLen = 1;
			}
			else if (m < 41) {
				width = 3;
				reqPreCompLen = 2;
			}
			else if (m < 121) {
				width = 4;
				reqPreCompLen = 4;
			}
			else if (m < 337) {
				width = 5;
				reqPreCompLen = 8;
			}
			else if (m < 897) {
				width = 6;
				reqPreCompLen = 16;
			}
			else if (m < 2305) {
				width = 7;
				reqPreCompLen = 32;
			}
			else {
				width = 8;
				reqPreCompLen = 127;
			}

			// The length of the precomputation array
			var preCompLen = 1;

			ECPoint[] preComp = preComp = new ECPoint[] {p};
			var twiceP = p.Twice();

			if (preCompLen < reqPreCompLen) {
				// Precomputation array must be made bigger, copy existing preComp
				// array into the larger new preComp array
				var oldPreComp = preComp;
				preComp = new ECPoint[reqPreCompLen];
				Array.Copy(oldPreComp, 0, preComp, 0, preCompLen);

				for (var i = preCompLen; i < reqPreCompLen; i++) {
					// Compute the new ECPoints for the precomputation array.
					// The values 1, 3, 5, ..., 2^(width-1)-1 times p are
					// computed
					preComp[i] = twiceP + preComp[i - 1];
				}
			}

			// Compute the Window NAF of the desired width
			var wnaf = WindowNaf(width, k);
			var l = wnaf.Length;

			// Apply the Window NAF to p using the precomputed ECPoint values.
			var q = p.Curve.Infinity;
			for (var i = l - 1; i >= 0; i--) {
				q = q.Twice();

				if (wnaf[i] == 0) continue;
				if (wnaf[i] > 0) {
					q += preComp[(wnaf[i] - 1) / 2];
				}
				else {
					// wnaf[i] < 0
					q -= preComp[(-wnaf[i] - 1) / 2];
				}
			}

			return q;
		}

		public static ECPoint Parse(string value, ECCurve curve) => DecodePoint(value.HexToBytes(), curve);

		public override string ToString() => EncodePoint(true).ToHexString();

		internal ECPoint Twice() {
			if (IsInfinity)
				return this;
			if (Y.Value.Sign == 0)
				return Curve.Infinity;
			var two = new ECFieldElement(2, Curve);
			var three = new ECFieldElement(3, Curve);
			var gamma = (X.Square() * three + Curve.A) / (Y * two);
			var x3 = gamma.Square() - X * two;
			var y3 = gamma * (X - x3) - Y;
			return new ECPoint(x3, y3, Curve);
		}

		private static sbyte[] WindowNaf(sbyte width, BigInteger k) {
			var wnaf = new sbyte[k.GetBitLength() + 1];
			var pow2wB = (short) (1 << width);
			var i = 0;
			var length = 0;
			while (k.Sign > 0) {
				if (!k.IsEven) {
					var remainder = k % pow2wB;
					if (remainder.TestBit(width - 1)) {
						wnaf[i] = (sbyte) (remainder - pow2wB);
					}
					else {
						wnaf[i] = (sbyte) remainder;
					}

					k -= wnaf[i];
					length = i;
				}
				else {
					wnaf[i] = 0;
				}

				k >>= 1;
				i++;
			}

			length++;
			var wnafShort = new sbyte[length];
			Array.Copy(wnaf, 0, wnafShort, 0, length);
			return wnafShort;
		}

		public static ECPoint operator -(ECPoint x) {
			return new ECPoint(x.X, -x.Y, x.Curve);
		}

		public static ECPoint operator *(ECPoint p, byte[] n) {
			if (p == null || n == null)
				throw new ArgumentNullException();
			if (n.Length != 32)
				throw new ArgumentException();
			if (p.IsInfinity)
				return p;
			//BigInteger的内存无法被保护，可能会有安全隐患。此处的k需要重写一个SecureBigInteger类来代替
			var k = new BigInteger(n.Reverse().Concat(new byte[1]).ToArray());
			return k.Sign == 0 ? p.Curve.Infinity : Multiply(p, k);
		}

		public static ECPoint operator +(ECPoint x, ECPoint y) {
			if (x.IsInfinity)
				return y;
			if (y.IsInfinity)
				return x;
			if (x.X.Equals(y.X)) {
				if (x.Y.Equals(y.Y))
					return x.Twice();
				Debug.Assert(x.Y.Equals(-y.Y));
				return x.Curve.Infinity;
			}

			var gamma = (y.Y - x.Y) / (y.X - x.X);
			var x3 = gamma.Square() - x.X - y.X;
			var y3 = gamma * (x.X - x3) - x.Y;
			return new ECPoint(x3, y3, x.Curve);
		}

		public static ECPoint operator -(ECPoint x, ECPoint y) {
			if (y.IsInfinity)
				return x;
			return x + (-y);
		}
	}
}
