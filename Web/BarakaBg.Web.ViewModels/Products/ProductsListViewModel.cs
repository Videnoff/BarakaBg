namespace BarakaBg.Web.ViewModels.Products
{
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;
    using BarakaBg.Data.Models;
    using BarakaBg.Services.Mapping;
    using Ganss.XSS;

    public class ProductsListViewModel : PagingViewModel, IMapFrom<Product>, IHaveCustomMappings
    {

        public int Id { get; set; }

        public string Title { get; set; }

        public List<ProductComment> ProductComments { get; set; }

        public string Comment { get; set; }

        public int ProductId { get; set; }

        public int Rating { get; set; }

        public IEnumerable<ProductInListViewModel> Products { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Product, SingleProductViewModel>()
                .ForMember(
                    x => x.ProductComments,
                    opt => opt.MapFrom(member => member.ProductComments.OrderByDescending(x => x.CreatedOn)));
        }
    }
}
