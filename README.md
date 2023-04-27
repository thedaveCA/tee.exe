# TEE

Redumentary implementation of the `tee` command.
Read standand input, write to console plus one or more files.
Read more on the Wikipedia [tee article](https://en.wikipedia.org/wiki/Tee_(command)).

## USAGE

```
externalcommand 2>&1 | tee [--flags...] output_files...
```

 | Flag                         | Description                                    |
 | ---------------------------- | ---------------------------------------------- |
 | -a  --append, --noappend     | Append to file                                 |
 | -f  --flush, --noflush       | Flush file to disk immediately                 |
 | -w  --watch "string"         | Add a string to the watch list                 |
 |     --watchred               | Display lines containing a watch string in red |
 |     --noconsole              | Suppress writing to console                    |
 | -h  --help                   | Get help for commands                          |
 | -                            | All remaining parameters are filenames         |

TEE parses the output looking for watch strings, and if found will return errorlevel 1.

## Notes

Release targets *.net 7.0* and is built for Windows 10 and newer, *x64* only. For older versions of Windows or *x86* I would recommend [unxutils](https://sourceforge.net/projects/unxutils/files/unxutils/current/).

`--append` and `--flush` apply to all subsequent files.

In Windows it is not possible for TEE to observe the errorlevel of the piped process, and only TEE's errorlevel can be returned, therefore you may want to watch for a string and allow another application to act upon it.

Adding `2>&1` before the pipe (`|`) redirects errors as well as normal output.