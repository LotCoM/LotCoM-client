namespace LotCoMClient.Models.Datasources;

/// <summary>
/// Provides controlled access and manipulation of data in the Database Table located at DataTablePath.
/// </summary>
/// <param name="DataTablePath">A full file path to a Database Table file in the LotCoM database.</param>
public class DataTable(string DataTablePath) {
    /// <summary>
    /// The Path of the database table file in the LotCoM database filing system.
    /// </summary>
    private readonly string _path = DataTablePath;
    /// <summary>
    /// Holds the currently-read entries in the DataTable.
    /// </summary>
    private List<string> _entries = [];

    /// <summary>
    /// Opens, reads, and formats the text in DataTable._path as a list of string entries.
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
    /// <returns>A List of string entries.</returns>
    private List<string> Read() {
        // read the Database Table at the _path property
        string Text = File.ReadAllText(_path);
        // separate the read text into entry lines (split by newline character)
        List<string> Entries = Text.Split("\n").ToList();
        // remove any empty lines
        Entries = Entries.Where(x => !x.Equals("")).ToList();
        return Entries;
    }

    /// <summary>
    /// Asynchronously opens, reads, and formats the text in DataTable._path as a list of string entries.
    /// </summary>
    /// <exception cref="OperationCanceledException"></exception>
    /// <returns>A List of string entries.</returns>
    private async Task<List<string>> ReadAsync() {
        // read the Database Table as the _path property asynchronously
        string Text = await File.ReadAllTextAsync(_path);
        // format the Text as entries on a new CPU thread
        List<string> Entries = await Task.Run(() => {
            List<string> Split = Text.Split("\n").ToList();
            // remove any empty lines
            Split = Split.Where(x => !x.Equals("")).ToList();
            return Split;
        });
        return Entries;
    }

    /// <summary>
    /// Opens and overwrites the data in DataTable._path with the current list of strings in DataTable._entries.
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
        // format the Entries in _entries as single string separated by newlines
        string Text = "";
        foreach (string _entry in _entries) {
            Text = $"{Text}{_entry}\n";
        }
        // write the single string as text to the Database Table file at _path
        File.WriteAllText(_path, Text);
    }

    /// <summary>
    /// Asynchronously opens and overwrites the data in DataTable._path with the current list of strings in DataTable._entries.
    /// </summary>
    /// <exception cref="OperationCanceledException"></exception>
    /// <returns>A List of string entries.</returns>
    private async Task SaveAsync() {
        // format the Entries in _entries as single string separated by newlines on a new CPU thread
        string Text = await Task.Run(() => {
            string Formatted = "";
            foreach (string _entry in _entries) {
                Formatted = $"{Formatted}{_entry}\n";
            }
            // return the formatted single string
            return Formatted;
        });
        // asynchronously write the single string as text to the Database Table file at _path
        await File.WriteAllTextAsync(_path, Text);
    }

    /// <summary>
    /// Updates the Entries currently stored in DataTable._entries and returns the list held by the property.
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
    /// <returns>A List of string entries.</returns>
    public List<string> GetEntries() {
        // update the Entries in runtime
        _entries = Read();
        // return the Entries stored in runtime
        return _entries;
    }

    /// <summary>
    /// Asynchronously updates the Entries currently stored in DataTable._entries and returns the list held by the property.
    /// </summary>
    /// <exception cref="OperationCanceledException"></exception>
    /// <returns>A List of string entries.</returns>
    public async Task<List<string>> GetEntriesAsync() {
        // update the Entries in runtime
        _entries = await ReadAsync();
        // return the Entries stored in runtime
        return _entries;
    }

    /// <summary>
    /// Saves Entries to DataTable._path and updates DataTable._entries in runtime.
    /// </summary>
    /// <param name="Entries">A List of entry strings to write to DataTable._path.</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="System.Security.SecurityException"></exception>
    public void SaveEntries(List<string> Entries) {
        // update the _entries property
        _entries = Entries;
        // save the DataTable
        Save();
        // update the _entries property to the post-save entries list
        _entries = GetEntries();
    }

    /// <summary>
    /// Asynchronously saves Entries to DataTable._path and updates DataTable._entries in runtime.
    /// </summary>
    /// <param name="Entries">A List of entry strings to write to DataTable._path.</param>
    /// <exception cref="OperationCanceledException"></exception>
    /// <returns></returns>
    public async Task SaveEntriesAsync(List<string> Entries) {
        // update the _entries property
        _entries = Entries;
        // save the DataTable asynchronously
        await SaveAsync();
        // update the _entries property to the post-save entries list asynchronously
        _entries = await GetEntriesAsync();
    }
}