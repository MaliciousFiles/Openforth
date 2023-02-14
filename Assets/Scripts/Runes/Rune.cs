public abstract class Rune
{
    public readonly string display;

    public Rune(string display)
    {
        this.display = display;
    }
}

public class NumberRune : Rune
{
    public readonly int number;

    public NumberRune(int number, string display) : base(display)
    {
        this.number = number;
    }
}

public class SymbolRune : Rune
{
    public readonly RuneSymbol symbol;

    public SymbolRune(RuneSymbol symbol, string display) : base(display)
    {
        this.symbol = symbol;
    }
}