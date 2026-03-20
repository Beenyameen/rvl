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

            byte[] compressed = RvlCodec.Compress(original);
            Assert.NotNull(compressed);
            Assert.True(compressed.Length > 0);

            short[] decompressed = RvlCodec.Decompress(compressed);
            Assert.Equal(original, decompressed);
        }

        [Fact]
        public void TestCompressDecompressEmpty()
        {
            short[] original = Array.Empty<short>();
            byte[] compressed = RvlCodec.Compress(original);
            short[] decompressed = RvlCodec.Decompress(compressed);
            Assert.Equal(original, decompressed);
        }

        [Fact]
        public void TestCompressDecompressLargeZeros()
        {
            short[] original = new short[10000];

            byte[] compressed = RvlCodec.Compress(original);
            Assert.True(compressed.Length < original.Length * 2);

            short[] decompressed = RvlCodec.Decompress(compressed);
            Assert.Equal(original, decompressed);
        }

        [Fact]
        public void TestCompressDecompressNegativeValues()
        {
            short[] original = { 0, -1, -500, 32767, -32768, 0, 42, -42 };

            byte[] compressed = RvlCodec.Compress(original);
            short[] decompressed = RvlCodec.Decompress(compressed);
            Assert.Equal(original, decompressed);
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

            byte[] compressed = RvlCodec.Compress(original);
            short[] decompressed = RvlCodec.Decompress(compressed);
            Assert.Equal(original, decompressed);
        }

        [Fact]
        public void TestInvalidInput()
        {
            byte[] badInput = { 1, 2, 3 };
            var ex = Assert.Throws<ArgumentException>(() => RvlCodec.Decompress(badInput));
            Assert.Contains("too short", ex.Message);
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
