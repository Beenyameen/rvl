# rvl

[![PyPI version](https://badge.fury.io/py/rvl.svg)](https://badge.fury.io/py/rvl)
[![NuGet version](https://badge.fury.io/nu/rvl.svg)](https://badge.fury.io/nu/rvl)
[![Build Status](https://github.com/Beenyameen/rvl/actions/workflows/build_and_publish.yml/badge.svg)](https://github.com/Beenyameen/rvl/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

A monorepo containing fast implementations of the **RVL (Run-Length VLE) algorithm**, a lossless compression technique specifically designed for 16-bit depth images.

### Supported Languages

* [🐍 Python (`rvl` on PyPI)](./python/README.md)
* [🎯 C# / .NET (`rvl` on NuGet)](./csharp/README.md)

## Quick Installation

**Python:**
```bash
pip install rvl
```

**C# / .NET:**
```bash
dotnet add package rvl
```

## Credit

This repository relies on the core RVL compression algorithm developed by Andrew D. Wilson. For details on the algorithm's mechanics and performance, refer to the [original paper](https://www.microsoft.com/en-us/research/publication/fast-lossless-depth-image-compression/).