if (args.Length < 1) {
    Console.WriteLine("Usage: tee.exe <output_file> [additional_output_files...]");
    return;
}

List<StreamWriter> outputWriters = new();

try {
    // Open all the output files for writing.
    foreach (string outputFile in args) {
        StreamWriter writer = new(outputFile,append:true);
        outputWriters.Add(writer);
    }

    // Read from standard input and write to standard output and output files.
    string? inputLine;
    while ((inputLine = Console.ReadLine()) != null) {
        Console.WriteLine(inputLine);
        foreach (StreamWriter writer in outputWriters) {
            writer.WriteLine(inputLine);
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