using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Boleto.Infra;

namespace Boleto.WebApi
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            Log.Write("Api iniciada!");
        }

        protected void Application_End()
        {
            Log.Write("Api parada! Verifique o IIS");            
        }
    }
}