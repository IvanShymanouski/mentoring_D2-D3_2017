using System.Web.Mvc;
using CodeRewriting.Models;
using Castle.DynamicProxy;
using AOPLogger;

namespace CodeRewriting.Controllers
{
    public class HomeController : Controller
    {
        private ISquaring _squaringEvaluator;

        public HomeController()
        {
            var generator = new ProxyGenerator();
            _squaringEvaluator =
                generator.CreateInterfaceProxyWithTarget<ISquaring>(
                    new Squaring(), new LogInterceptor());
        }

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