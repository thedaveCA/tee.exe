List<StreamWriter> outputWriters = new();

bool appendToFile = false;
bool flushEveryWrite = false;
bool ConsoleToo = true;
bool UserNeedsHelp = false;
bool NoMoreFlags = false;
bool DebugRequested = false;

// If we have no args, assume the user needs help
if (args.Length == 0) {
    UserNeedsHelp = true;
}

try {
    // Open all the output files for writing.
    foreach (string outputFile in args) {
        // If a parameter starts with -, try to parse it
        if (!NoMoreFlags && outputFile.StartsWith('-')) {
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
        const int HelpWidthDescription = 38;

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

        Console.WriteLine("tee (--flag) (--flag...) filename (additionalfile.txt...)");
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
        HelpContents.Add("-f  --flush, --noflush", "Flush to file immediately");
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

        if (DebugRequested) {
            Console.WriteLine("## Notes");
            Console.WriteLine();
            Console.WriteLine("Release targets *.net 6.0 * and is built for *x64 *.For * x86 * I would recommend[unxutils](https://sourceforge.net/projects/unxutils/files/unxutils/current/).");
        }
    } else {
        // Read from standard input and write to standard output and output files.
        string? inputLine;
        while ((inputLine = Console.ReadLine()) != null) {
            if (ConsoleToo) {
                Console.WriteLine(inputLine);
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