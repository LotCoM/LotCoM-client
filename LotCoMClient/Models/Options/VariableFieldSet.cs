namespace LotCoMClient.Models.Datasources;

public class VariableFieldSet(string JBKNumber = "", string LotNumber = "", string DeburrJBKNumber = "", string DieNumber = "", string ModelNumber = "", string HeatNumber = "") {
    public string JBKNumber = JBKNumber;
    public string LotNumber = LotNumber;
    public string DeburrJBKNumber = DeburrJBKNumber;
    public string DieNumber = DieNumber;
    public string ModelNumber = ModelNumber;
    public string HeatNumber = HeatNumber;
}