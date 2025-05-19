using EcomerceMVC.Data;

namespace EcomerceMVC.ViewModels
{
    public class ProductViewModel
    {
        public string? SearchTerm { get; set; }
        public int? SelectedCategoryId { get; set; }

        public List<Loai> Categories { get; set; } = new();
        public List<HangHoa> Products { get; set; } = new();
    }
}
