## ChainEngine.Cryptography.EcDsa.Secp256k1

https://github.com/uchaindb/Cryptography.EcDsa.Secp256k1

This library compiles transaction signing and verifying algorithm secp256k1 and SHA256 which is used in several blockchains like Bitcoin, EOS and Graphene-based Steem, Golos, BitShares and of course in blockchain database [UChainDb](https://github.com/uchaindb).

* For verifying part, utilized MIT code from [AntShares].
* For signing part, utilizing [Cryptography.ECDSA] for far better performance.

[Cryptography.ECDSA]: https://github.com/Chainers/Cryptography.ECDSA.git
[AntShares]: https://github.com/AntShares/AntShares.git

### Usage

```cs
using UChainDB.BingChain.Engine.Cryptography;

var signAlgo = new Secp256k1();
var pubKey = signAlgo.GetPublicKey(privKey);
var data = Encoding.UTF8.GetBytes(message);

// signing
var sig = signAlgo.Sign(privKey, data);

// verifying
var result = signAlgo.Verify(pubKey, sig, data);
```

### Installation

```
Install-Package UChainDB.ChainEngine.Cryptography.EcDsa.Secp256k1
```

### API

```cs
public byte[] GetPublicKey(byte[] privateKey)
public byte[] Sign(byte[] privateKey, byte[] data)
public byte[] Sign(byte[] privateKey, IEnumerable<byte[]> data)
public bool Verify(byte[] publicKey, byte[] sig, byte[] data)
public bool Verify(byte[] publicKey, byte[] sig, IEnumerable<byte[]> data)
```

### License

MIT
