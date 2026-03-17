#define PY_SSIZE_T_CLEAN
#include <Python.h>
#include <stdint.h>
#include <string.h>

int CompressRVL(short* input, char* output, int numPixels);
void DecompressRVL(char* input, short* output, int numPixels);

static PyObject* rvl_compress(PyObject* self, PyObject* args) {
    Py_buffer view;
    if (!PyArg_ParseTuple(args, "y*", &view)) {
        return NULL;
    }

    if (view.len % 2 != 0) {
        PyBuffer_Release(&view);
        PyErr_SetString(PyExc_ValueError, "Input buffer length must be a multiple of 2 bytes (16-bit pixels).");
        return NULL;
    }

    int numPixels = (int)(view.len / 2);

    size_t out_max_size = 4 + (size_t)numPixels * 3 + 1024;
    char* output = (char*)PyMem_Malloc(out_max_size);
    if (!output) {
        PyBuffer_Release(&view);
        return PyErr_NoMemory();
    }

    output[0] = (numPixels >> 0) & 0xFF;
    output[1] = (numPixels >> 8) & 0xFF;
    output[2] = (numPixels >> 16) & 0xFF;
    output[3] = (numPixels >> 24) & 0xFF;

    int compressed_size = 0;

    if (((uintptr_t)view.buf) % 2 != 0) {
        short* aligned_input = (short*)PyMem_Malloc(view.len);
        if (!aligned_input) {
            PyMem_Free(output);
            PyBuffer_Release(&view);
            return PyErr_NoMemory();
        }
        memcpy(aligned_input, view.buf, view.len);
        Py_BEGIN_ALLOW_THREADS
        compressed_size = CompressRVL(aligned_input, output + 4, numPixels);
        Py_END_ALLOW_THREADS
        PyMem_Free(aligned_input);
    } else {
        Py_BEGIN_ALLOW_THREADS
        compressed_size = CompressRVL((short*)view.buf, output + 4, numPixels);
        Py_END_ALLOW_THREADS
    }

    PyBuffer_Release(&view);

    PyObject* result = PyBytes_FromStringAndSize(output, 4 + compressed_size);
    PyMem_Free(output);

    return result;
}

static PyObject* rvl_decompress(PyObject* self, PyObject* args) {
    Py_buffer view;
    if (!PyArg_ParseTuple(args, "y*", &view)) {
        return NULL;
    }

    if (view.len < 4) {
        PyBuffer_Release(&view);
        PyErr_SetString(PyExc_ValueError, "Compressed data too short.");
        return NULL;
    }

    char* input = (char*)view.buf;
    int numPixels = ((unsigned char)input[0]) | 
                    (((unsigned char)input[1]) << 8) | 
                    (((unsigned char)input[2]) << 16) | 
                    (((unsigned char)input[3]) << 24);

    if (numPixels < 0) {
        PyBuffer_Release(&view);
        PyErr_SetString(PyExc_ValueError, "Invalid number of pixels.");
        return NULL;
    }

    size_t out_size = (size_t)numPixels * 2;
    short* output = (short*)PyMem_Malloc(out_size);
    if (!output && out_size > 0) {
        PyBuffer_Release(&view);
        return PyErr_NoMemory();
    }

    if (((uintptr_t)(input + 4)) % 4 != 0) {
        char* aligned_input = (char*)PyMem_Malloc(view.len - 4);
        if (!aligned_input) {
            if (output) PyMem_Free(output);
            PyBuffer_Release(&view);
            return PyErr_NoMemory();
        }
        memcpy(aligned_input, input + 4, view.len - 4);
        Py_BEGIN_ALLOW_THREADS
        DecompressRVL(aligned_input, output, numPixels);
        Py_END_ALLOW_THREADS
        PyMem_Free(aligned_input);
    } else {
        Py_BEGIN_ALLOW_THREADS
        DecompressRVL(input + 4, output, numPixels);
        Py_END_ALLOW_THREADS
    }

    PyBuffer_Release(&view);

    PyObject* result = PyBytes_FromStringAndSize((char*)output, out_size);
    if (output) PyMem_Free(output);

    return result;
}

PyDoc_STRVAR(compress_doc,
"compress(data, /)\n"
"--\n\n"
"Compress a 16-bit depth image array.\n\n"
"Args:\n"
"    data (bytes | bytearray | memoryview): The raw 16-bit depth data to compress.\n"
"        The length must be a multiple of 2 bytes.\n\n"
"Returns:\n"
"    bytes: The RVL compressed image data.");

PyDoc_STRVAR(decompress_doc,
"decompress(data, /)\n"
"--\n\n"
"Decompress an RVL compressed image.\n\n"
"Args:\n"
"    data (bytes | bytearray | memoryview): The RVL compressed bytes.\n\n"
"Returns:\n"
"    bytes: The decompressed 16-bit depth data.");

static PyMethodDef RvlMethods[] = {
    {"compress", rvl_compress, METH_VARARGS, compress_doc},
    {"decompress", rvl_decompress, METH_VARARGS, decompress_doc},
    {NULL, NULL, 0, NULL}
};

static struct PyModuleDef rvlmodule = {
    PyModuleDef_HEAD_INIT,
    "_rvl",
    "Internal C extension for RVL compression.",
    -1,
    RvlMethods
};

PyMODINIT_FUNC PyInit__rvl(void) {
    return PyModule_Create(&rvlmodule);
}
