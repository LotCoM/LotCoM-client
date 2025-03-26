using LotCoMClient.Models.Exceptions;

namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Provides controlled access and manipulation of database tables in the LotCoM Database.
/// </summary>
public class DataTable {
    /// <summary>
    /// The Path of the database table file in the LotCoM database filing system.
    /// </summary>
    private readonly string _path = "";
    /// <summary>
    /// The type of Data Record the table file contains (Prints || Scans).
    /// </summary>
    private readonly Type _recordType;
    /// <summary>
    /// Holds the currently-read Data Records in the DataTable.
    /// </summary>
    private List<DataRecord> _records = [];

    /// <summary>
    /// Parses a DataRecord of the DataTable's _recordType from CSVLine.
    /// </summary>
    /// <param name="CSVLine"></param>
    /// <exception cref="RecordParseException"></exception>
    /// <returns>A DataRecord object.</returns>
    private DataRecord ParseRecord(string CSVLine) {
        // attempt to parse the proper type of DataRecord from the CSV Line
        DataRecord ParsedRecord;
        // parse a PrintRecord
        if (_recordType.Equals(typeof(PrintRecord))) {
            ParsedRecord = PrintRecord.ParseFromCSV(CSVLine);
        // parse a ScanRecord
        } else {
            ParsedRecord = PrintRecord.ParseFromCSV(CSVLine);
        }
        // return the parsed DataRecord
        return ParsedRecord;
    }

    /// <summary>
    /// Constructs a new DataTable that provides controlled access and manipulation of data in the Database Table located at DataTablePath.
    /// </summary>
    /// <param name="DataTablePath">A full file path to a Database Table file in the LotCoM database.</param>
    public DataTable(string DataTablePath) {
        _path = DataTablePath;
        // calculate the record type from the path string
        if (_path.Contains("data_tables\\prints")) {
            _recordType = typeof(PrintRecord);
        } else if (_path.Contains("data_tables\\scans")) {
            _recordType = typeof(ScanRecord);
        // the path passed isn't a valid Database Table path; throw an exception
        } else {
            throw new ArgumentException($"Could not create a DataTable object from the file at {_path}.");
        }
        // read the database table and populate runtime
        _records = Read();
    }

    /// <summary>
    /// Opens, reads, and formats the text in DataTable._path as a list of DataRecords.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="System.Security.SecurityException"></exception>
    /// <exception cref="RecordParseException"></exception>
    /// <returns>A List of DataRecords.</returns>
    private List<DataRecord> Read() {
        // read the Database Table at the _path property
        string Text = File.ReadAllText(_path);
        // separate the read text into record lines (split by newline character)
        List<string> RecordLines = Text.Split("\n").ToList();
        // remove any empty lines
        RecordLines = RecordLines.Where(x => !x.Equals("")).ToList();
        // parse each line into a DataRecord
        List<DataRecord> ParsedRecords = [];
        foreach (string _line in RecordLines) {
            // use the ParseRecord method to parse the correct Record type
            DataRecord _parsedRecord;
            try {
                _parsedRecord = ParseRecord(_line);
            // one line could not be parsed; throw an exception
            } catch {
                throw new RecordParseException();
            }
            // add the parsed DataRecord object to the Record List
            ParsedRecords.Add(_parsedRecord);
        }
        // return the list of parsed DataRecords
        return ParsedRecords;
    }

    /// <summary>
    /// Asynchronously opens, reads, and formats the text in DataTable._path as a list of DataRecords.
    /// </summary>
    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="RecordParseException"></exception>
    /// <returns>A List of DataRecords.</returns>
    private async Task<List<DataRecord>> ReadAsync() {
        // read the Database Table as the _path property asynchronously
        string Text = await File.ReadAllTextAsync(_path);
        // format the Text as DataRecords on a new CPU thread
        List<string> RecordLines = await Task.Run(() => {
            List<string> Split = Text.Split("\n").ToList();
            // remove any empty lines
            Split = Split.Where(x => !x.Equals("")).ToList();
            return Split;
        });
        // parse the lines into DataRecords on a new CPU thread
        List<DataRecord> ParsedRecords = await Task.Run(() => {
            List<DataRecord> Records = [];
            foreach (string _line in RecordLines) {
                // use the ParseRecord method to parse the correct Record type
                DataRecord _parsedRecord;
                try {
                    _parsedRecord = ParseRecord(_line);
                // one line could not be parsed; throw an exception
                } catch {
                    throw new RecordParseException();
                }
                // add the parsed DataRecord object to the Record List
                Records.Add(_parsedRecord);
            }
            return Records;
        });
        return ParsedRecords;
    }

    /// <summary>
    /// Opens and overwrites the data in DataTable._path with the current list of DataRecords in DataTable._records.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="System.Security.SecurityException"></exception>
    private void Save() {
        // format the DataRecords in _records as single string separated by newlines
        string Text = "";
        foreach (DataRecord _record in _records) {
            Text = $"{Text}{_record.ToCSV()}\n";
        }
        // write the single string as text to the Database Table file at _path
        File.WriteAllText(_path, Text);
    }

    /// <summary>
    /// Asynchronously opens and overwrites the data in DataTable._path with the current list of DataRecords in DataTable._records.
    /// </summary>
    /// <exception cref="OperationCanceledException"></exception>
    /// <returns>A List of DataRecords.</returns>
    private async Task SaveAsync() {
        // format the DataRecords in _records as single string separated by newlines on a new CPU thread
        string Text = await Task.Run(() => {
            string Formatted = "";
            foreach (DataRecord _record in _records) {
                Formatted = $"{Formatted}{_record.ToCSV()}\n";
            }
            // return the formatted single string
            return Formatted;
        });
        // asynchronously write the single string as text to the Database Table file at _path
        await File.WriteAllTextAsync(_path, Text);
    }

    /// <summary>
    /// Updates the DataRecords currently stored in DataTable._records and returns the list held by the property.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="System.Security.SecurityException"></exception>
    /// <returns>A List of DataRecords.</returns>
    public List<DataRecord> GetRecords() {
        // update the DataRecords in runtime
        _records = Read();
        // return the DataRecords stored in runtime
        return _records;
    }

    /// <summary>
    /// Asynchronously updates the DataRecords currently stored in DataTable._records and returns the list held by the property.
    /// </summary>
    /// <exception cref="OperationCanceledException"></exception>
    /// <returns>A List of DataRecords.</returns>
    public async Task<List<DataRecord>> GetRecordsAsync() {
        // update the DataRecords in runtime
        _records = await ReadAsync();
        // return the DataRecords stored in runtime
        return _records;
    }

    /// <summary>
    /// Saves DataRecords to DataTable._path and updates DataTable._records in runtime.
    /// </summary>
    /// <param name="Records">A List of DataRecords to write to DataTable._path.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="System.Security.SecurityException"></exception>
    public void SaveRecords(List<DataRecord> Records) {
        // update the _records property
        _records = Records;
        // save the DataTable
        Save();
        // update the _records property to the post-save entries list
        _records = GetRecords();
    }

    /// <summary>
    /// Asynchronously saves DataRecords to DataTable._path and updates DataTable._records in runtime.
    /// </summary>
    /// <param name="Records">A List of DataRecords to write to DataTable._path.</param>
    /// <exception cref="OperationCanceledException"></exception>
    /// <returns></returns>
    public async Task SaveRecordsAsync(List<DataRecord> Records) {
        // update the _records property
        _records = Records;
        // save the DataTable asynchronously
        await SaveAsync();
        // update the _records property to the post-save DataRecords list asynchronously
        _records = await GetRecordsAsync();
    }
}