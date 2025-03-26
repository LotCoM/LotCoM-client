namespace LotCoMClient.Models.Exceptions;

public partial class RecordParseException(string? Message) : Exception(message: Message) {}