public enum PersonClothing
{
    Player,
    Dragon,
    Soldier,
    JohnWick,
    Gandalf
}

public interface IPerson
{
    int HP { get; set;  }
    int Damage { get; set;  }   
    
    PersonClothing head { get; set; }
    PersonClothing body { get; set; }
    PersonClothing weapon { get; set; }
}


public interface IMove
{
    void Move(double delta);
}

