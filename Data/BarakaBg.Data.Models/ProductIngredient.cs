namespace BarakaBg.Data.Models
{
    public class ProductIngredient
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int IngredientId { get; set; }

        public virtual Ingredient Ingredient { get; set; }
    }
}