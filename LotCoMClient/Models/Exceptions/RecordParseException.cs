namespace LotComClient.Models.Exceptions;

public partial class RecordParseException(string? Message = null) : Exception(message: Message) {}