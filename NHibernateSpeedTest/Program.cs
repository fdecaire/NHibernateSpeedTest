using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using System.IO;

namespace NHibernateSpeedTest
{
    class Program
    {
        static void Main(string[] args)
        {
var nHiberatePerformanceTests = new NHibernatePerformanceTests();
	        nHiberatePerformanceTests.RunAllTests();
        }

        private static void Setup()
        {
            using (var db = NHibernateHelper.OpenSession())
            {
                // delete any records from previous run
                var deptQuery = (from dept in db.Query<Department>() select dept).ToList();
                using (db.BeginTransaction())
                {
                    foreach (var item in deptQuery)
                    {
                        db.Delete(item);
                    }
                    db.Transaction.Commit();
                }

                var personQuery = (from pers in db.Query<Person>() select pers).ToList();
                using (db.BeginTransaction())
                {
                    foreach (var item in personQuery)
                    {
                        db.Delete(item);
                    }
                    db.Transaction.Commit();
                }

                var myDepartment = new Department()
                {
                    name = "Operations"
                };

                db.Save(myDepartment);
            }
        }
		

        private static void TestSelect()
        {


        }


        private static void TestUpdate()
        {

        }


        private static void TestDelete()
        {

        }
    }
}
