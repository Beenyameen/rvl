# rvl (Python)

[![PyPI version](https://badge.fury.io/py/rvl.svg)](https://badge.fury.io/py/rvl)
[![Supported Python versions](https://img.shields.io/pypi/pyversions/rvl.svg)](https://pypi.org/project/rvl/)

Python bindings for the RVL algorithm, a lossless compression technique designed for 16-bit depth images.

For full details, please see the [root repository README](../README.md).

## Installation

```bash
pip install rvl
```

## Quick Start

```python
import array
import rvl

# Create a sample 16-bit array (e.g., depth data).
data = array.array('h', [0, 0, 100, 102, 105, 0, 0, 0, 250, 255])
raw_bytes = data.tobytes()

# Compress.
compressed_data = rvl.compress(raw_bytes)

# Decompress.
decompressed_bytes = rvl.decompress(compressed_data)
assert decompressed_bytes == raw_bytes
```