using Godot;

namespace Praguejam;

public partial class YearCounter : Node
{
    private int _year = 15;

    void NewYear()
    {
        _year++;
    }

    int GetCurrentYear()
    {
        return _year;
    }
}