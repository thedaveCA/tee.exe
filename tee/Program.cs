List<StreamWriter> outputWriters = new();

bool appendToFile = false;
bool flushEveryWrite = false;
bool ConsoleToo = true;
bool UserNeedsHelp = false;
bool NoMoreFlags = false;
bool DebugRequested = false;
bool NextParamIsErr = false;
bool ReportWatchInRed = false;
List<string> ErrorStrings = new();
bool FoundErr = false;

// If we have no args, assume the user needs help
if (args.Length == 0) {
    UserNeedsHelp = true;
}

try {
    // Open all the output files for writing.
    foreach (string outputFile in args) {
        // If a parameter starts with -, try to parse it
        if (NextParamIsErr) {
            ErrorStrings.Add(outputFile);
            if (DebugRequested) {
                Console.WriteLine($"DEBUG: Adding WATCH: {outputFile}");
            }
            NextParamIsErr = false;
        } else if (!NoMoreFlags && outputFile.StartsWith('-')) {
            if (DebugRequested) {
                Console.WriteLine($"DEBUG: Parsing flag: {outputFile}");
            }

            switch (outputFile.ToLowerInvariant()) {
                case "-":
                    // From now on everything will be treated as a file, such that "- --help" writes
                    // a file called "help"
                    NoMoreFlags = true;
                    break;
                case "--debug":
                    Console.WriteLine($"DEBUG: Parsing flag: {outputFile}");
                    DebugRequested = true;
                    break;
                case "-a":
                case "--append":
                    appendToFile = true;
                    break;
                case "--noappend":
                    appendToFile = false;
                    ;
                    break;
                case "-f":
                case "--flush":
                    flushEveryWrite = true;
                    break;
                case "--noflush":
                    flushEveryWrite = true;
                    break;
                case "--noconsole":
                    ConsoleToo = false;
                    break;
                case "-w":
                case "--watch":
                    NextParamIsErr = true;
                    break;
                case "--watchred":
                    ReportWatchInRed = true;
                    break;
                default:
                    // Anything else starting with a - triggers help, ERROR should not be displayed
                    // for actual help requests
                    if (outputFile is not ("-h" or "--help")) {
                        Console.WriteLine($"ERROR: `{outputFile}` not recognized.");
                    }
                    UserNeedsHelp = true;
                    break;
            }
        } else {
            // Whatever is left is (hopefully) a filename
            if (DebugRequested) {
                Console.WriteLine($"DEBUG: Parsing file: {outputFile}");
            }

            StreamWriter writer = new(outputFile, append: appendToFile) {
                AutoFlush = flushEveryWrite
            };
            outputWriters.Add(writer);
        }
    }
    if (UserNeedsHelp) {
        const int HelpWidthFlag = 28;
        const int HelpWidthDescription = 46;

        if (DebugRequested) {
            Console.WriteLine("# TEE");
        }
        Console.WriteLine();
        Console.WriteLine("Redumentary implementation of the `tee` command.");
        Console.WriteLine("Read standand input, write to console plus one or more files.");
        if (DebugRequested) {
            Console.WriteLine("Read more on the Wikipedia [tee article](https://en.wikipedia.org/wiki/Tee_(command)).");
        } else {
            Console.WriteLine("For more information visit https://github.com/thedaveCA/tee.exe#readme");
        }

        Console.WriteLine();
        Console.WriteLine("## USAGE");
        Console.WriteLine("");
        if (DebugRequested) {
            Console.WriteLine("```");
        }

        Console.WriteLine("externalcommand 2>&1 | tee [--flags...] output_files...");
        if (DebugRequested) {
            Console.WriteLine("```");
        }

        Console.WriteLine();

        Dictionary<string, string> HelpContents = new();
        if (DebugRequested) {
            HelpContents.Add("Flag ", "Description");
            HelpContents.Add(new string('-', HelpWidthFlag), new string('-', HelpWidthDescription));
        }
        HelpContents.Add("-a  --append, --noappend", "Append to file");
        HelpContents.Add("-f  --flush, --noflush", "Flush file to disk immediately");
        HelpContents.Add("-w  --watch \"string\"", "Add a string to the watch list");
        HelpContents.Add("    --watchred", "Display lines containing a watch string in red");
        HelpContents.Add("    --noconsole", "Suppress writing to console");
        HelpContents.Add("-h  --help", "Get help for commands");
        HelpContents.Add("-", "All remaining parameters are filenames");

        foreach (KeyValuePair<string, string> HelpItem in HelpContents) {
            if (DebugRequested) {
                Console.WriteLine($" | {HelpItem.Key,-HelpWidthFlag} | {HelpItem.Value,-HelpWidthDescription} |");
            } else {
                Console.WriteLine($" {HelpItem.Key,-HelpWidthFlag} {HelpItem.Value}");
            }
        }
        Console.WriteLine();
        Console.WriteLine("TEE parses the output looking for watch strings, and if found will return errorlevel 1.");
        Console.WriteLine();

        if (DebugRequested) {
            // When --debug --help are used together the help is in markdown format for use in the repository's README.md
            Console.WriteLine("## Notes");
            Console.WriteLine();
            Console.WriteLine("Release targets *.net 7.0* and is built for Windows 10 and newer, *x64* only. For older versions of Windows or *x86* I would recommend [unxutils](https://sourceforge.net/projects/unxutils/files/unxutils/current/).");
            Console.WriteLine();
        }
        Console.WriteLine("`--append` and `--flush` apply to all subsequent files.");
        Console.WriteLine();
        Console.WriteLine("In Windows it is not possible for TEE to observe the errorlevel of the piped process, and only TEE's errorlevel can be returned, therefore you may want to watch for a string and allow another application to act upon it.");
        Console.WriteLine();
        Console.WriteLine("Adding `2>&1` before the pipe (`|`) redirects errors as well as normal output.");
        Console.WriteLine();
    } else {
        // Read from standard input and write to standard output and output files.
        string? inputLine;
        ConsoleColor? ConsoleOriginalColor = null;
        while ((inputLine = Console.ReadLine()) != null) {
            foreach (string ErrorString in ErrorStrings) {
                if (inputLine.Contains(ErrorString)) {
                    FoundErr = true;
                    if (ReportWatchInRed && ConsoleToo) {
                        ConsoleOriginalColor ??= Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                    }
                }
            }
            if (ConsoleToo) {
                Console.WriteLine(inputLine);
                // Once we've found an error, set the color back
                if (FoundErr && ReportWatchInRed) { Console.ForegroundColor = ConsoleOriginalColor ?? ConsoleColor.White; }
            }
            foreach (StreamWriter writer in outputWriters) {
                writer.WriteLine(inputLine);
            }
        }
    }
} catch (Exception ex) {
    Console.Error.WriteLine("Error: " + ex.Message);
} finally {
    // Close all the output files.
    foreach (StreamWriter writer in outputWriters) {
        writer.Close();
    }
}

if (DebugRequested && !UserNeedsHelp) {
    if (FoundErr) {
        Console.WriteLine(Environment.NewLine+"DEBUG: Found watch string, returning errorlevel 1");
    } else {
        Console.WriteLine(Environment.NewLine + "DEBUG: Watch string was not found.");
    }
}

return FoundErr ? 1 : 0;
