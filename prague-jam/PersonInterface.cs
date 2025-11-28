enum PersonClothing
{
    Player,
    Dragon,
    Soldier,
    JohnWick,
    Gandalf
}

interface Person
{
    int HP { get; set;  }
    int Damage { get; set;  }   
    
    PersonClothing head { get; set; }
    PersonClothing body { get; set; }
    PersonClothing weapon { get; set; }
}