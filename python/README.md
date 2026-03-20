# rvl (Python)

Python bindings for the RVL algorithm, a lossless compression technique designed for 16-bit depth images.

For full details, see the [root repository README](https://github.com/Beenyameen/rvl/blob/main/README.md).

## Installation

```bash
pip install rvl
```

## Example

```python
import array
import rvl

# Create a sample 16-bit array (e.g., depth data).
data = array.array("h", [0, 0, 100, 102, 105, 0, 0, 0, 250, 255])

# Compress.
compressed_data = rvl.compress(data)

# Decompress.
decompressed_data = rvl.decompress(compressed_data)
```