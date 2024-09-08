using Newtonsoft.Json;

namespace LinqDemo
{
    internal static class ShoeHelper
    {
        // 以 this 為起始寫出名為 ShoeMark 的 Shoe.ShoeMark 擴充方法
        public static void ShoeMark(this Shoe shoe, int id)
        {
            shoe.id = id;
        }

        public static string Serialize<DT> (this DT obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static DT Deserialize<DT> (this string json)
        {
            return JsonConvert.DeserializeObject<DT>(json);
        }
    }
}
