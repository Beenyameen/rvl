# rvl (C# / .NET)

C# port of the RVL algorithm, a lossless compression technique designed for 16-bit depth images.

For full details, see the [root repository README](https://github.com/Beenyameen/rvl/blob/main/README.md).

## Installation

```bash
dotnet add package rvl
```

## Example

```csharp
using Rvl;

// Create a sample 16-bit array (e.g., depth data).
short[] data = { 0, 0, 100, 102, 105, 0, 0, 0, 250, 255 };
byte[] rawBytes = new byte[data.Length * 2];
System.Buffer.BlockCopy(data, 0, rawBytes, 0, rawBytes.Length);

// Compress.
byte[] compressedData = RvlCodec.Compress(rawBytes);

// Decompress.
byte[] decompressedBytes = RvlCodec.Decompress(compressedData);
```