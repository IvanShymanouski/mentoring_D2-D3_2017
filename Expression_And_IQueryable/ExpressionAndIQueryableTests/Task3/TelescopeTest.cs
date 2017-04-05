using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Linq;
using ExpressionAndIQueryble.E3SClient.Entities;

namespace ExpressionAndIQueryble
{
    [TestClass]
    public class TelescopeTest
    {
        [TestMethod]
        public void SwapEqualStatementElementsTest()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            foreach (var emp in employees.Where(e => "EPBYMINW5387" == e.workstation))
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.office);
            }
        }

        [TestMethod]
        public void StringEndsWithTest()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            foreach (var emp in employees.Where(e => e.workstation.EndsWith("5387")))
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.office);
            }
        }

        [TestMethod]
        public void StringStartWithTest()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            foreach (var emp in employees.Where(e => e.workstation.StartsWith("EPBYMINW53")))
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.office);
            }
        }

        [TestMethod]
        public void StringContainsTest()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            foreach (var emp in employees.Where(e => e.workstation.Contains("NW538")))
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.office);
            }
        }

        [TestMethod]
        public void StatementAndTest()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            foreach (var emp in employees.Where(e => "EPBYMINW5387" == e.workstation && e.nativename.StartsWith("Ив")))
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.office);
            }
        }
    }
}
