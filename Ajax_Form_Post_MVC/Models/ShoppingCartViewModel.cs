using System.Collections.Generic;
namespace MVC_BanHoa.Models
{
    public class ShoppingCartViewModel
    {
        public ShoppingCartViewModel()
        {
            this.CartItems = new List<CartModel>();
        }
        public List<CartModel> CartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}