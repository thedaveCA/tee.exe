# tee

Redumentary implementation of the [`tee`](https://en.wikipedia.org/wiki/Tee_(command)) command, reading standard input and writing it to standard output in real-time as well as writing to one or more files.

## Usage

```
tee.exe <output_file> [additional_output_files...]
```

## Notes

Release targets *.net 6.0* and is built for *x64*. For *x86* I would recommend [unxutils](https://sourceforge.net/projects/unxutils/files/unxutils/current/).