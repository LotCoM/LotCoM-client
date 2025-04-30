namespace LotCoMClient.Models.Options;

public class VariableFieldSet(int? JBKNumber = null, string? LotNumber = null, int? DeburrJBKNumber = null, int? DieNumber = null, string? ModelNumber = null, string? HeatNumber = null) {
    public int? JBKNumber = JBKNumber;
    public string? LotNumber = LotNumber;
    public int? DeburrJBKNumber = DeburrJBKNumber;
    public int? DieNumber = DieNumber;
    public string? ModelNumber = ModelNumber;
    public string? HeatNumber = HeatNumber;
}