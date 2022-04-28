using Digisoft.Sales.Attributes;
using System.Web;
using System.Web.Mvc;

namespace Digisoft.Sales
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ExceptionHandlerAttribute());
            //filters.Add(new HandleErrorAttribute());
        }
    }
}
