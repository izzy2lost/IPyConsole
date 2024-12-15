using System.ComponentModel;

namespace DynamicFilterinApi.Models
{
    public record ProductName(string Name);

    public record Category(string Name);

    public record PriceRange(decimal? Min, decimal? Max);

    public class ProductSearchCriteria
    {
        public ProductName[]? Names { get; set; }

        public Category[]? Categories { get; set; }

        public PriceRange? Price { get; set; }

        public bool? IsActive { get; set; }
    }
}
