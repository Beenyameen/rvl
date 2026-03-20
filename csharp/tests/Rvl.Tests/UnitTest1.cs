using System;
using Xunit;

namespace Rvl.Tests
{
    public class RvlTests
    {
        [Fact]
        public void TestCompressDecompressBasic()
        {
            short[] original = { 0, 0, 0, 1, 2, 3, 0, 0, 4, 5, 0 };
            byte[] originalBytes = new byte[original.Length * 2];
            Buffer.BlockCopy(original, 0, originalBytes, 0, originalBytes.Length);

            byte[] compressed = RvlCodec.Compress(originalBytes);
            Assert.NotNull(compressed);
            Assert.True(compressed.Length > 0);

            byte[] decompressed = RvlCodec.Decompress(compressed);
            Assert.NotNull(decompressed);
            Assert.Equal(originalBytes, decompressed);
        }

        [Fact]
        public void TestCompressDecompressEmpty()
        {
            byte[] original = new byte[0];
            byte[] compressed = RvlCodec.Compress(original);
            byte[] decompressed = RvlCodec.Decompress(compressed);
            Assert.Equal(original, decompressed);
        }

        [Fact]
        public void TestCompressDecompressLargeZeros()
        {
            short[] original = new short[10000];
            byte[] originalBytes = new byte[original.Length * 2];
            Buffer.BlockCopy(original, 0, originalBytes, 0, originalBytes.Length);

            byte[] compressed = RvlCodec.Compress(originalBytes);
            Assert.True(compressed.Length < originalBytes.Length);

            byte[] decompressed = RvlCodec.Decompress(compressed);
            Assert.Equal(originalBytes, decompressed);
        }

        [Fact]
        public void TestCompressDecompressNegativeValues()
        {
            short[] original = { 0, -1, -500, 32767, -32768, 0, 42, -42 };
            byte[] originalBytes = new byte[original.Length * 2];
            Buffer.BlockCopy(original, 0, originalBytes, 0, originalBytes.Length);

            byte[] compressed = RvlCodec.Compress(originalBytes);
            byte[] decompressed = RvlCodec.Decompress(compressed);
            Assert.Equal(originalBytes, decompressed);
        }

        [Fact]
        public void TestCompressDecompressRandomData()
        {
            Random random = new Random(42);
            short[] original = new short[1000];
            for (int i = 0; i < original.Length; i++)
            {
                original[i] = (short)random.Next(-32768, 32768);
            }
            byte[] originalBytes = new byte[original.Length * 2];
            Buffer.BlockCopy(original, 0, originalBytes, 0, originalBytes.Length);

            byte[] compressed = RvlCodec.Compress(originalBytes);
            byte[] decompressed = RvlCodec.Decompress(compressed);
            Assert.Equal(originalBytes, decompressed);
        }

        [Fact]
        public void TestInvalidInput()
        {
            byte[] badInput = { 1, 2, 3 };
            var ex1 = Assert.Throws<ArgumentException>(() => RvlCodec.Compress(badInput));
            Assert.Contains("multiple of 2", ex1.Message);

            var ex2 = Assert.Throws<ArgumentException>(() => RvlCodec.Decompress(badInput));
            Assert.Contains("too short", ex2.Message);
        }

        [Fact]
        public void TestDecompressInvalidPixels()
        {
            byte[] badHeader = { 0xff, 0xff, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00 };
            var ex = Assert.Throws<ArgumentException>(() => RvlCodec.Decompress(badHeader));
            Assert.Contains("Invalid number of pixels", ex.Message);
        }
    }
}