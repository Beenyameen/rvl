# rvl

[![PyPI version](https://badge.fury.io/py/rvl.svg)](https://badge.fury.io/py/rvl)
[![Supported Python versions](https://img.shields.io/pypi/pyversions/rvl.svg)](https://pypi.org/project/rvl/)
[![Build Status](https://github.com/Beenyameen/rvl/actions/workflows/build_and_publish.yml/badge.svg)](https://github.com/Beenyameen/rvl/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Python bindings for the RVL algorithm, a lossless compression technique designed for 16-bit depth images.

## Installation

You can install `rvl` directly from PyPI:

```bash
pip install rvl
```

*Pre-built wheels are available for Windows, macOS, and Linux (including 64-bit ARM devices). Older Python versions (3.2-3.5) will automatically compile from the source distribution.*

## Quick Start

`rvl` can compress and decompress raw bytes, bytearrays, or memoryviews. This works with 16-bit depth data from arrays or libraries like `numpy`.

```python
import array
import rvl

# Create a sample 16-bit array (e.g., depth data).
# 'h' represents signed 16-bit integers.
data = array.array('h', [0, 0, 100, 102, 105, 0, 0, 0, 250, 255])
raw_bytes = data.tobytes()

# Compress.
compressed_data = rvl.compress(raw_bytes)
print(f"Original size: {len(raw_bytes)} bytes")
print(f"Compressed size: {len(compressed_data)} bytes")

# Decompress.
decompressed_bytes = rvl.decompress(compressed_data)

# Verify.
assert decompressed_bytes == raw_bytes
```

## Credit

This Python wrapper relies on the core RVL compression algorithm developed by Andrew D. Wilson. For details on the algorithm's mechanics and performance, refer to the [original paper](https://www.microsoft.com/en-us/research/publication/fast-lossless-depth-image-compression/).
