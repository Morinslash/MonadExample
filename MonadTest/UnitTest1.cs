using System;
using System.Reflection;
using System.Reflection.Emit;
using Xunit;

namespace MonadTest;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var result = WorkingExample
            .GetRandomNumberOver5()
            .MapRight(WorkingExample.SquareRoot)
            .MapLeft(_ => 0d);
    }
}

public static class WorkingExample
{
    public static DiscriminatedUnion<string, int> GetRandomNumberOver5()
    {
        var result = Random.Shared.Next(10);
        if (result > 5)
        {
            return new(result);
        }

        return new("too low number");
    }

    public static DiscriminatedUnion<string, double> SquareRoot(int number)
    {
        if (number < 0)
        {
            return new("Not able to process");
        }
        return new(Math.Sqrt(number));
    }
}

public class DiscriminatedUnion<TLeft,TRight>
{
    private TLeft? value1;
    private TRight? value2;
    private Branch currentBranch;

    public DiscriminatedUnion(TLeft value)
    {
        value1 = value;
        currentBranch = Branch.LEFT;
    }

    public DiscriminatedUnion(TRight value)
    {
        value2 = value;
        currentBranch = Branch.RIGHT;
    }

    public DiscriminatedUnion<TLeft, TNew> MapRight<TNew>(Func<TRight, TNew> function)
    {
        if (currentBranch == Branch.LEFT)
        {
            return new(value1);
        }
        return new(function(value2));
    }
    
    public DiscriminatedUnion<TNew, TRight> MapLeft<TNew>(Func<TLeft, TNew> function)
    {
        if (currentBranch == Branch.RIGHT)
        {
            return new(value2);
        }
        return new(function(value1));
    }
}

public enum Branch
{
    LEFT,
    RIGHT,
}