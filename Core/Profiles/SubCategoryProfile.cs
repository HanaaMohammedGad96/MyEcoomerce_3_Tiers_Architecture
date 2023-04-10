using AutoMapper;
using Core.Entities;
using Core.Helpers;
using Core.Models.SubCategory;

namespace Core.Profiles
{
    public class SubCategoryProfile : Profile
    {
        public SubCategoryProfile()
        {
            CreateMap<SubCategory, PostSubCategoryDto>().ReverseMap();    
            CreateMap<SubCategory, PutSubCategoryDto>().ReverseMap();
            CreateMap<SubCategory, SubCategoryVm>()
                .ForMember(destination => destination.ParentCategoryName, 
                               options => options.MapFrom(source => 
                                       LanguageHelper.IsArabic() ? source.Category.NameArabic : source.Category.NameEnglish))
                .ForMember(destination => destination.Name,
                               options => options.MapFrom(source =>
                                       LanguageHelper.IsArabic() ? source.NameArabic : source.NameEnglish))
                .ForMember(destination => destination.Description,
                               options => options.MapFrom(source =>
                                       LanguageHelper.IsArabic() ? source.DescriptionArabic : source.DescriptionEnglish));
        }
    }
}
