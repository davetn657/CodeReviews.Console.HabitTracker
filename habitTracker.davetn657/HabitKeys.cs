class HabitKeys
{
    public string Name { get; set; } = string.Empty;
    public string? Id { get; set; } = string.Empty;
    public string? Date { get; set; } = string.Empty;
    public string? Measurement { get; set; } = string.Empty;

    public void Add(string columnName)
    {
        if(Id == string.Empty) Id = columnName;
        else if (Date == string.Empty) Date = columnName;
        else if (Measurement == string.Empty) Measurement = columnName;
    }
}
