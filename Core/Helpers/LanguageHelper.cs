using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Core.Helpers;
                     
internal class LanguageHelper
{
    private const string DEFAULT_LANG = "ar";
    private const string LANGUAGE_HEADER_NAME = "Accept-Language";
    private static HttpContext Context => new HttpContextAccessor().HttpContext;
    public static bool IsArabic() 
    {
        StringValues Lang;
        Context.Request.Headers.TryGetValue(LANGUAGE_HEADER_NAME, out Lang);
        return string.IsNullOrEmpty(Lang) || Lang == DEFAULT_LANG;
    }
}
