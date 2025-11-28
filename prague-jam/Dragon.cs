class Dragon : Person
{
    public int HP { get; set; } = 300;
    public int Damage { get; set; } = 50;

    public PersonClothing head { get; set; } = PersonClothing.Dragon;
    public PersonClothing body { get; set; } = PersonClothing.Dragon;
    public PersonClothing weapon { get; set; } = PersonClothing.Dragon;
}