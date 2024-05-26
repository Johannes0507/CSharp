using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inherit
{
    internal class Program
    {
        // 抽象類別
        abstract class Shape
        {
            public abstract double Area();
        }

        class Triangle : Shape
        {
            float height=3, width=4;
            public override double Area()
            {
                return height * width / 2;
            }
        }

        internal class Shoe
        {
            protected string material;
            public virtual void ShowMaterial()
            {
                Console.WriteLine("鞋底");
                Console.WriteLine("膠");
            }
        }
        internal class On : Shoe
        {
            public string 鞋帶;
            public override void ShowMaterial()
            {
                Console.WriteLine("My鞋底");
                Console.WriteLine("My膠");
            }
        }
        class Puma : Shoe
        {
        }
        static void Main(string[] args)
        {
            Shoe shoe = new Shoe();
            //On on = new On();
            Shoe onShoe = new On();
            Shoe[] shoes = new Shoe[] { new Shoe(), new On(), new Puma() };
            foreach (var sh in shoes)
            {
                sh.ShowMaterial();
            }
            //On shoeOn = new Shoe();
        }
    }
}
