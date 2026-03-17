using System;

namespace Rvl
{
    public static class Rvl
    {
        public static byte[] Compress(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length % 2 != 0)
                throw new ArgumentException("Input buffer length must be a multiple of 2 bytes (16-bit pixels).");

            int numPixels = data.Length / 2;

            // Header (4 bytes) + roughly worst-case scenario
            int outMaxSize = 4 + numPixels * 3 + 1024;
            byte[] output = new byte[outMaxSize];

            output[0] = (byte)(numPixels & 0xFF);
            output[1] = (byte)((numPixels >> 8) & 0xFF);
            output[2] = (byte)((numPixels >> 16) & 0xFF);
            output[3] = (byte)((numPixels >> 24) & 0xFF);

            int compressedSize;

            unsafe
            {
                fixed (byte* pIn = data)
                fixed (byte* pOut = output)
                {
                    short* input = (short*)pIn;
                    byte* outBuf = pOut + 4;
                    compressedSize = CompressRVL(input, outBuf, numPixels);
                }
            }

            byte[] result = new byte[4 + compressedSize];
            Array.Copy(output, result, result.Length);
            return result;
        }

        public static byte[] Decompress(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Length < 4)
                throw new ArgumentException("Compressed data too short.");

            int numPixels = data[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24);
            if (numPixels < 0)
                throw new ArgumentException("Invalid number of pixels.");

            byte[] output = new byte[numPixels * 2];

            unsafe
            {
                fixed (byte* pIn = data)
                fixed (byte* pOut = output)
                {
                    byte* input = pIn + 4;
                    short* outBuf = (short*)pOut;
                    DecompressRVL(input, outBuf, numPixels);
                }
            }

            return output;
        }

        private unsafe static int CompressRVL(short* input, byte* output, int numPixels)
        {
            int* buffer = (int*)output;
            int* pBuffer = buffer;
            int nibblesWritten = 0;
            int word = 0;
            short* end = input + numPixels;
            short previous = 0;

            while (input < end)
            {
                int zeros = 0, nonzeros = 0;
                for (; (input < end) && *input == 0; input++, zeros++) ;
                EncodeVLE(ref pBuffer, ref word, ref nibblesWritten, zeros);

                for (short* p = input; (p < end) && *p != 0; p++, nonzeros++) ;
                EncodeVLE(ref pBuffer, ref word, ref nibblesWritten, nonzeros);

                for (int i = 0; i < nonzeros; i++)
                {
                    short current = *input++;
                    int delta = current - previous;
                    int positive = (delta << 1) ^ (delta >> 31);
                    EncodeVLE(ref pBuffer, ref word, ref nibblesWritten, positive);
                    previous = current;
                }
            }
            if (nibblesWritten != 0)
            {
                *pBuffer++ = word << (4 * (8 - nibblesWritten));
            }
            return (int)((byte*)pBuffer - (byte*)buffer);
        }

        private unsafe static void EncodeVLE(ref int* pBuffer, ref int word, ref int nibblesWritten, int value)
        {
            do
            {
                int nibble = value & 0x7;
                if ((value >>= 3) != 0) nibble |= 0x8;
                word <<= 4;
                word |= nibble;
                if (++nibblesWritten == 8)
                {
                    *pBuffer++ = word;
                    nibblesWritten = 0;
                    word = 0;
                }
            } while (value != 0);
        }

        private unsafe static void DecompressRVL(byte* input, short* output, int numPixels)
        {
            int* buffer = (int*)input;
            int* pBuffer = buffer;
            int nibblesWritten = 0;
            int word = 0;
            short current, previous = 0;
            int numPixelsToDecode = numPixels;

            while (numPixelsToDecode > 0)
            {
                int zeros = DecodeVLE(ref pBuffer, ref word, ref nibblesWritten);
                numPixelsToDecode -= zeros;
                for (; zeros > 0; zeros--)
                    *output++ = 0;

                int nonzeros = DecodeVLE(ref pBuffer, ref word, ref nibblesWritten);
                numPixelsToDecode -= nonzeros;
                for (; nonzeros > 0; nonzeros--)
                {
                    int positive = DecodeVLE(ref pBuffer, ref word, ref nibblesWritten);
                    int delta = (positive >> 1) ^ -(positive & 1);
                    current = (short)(previous + delta);
                    *output++ = current;
                    previous = current;
                }
            }
        }

        private unsafe static int DecodeVLE(ref int* pBuffer, ref int word, ref int nibblesWritten)
        {
            uint nibble;
            int value = 0, bits = 29;
            do
            {
                if (nibblesWritten == 0)
                {
                    word = *pBuffer++;
                    nibblesWritten = 8;
                }
                nibble = (uint)word & 0xf0000000;
                value |= (int)((nibble << 1) >> bits);
                word <<= 4;
                nibblesWritten--;
                bits -= 3;
            } while ((nibble & 0x80000000) != 0);
            return value;
        }
    }
}