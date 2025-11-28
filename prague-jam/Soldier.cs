class Soldier : Person
{
    public int HP { get; set; } = 150;
    public int Damage { get; set; } = 20;

    public PersonClothing head { get; set; } = PersonClothing.Soldier;
    public PersonClothing body { get; set; } = PersonClothing.Soldier;
    public PersonClothing weapon { get; set; } = PersonClothing.Soldier;
}