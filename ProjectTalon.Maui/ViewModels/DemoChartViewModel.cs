
namespace ProjectTalon.Maui.ViewModels
{
    public class DemoChartViewModel
    {
        public List<Sales> Data { get; set; }

        public DemoChartViewModel()
        {
            Data = new List<Sales>()
            {
                new Sales(){Product = "iPad", SalesRate = 25},
                new Sales(){Product = "iPhone", SalesRate = 35},
                new Sales(){Product = "MacBook", SalesRate = 15},
                new Sales(){Product = "Mac", SalesRate = 5},
                new Sales(){Product = "Others", SalesRate = 10},
            };
        }
    }
    public class Sales
    {
        public string Product { get; set; }
        public double SalesRate { get; set; }
    }
}
