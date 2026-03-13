import array
import random
import pytest
import rvl

def test_compress_decompress_basic():
    original = array.array('h', [0, 0, 0, 1, 2, 3, 0, 0, 4, 5, 0])
    original_bytes = original.tobytes()
    compressed = rvl.compress(original_bytes)
    assert isinstance(compressed, bytes)
    assert len(compressed) > 0
    decompressed = rvl.decompress(compressed)
    assert isinstance(decompressed, bytes)
    assert decompressed == original_bytes

def test_compress_decompress_empty():
    original = b""
    compressed = rvl.compress(original)
    decompressed = rvl.decompress(compressed)
    assert decompressed == original

def test_compress_decompress_large_zeros():
    original = array.array('h', [0] * 10000).tobytes()
    compressed = rvl.compress(original)
    assert len(compressed) < len(original)
    decompressed = rvl.decompress(compressed)
    assert decompressed == original

def test_compress_decompress_negative_values():
    original = array.array('h', [0, -1, -500, 32767, -32768, 0, 42, -42])
    original_bytes = original.tobytes()
    compressed = rvl.compress(original_bytes)
    decompressed = rvl.decompress(compressed)
    assert decompressed == original_bytes

def test_compress_bytearray():
    original = array.array('h', [10, 20, 30, 40, 50])
    original_bytearray = bytearray(original.tobytes())
    compressed = rvl.compress(original_bytearray)
    decompressed = rvl.decompress(compressed)
    assert decompressed == original.tobytes()

def test_compress_decompress_random_data():
    random.seed(42)
    original = array.array('h', [random.randint(-32768, 32767) for _ in range(1000)])
    original_bytes = original.tobytes()
    compressed = rvl.compress(original_bytes)
    decompressed = rvl.decompress(compressed)
    assert decompressed == original_bytes

def test_invalid_input():
    with pytest.raises(ValueError, match="multiple of 2 bytes"):
        rvl.compress(b"123")
    with pytest.raises(ValueError, match="too short"):
        rvl.decompress(b"123")

def test_decompress_invalid_pixels():
    bad_header = b"\xff\xff\xff\xff" + b"\x00\x00\x00\x00"
    with pytest.raises(ValueError, match="Invalid number of pixels."):
        rvl.decompress(bad_header)

def test_memoryview():
    original = array.array('h', [1, 2, 3, 4, 5])
    view = memoryview(original)
    compressed = rvl.compress(view)
    decompressed = rvl.decompress(compressed)
    assert decompressed == original.tobytes()

def test_unaligned_buffer():
    original = array.array('h', [10, 20, 30, 40, 50, 60]).tobytes()
    padded = b"X" + original
    view = memoryview(padded)[1:]
    compressed = rvl.compress(view)
    decompressed = rvl.decompress(compressed)
    assert decompressed == original
