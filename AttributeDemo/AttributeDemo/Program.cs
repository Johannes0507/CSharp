namespace AttributeDemo
{
    class DisplayNameAttribute : Attribute 
    {
        public string Name { get; set; }
    }

    class DemoType
    {
        [DisplayName(Name = "MyID")]
        [Obsolete("不要再用嘍！", error: false)]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            DemoType d = new DemoType();
            d.Id = 3;
            var type = typeof(DemoType);
            foreach (var pi in type.GetProperties())
            {
                var attrs = pi.GetCustomAttributes(false);
                foreach (var attr in attrs)
                {
                    if(attr is DisplayNameAttribute dn) 
                    {
                        Console.WriteLine($"{dn.Name}={}");
                    }
                }
            }
        }
    }
}