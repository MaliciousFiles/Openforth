using System;
using System.Collections.Generic;
using System.Linq;

public class RuneDecks
{
    private static readonly Dictionary<int, int> initialNumberDeck = new()
    {
        { 0, 6 },
        { 1, 10 },
        { 2, 8 },
        { 3, 6 },
        { 4, 4 },
        { 5, 2 }
    };
    private static readonly Dictionary<RuneSymbol, int> initialSymbolDeck = new()
    {
        { RuneSymbol.Harm, 5 },
        { RuneSymbol.Add, 4 },
        { RuneSymbol.Subtract, 2 },
        { RuneSymbol.Pop, 2 },
        { RuneSymbol.Invert, 1 },
        { RuneSymbol.HealthField, 2 },
        { RuneSymbol.CastSpeedField, 2 },
        { RuneSymbol.TurnOrder, 1 },
        { RuneSymbol.AddInPlace, 2 },
        { RuneSymbol.SubtractInPlace, 1 },
        { RuneSymbol.Alias, 2 },
        { RuneSymbol.Skip, 2 }
    };

    private static List<NumberRune> numberDeck;
    private static List<SymbolRune> symbolDeck;

    public static void InitializeDecks()
    {
        Random rnd = new();

        numberDeck = initialNumberDeck.SelectMany(kvp => Enumerable.Repeat(new NumberRune(kvp.Key, kvp.Key.ToString()), kvp.Value))
            .OrderBy((item) => rnd.Next()).ToList();
        symbolDeck = initialSymbolDeck.SelectMany(kvp => Enumerable.Repeat(new SymbolRune(kvp.Key, "S"+(int) kvp.Key), kvp.Value))
            .OrderBy((item) => rnd.Next()).ToList();
    }

    public static Rune Draw(RuneType deck)
    {
        Rune rune = deck == RuneType.Number ? numberDeck[0] : symbolDeck[0];

        if (deck == RuneType.Number) numberDeck.RemoveAt(0);
        else symbolDeck.RemoveAt(0);

        return rune;
    }
}

public enum RuneType
{
    Number,
    Symbol
}