typedef struct {
    int *buffer;
    int *pBuffer;
    int word;
    int nibblesWritten;
} RVLState;

static void EncodeVLE(RVLState* state, int value) {
    do {
        int nibble = value & 0x7; // lower 3 bits
        if (value >>= 3) nibble |= 0x8; // more to come
        state->word <<= 4;
        state->word |= nibble;
        if (++state->nibblesWritten == 8) { // output word
            *state->pBuffer++ = state->word;
            state->nibblesWritten = 0;
            state->word = 0;
        }
    } while (value);
}

static int DecodeVLE(RVLState* state) {
    unsigned int nibble;
    int value = 0, bits = 29;
    do {
        if (!state->nibblesWritten) {
            state->word = *state->pBuffer++; // load word
            state->nibblesWritten = 8;
        }
        nibble = state->word & 0xf0000000;
        value |= (nibble << 1) >> bits;
        state->word <<= 4;
        state->nibblesWritten--;
        bits -= 3;
    } while (nibble & 0x80000000);
    return value;
}

int CompressRVL(short* input, char* output, int numPixels) {
    RVLState state;
    state.buffer = state.pBuffer = (int*)output;
    state.nibblesWritten = 0;
    state.word = 0;
    short *end = input + numPixels;
    short previous = 0;
    while (input != end) {
        int zeros = 0, nonzeros = 0;
        for (; (input != end) && !*input; input++, zeros++);
        EncodeVLE(&state, zeros); // number of zeros
        for (short* p = input; (p != end) && *p++; nonzeros++);
        EncodeVLE(&state, nonzeros); // number of nonzeros
        for (int i = 0; i < nonzeros; i++) {
            short current = *input++;
            int delta = current - previous;
            int positive = (delta << 1) ^ (delta >> 31);
            EncodeVLE(&state, positive); // nonzero value
            previous = current;
        }
    }
    if (state.nibblesWritten) // last few values
        *state.pBuffer++ = state.word << (4 * (8 - state.nibblesWritten));
    return (int)((char*)state.pBuffer - (char*)state.buffer); // num bytes
}

void DecompressRVL(char* input, short* output, int numPixels) {
    RVLState state;
    state.buffer = state.pBuffer = (int*)input;
    state.nibblesWritten = 0;
    state.word = 0;
    short current, previous = 0;
    int numPixelsToDecode = numPixels;
    while (numPixelsToDecode) {
        int zeros = DecodeVLE(&state); // number of zeros
        numPixelsToDecode -= zeros;
        for (; zeros; zeros--)
            *output++ = 0;
        int nonzeros = DecodeVLE(&state); // number of nonzeros
        numPixelsToDecode -= nonzeros;
        for (; nonzeros; nonzeros--) {
            int positive = DecodeVLE(&state); // nonzero value
            int delta = (positive >> 1) ^ -(positive & 1);
            current = previous + delta;
            *output++ = current;
            previous = current;
        }
    }
}