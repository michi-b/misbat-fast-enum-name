using Misbat.FastEnumNames.Attributes;

namespace Misbat.FastEnumNames.Console;

[FastNamed]
public enum TestEnum
{
    Test1,
    Test2,
}

public static class Program
{
    private static void Main()
    {
        foreach (var value in Enum.GetValues<TestEnum>())
        {
            System.Console.WriteLine($"enum with name {Enum.GetName(value)} has value {(int)value}");
        }
    }
}