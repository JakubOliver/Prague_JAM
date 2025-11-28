class Gandalf : Person
{
    public int HP { get; set; } = 250;
    public int Damage { get; set; } = 30;

    public PersonClothing head { get; set; } = PersonClothing.Gandalf;
    public PersonClothing body { get; set; } = PersonClothing.Gandalf;
    public PersonClothing weapon { get; set; } = PersonClothing.Gandalf;
}
