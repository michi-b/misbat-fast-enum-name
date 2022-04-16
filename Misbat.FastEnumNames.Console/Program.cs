namespace Misbat.FastEnumNames.Console;

[FastNamed]
public enum TestEnum : ulong
{
    Test1,
    Test2,
    Test3,

    Test5 = Test1
}

public static partial class Program
{
    static void Main()
    {
    }

}