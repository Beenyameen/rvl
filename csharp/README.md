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

// Compress.
byte[] compressedData = RvlCodec.Compress(data);

// Decompress.
short[] decompressedData = RvlCodec.Decompress(compressedData);
```
