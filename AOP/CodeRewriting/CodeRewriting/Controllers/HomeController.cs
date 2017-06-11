using System.Web.Mvc;
using CodeRewriting.Models;
using Castle.DynamicProxy;

namespace CodeRewriting.Controllers
{
    public class HomeController : Controller
    {
        private Squaring _squaringEvaluator = new Squaring();

        // GET: Home
        public ActionResult Index()
        {
            return View(new SquaringResult());
        }

        [HttpPost]
        public ActionResult Index(double value, int power)
        {
            var result = _squaringEvaluator.Square(value, power);

            return View(new SquaringResult { Power = power, Value = value, Result = result });
        }
    }
}