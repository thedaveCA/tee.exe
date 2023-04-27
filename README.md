# TEE

Redumentary implementation of the `tee` command.
Read standand input, write to console plus one or more files.
Read more on the Wikipedia [tee article](https://en.wikipedia.org/wiki/Tee_(command)).

## USAGE

```
tee (--flag) (--flag...) filename (additionalfile.txt...)
```

 | Flag                         | Description                            |
 | ---------------------------- | -------------------------------------- |
 | -a  --append, --noappend     | Append to file                         |
 | -f  --flush, --noflush       | Flush to file immediately              |
 |     --noconsole              | Suppress writing to console            |
 | -h  --help                   | Get help for commands                  |
 | -                            | All remaining parameters are filenames |

## Notes

Release targets *.net 6.0 * and is built for *x64 *.For * x86 * I would recommend[unxutils](https://sourceforge.net/projects/unxutils/files/unxutils/current/).
