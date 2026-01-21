using System.Web.Mvc;

namespace WebChartParse.Controllers
{
    public class HomeController : Controller
    {
        public void Die(string message)
        {
            Response.Write(message);
            Response.End();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "This page explains what the application does and how to use it.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
