# rvl (C# / .NET)

[![NuGet version](https://badge.fury.io/nu/rvl.svg)](https://badge.fury.io/nu/rvl)

C# port of the RVL algorithm, a lossless compression technique designed for 16-bit depth images.

For full details, please see the [root repository README](../README.md).

## Installation

```bash
dotnet add package rvl
```

## Quick Start

```csharp
using System;
using Rvl;

// Create a sample 16-bit array (e.g., depth data).
short[] data = { 0, 0, 100, 102, 105, 0, 0, 0, 250, 255 };
byte[] rawBytes = new byte[data.Length * 2];
Buffer.BlockCopy(data, 0, rawBytes, 0, rawBytes.Length);

// Compress.
byte[] compressedData = Rvl.Rvl.Compress(rawBytes);

// Decompress.
byte[] decompressedBytes = Rvl.Rvl.Decompress(compressedData);
```