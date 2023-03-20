namespace shopWebAPI.Models
{
	public class CartProduct
	{

        public int? Fk_product_id { get; set; }
        public int? Cart_product_id { get; set; }
        public decimal Price { get; set; }
        public string? Name { get; set; } 
        public string? Image_link { get; set; }
        public int? Quantity { get; set; }
        public double Special_price { get; set; }
    }
}


/*
 
select
    p.name,
    p.price,
    p.image_link,
    p.special_price,
    public.order_product.id,
    public.order_product.quantity

from
    public.order_product

inner join product p on
    order_product.fk_product_id = p.id
where  public.order_product.status= 1
order by public.order_product.id





 * **/