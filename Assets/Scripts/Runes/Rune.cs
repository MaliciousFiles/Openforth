using System;
using System.Collections.Generic;

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
    public static readonly List<RuneSymbol> FIELD_RUNES = new() { RuneSymbol.HealthField, RuneSymbol.CastSpeedField };
    public static readonly RuneObject DUMMY_NUMBER = new(null, new NumberRune(0, ""));
    public static readonly RuneObject DUMMY_PLAYER = new(null, new SymbolRune(RuneSymbol.PlayerA, ""));
    
    public readonly RuneSymbol symbol;

    public SymbolRune(RuneSymbol symbol, string display) : base(display)
    {
        this.symbol = symbol;
    }

    private static bool IsPlayer(RuneObject obj)
    {
        if (obj.rune.GetType() != typeof(SymbolRune)) return false;
        SymbolRune rune = (SymbolRune) obj.rune;

        return rune.symbol == RuneSymbol.PlayerA || rune.symbol == RuneSymbol.PlayerB ||
               rune.symbol == RuneSymbol.PlayerC || rune.symbol == RuneSymbol.PlayerD;
    }
    
    private static bool IsField(RuneObject obj)
    {
        if (obj.rune.GetType() != typeof(SymbolRune)) return false;
        SymbolRune rune = (SymbolRune) obj.rune;

        return rune.symbol == RuneSymbol.HealthField || rune.symbol == RuneSymbol.CastSpeedField;
    }

    private static bool IsNumber(RuneObject obj)
    {
        return obj.rune.GetType() == typeof(NumberRune) || IsField(obj);
    }

    public int GetTargets(Stack<RuneObject> stack)
    {
        RuneObject cur = stack.Pop();
        RuneObject obj1, obj2;
        
        switch (symbol)
        {
            case RuneSymbol.PlayerA:
                return 0;
            case RuneSymbol.PlayerB:
                return 0;
            case RuneSymbol.PlayerC:
                return 0;
            case RuneSymbol.PlayerD:
                return 0;
            case RuneSymbol.Harm:
                // pops player and number
                if (stack.TryPop(out obj1) && stack.TryPop(out obj2) &&
                    (IsNumber(obj1) && IsPlayer(obj2) || IsNumber(obj2) && IsPlayer(obj1))) 
                {
                    return 2;
                }
                return -1;
            case RuneSymbol.Add:
                // pops two numbers or two players, pushing a new number or a player group
                if (stack.TryPop(out obj1) && stack.TryPop(out obj2) && obj1.rune.GetType() == obj2.rune.GetType() &&
                    (IsNumber(obj1) || IsPlayer(obj1)))
                {
                    // TODO: replace `DUMMY_PLAYER` with a player group
                    stack.Push(IsNumber(obj1) ? DUMMY_NUMBER : DUMMY_PLAYER);
                    return 2;
                }
                return -1;
            case RuneSymbol.Subtract:
                // pops two numbers, pushing a number
                if (stack.TryPop(out obj1) && stack.TryPop(out obj2) && IsNumber(obj1) && IsNumber(obj2))
                {
                    stack.Push(DUMMY_NUMBER);
                    return 2;
                }
                return -1;
            case RuneSymbol.Pop:
                // pops anything
                if (stack.TryPop(out obj1))
                {
                    return 1;
                }
                return -1;
            case RuneSymbol.Invert:
                // pops number or TurnOrder, pushing number or TurnOrder respectively
                if (stack.TryPop(out obj1) && (IsNumber(obj1) ||
                                               ((SymbolRune)obj1.rune).symbol == RuneSymbol.TurnOrder))
                {
                    stack.Push(IsNumber(obj1) ? DUMMY_NUMBER : obj1); // either inverts or reverses turn order: either pushes number or a new turn order 
                    return 1;
                }
                return -1;
            case RuneSymbol.HealthField:
                // pops player, pushing field
                if (stack.TryPop(out obj1) && IsPlayer(obj1))
                {
                    stack.Push(cur);
                    return 1;
                }
                return -1;
            case RuneSymbol.CastSpeedField:
                // pops player, pushing field
                if (stack.TryPop(out obj1) && IsPlayer(obj1))
                {
                    stack.Push(cur);
                    return 1;
                }
                return -1;
            case RuneSymbol.TurnOrder:
                // TODO: no idea ._.
                return -1;
            case RuneSymbol.AddInPlace:
                // pops two numbers, pushing number if neither input was a field
                if (stack.TryPop(out obj1) && stack.TryPop(out obj2) && IsNumber(obj1) && IsNumber(obj2) &&
                    !(IsField(obj1) && IsField(obj2))) 
                {
                    if (!IsField(obj1) && !IsField(obj2)) stack.Push(DUMMY_NUMBER);
                    return 2;
                }
                return -1;
            case RuneSymbol.SubtractInPlace:
                // pops two numbers, pushing number
                if (stack.TryPop(out obj1) && stack.TryPop(out obj2) && IsNumber(obj1) && IsNumber(obj2) &&
                    !(IsField(obj1) && IsField(obj2))) 
                {
                    if (!IsField(obj1) && !IsField(obj2)) stack.Push(DUMMY_NUMBER);
                    return 2;
                }
                return -1;
            case RuneSymbol.Alias:
                // TODO: no idea ._.
                return -1;
            case RuneSymbol.Skip:
                // TODO: handle this... somehow
                return -1;
            default:
                return 0;
        }
    }
}