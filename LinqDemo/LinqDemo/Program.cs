using LinqDemo;

class Shoe
{
    public int id;
}
class FactoryFactory 
{
    
}
internal class Program
{
    private static void Main(string[] args)
    {
        var list = (from v in Enumerable.Range(1, 100) select v * 2).ToList();
        list.Where(v => v % 6 == 0).Where(v => v % 8 == 0).ToList().ForEach(Console.WriteLine);
        Shoe shoe = new ();
        shoe.ShoeMark(1);
    }
}