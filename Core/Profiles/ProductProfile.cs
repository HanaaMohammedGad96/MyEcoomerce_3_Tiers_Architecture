using AutoMapper;
using Core.Entities;
using Core.Helpers;
using Core.Models.Product;

namespace Core.Profiles;

public class ProductProfile : Profile
{
	public ProductProfile()
	{
		CreateMap<Product, PostProductDto>().ReverseMap();
		CreateMap<Product, PutProductDto>().ReverseMap();

		CreateMap<Product, ProductVm>()
			.ForMember(destination => destination.Name,
							options => options.MapFrom(source =>
									LanguageHelper.IsArabic() ? source.NameArabic : source.NameEnglish))
			.ForMember(destination => destination.ShortDescription,
							options => options.MapFrom(source =>
									LanguageHelper.IsArabic() ? source.ShortDescriptionArabic : source.ShortDescriptionEnglish));
       
		CreateMap<ProductImage, ProductImageVm>().ReverseMap();
    }
}
