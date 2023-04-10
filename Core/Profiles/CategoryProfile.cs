using AutoMapper;
using Core.Entities;
using Core.Helpers;
using Core.Models.Category;

namespace Core.Profiles
{
    public class CategoryProfile  : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, PostCategoryDto>().ReverseMap();
            CreateMap<Category, PutCategoryDto>().ReverseMap();
            CreateMap<Category, CategoryVm>()
                .ForMember(destination => destination.Name,
                                options => options.MapFrom(source =>
                                         LanguageHelper.IsArabic() ? source.NameArabic : source.NameEnglish))
                .ForMember(destination => destination.Description,
                                options => options.MapFrom(source =>
                                         LanguageHelper.IsArabic() ? source.DescriptionArabic : source.DescriptionEnglish));
        }
    }
}
