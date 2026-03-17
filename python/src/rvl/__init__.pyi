from typing import Union

_BufferLike = Union[bytes, bytearray, memoryview]

def compress(data: _BufferLike) -> bytes:
    """
    Compress a 16-bit depth image array.
    
    Args:
        data: The raw 16-bit depth data (bytes, bytearray, or memoryview) to compress.
            The length must be a multiple of 2 bytes.
            
    Returns:
        The RVL compressed image data.
    """
    ...

def decompress(data: _BufferLike) -> bytes:
    """
    Decompress an RVL compressed image.
    
    Args:
        data: The RVL compressed bytes (bytes, bytearray, or memoryview).
        
    Returns:
        The decompressed 16-bit depth data as bytes.
    """
    ...
