namespace otk;

public static class Program
{
    private static void Main()
    {
        using var okno = new Okno(1366, 768, "okno");
        okno.Run();   
    }
}