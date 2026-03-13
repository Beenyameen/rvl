import os
from setuptools import setup, Extension

here = os.path.abspath(os.path.dirname(__file__))

rvl_ext = Extension(
    "rvl._rvl",
    sources=["src/rvl/rvl.c", "src/rvl/_rvl.c"],
    include_dirs=["src/rvl"],
)

with open(os.path.join(here, "README.md"), encoding="utf-8") as f:
    long_description = f.read()

setup(
    name="rvl",
    version="1.0.2",
    description="Python bindings for the RVL lossless compression algorithm.",
    long_description=long_description,
    long_description_content_type="text/markdown",
    url="https://github.com/Beenyameen/rvl",
    author="Beenyameen",
    classifiers=[
        "Development Status :: 5 - Production/Stable",
        "Intended Audience :: Developers",
        "License :: OSI Approved :: MIT License",
        "Programming Language :: Python :: 3",
        "Programming Language :: Python :: 3.2",
        "Programming Language :: Python :: 3.3",
        "Programming Language :: Python :: 3.4",
        "Programming Language :: Python :: 3.5",
        "Programming Language :: Python :: 3.6",
        "Programming Language :: Python :: 3.7",
        "Programming Language :: Python :: 3.8",
        "Programming Language :: Python :: 3.9",
        "Programming Language :: Python :: 3.10",
        "Programming Language :: Python :: 3.11",
        "Programming Language :: Python :: 3.12",
        "Programming Language :: Python :: 3.13",
        "Programming Language :: Python :: Implementation :: CPython",
    ],
    keywords="compression, rvl, image, 16-bit, depth, lossless",
    package_dir={"": "src"},
    packages=["rvl"],
    package_data={"rvl": ["py.typed", "*.pyi"]},
    ext_modules=[rvl_ext],
    python_requires=">=3.2",
)
