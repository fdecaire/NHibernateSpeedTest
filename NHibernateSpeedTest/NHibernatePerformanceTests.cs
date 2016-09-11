using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace NHibernateSpeedTest
{
	public class NHibernatePerformanceTests
	{
		private int _departmentKey;

		public NHibernatePerformanceTests()
		{
			File.Delete(@"c:\sqlserverresults.txt");
		}

		public void InitializeData()
		{
			using (var db = NHibernateHelper.OpenSession())
			{
				// delete any records from previous run
				var personQuery = (from pers in db.Query<Person>() select pers).ToList();
				using (db.BeginTransaction())
				{
					foreach (var item in personQuery)
					{
						db.Delete(item);
					}
					db.Transaction.Commit();
				}

				var deptQuery = (from dept in db.Query<Department>() select dept).ToList();
				using (db.BeginTransaction())
				{
					foreach (var item in deptQuery)
					{
						db.Delete(item);
					}
					db.Transaction.Commit();
				}

				// insert one department
				var myDepartment = new Department
				{
					name = "Operations"
				};

				db.Save(myDepartment);

				// select the primary key of the department table for the only record that exists
				_departmentKey = (from d in db.Query<Department>() where d.name == "Operations" select d.id).FirstOrDefault();
			}
		}

		public void RunAllTests()
		{
			double smallest = -1;
			for (int i = 0; i < 5; i++)
			{
				InitializeData();

				double result = TestInsert();

				if (smallest < 0)
				{
					smallest = result;
				}
				else
				{
					if (result < smallest)
					{
						smallest = result;
					}
				}
			}
			WriteLine("INSERT:" + smallest);
			Console.WriteLine("INSERT:" + smallest);

			smallest = -1;
			for (int i = 0; i < 5; i++)
			{
				InitializeData();
				TestInsert();

				double result = TestUpdate();

				if (smallest < 0)
				{
					smallest = result;
				}
				else
				{
					if (result < smallest)
					{
						smallest = result;
					}
				}
			}
			WriteLine("UPDATE:" + smallest);
			Console.WriteLine("UPDATE:" + smallest);

			smallest = -1;
			for (int i = 0; i < 5; i++)
			{
				InitializeData();
				TestInsert();

				double result = TestSelect();

				if (smallest < 0)
				{
					smallest = result;
				}
				else
				{
					if (result < smallest)
					{
						smallest = result;
					}
				}
			}
			WriteLine("SELECT:" + smallest);
			Console.WriteLine("SELECT:" + smallest);

			smallest = -1;
			for (int i = 0; i < 5; i++)
			{
				InitializeData();
				TestInsert();

				double result = TestDelete();

				if (smallest < 0)
				{
					smallest = result;
				}
				else
				{
					if (result < smallest)
					{
						smallest = result;
					}
				}
			}
			WriteLine("DELETE:" + smallest);
			Console.WriteLine("DELETE:" + smallest);

			WriteLine("");
		}

		public double TestInsert()
		{
			using (var db = NHibernateHelper.OpenSession())
			{
				// read first and last names
				var firstnames = new List<string>();
				using (var sr = new StreamReader(@"..\..\Data\firstnames.txt"))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
						firstnames.Add(line);
				}

				var lastnames = new List<string>();
				using (var sr = new StreamReader(@"..\..\Data\lastnames.txt"))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
						lastnames.Add(line);
				}

				//test inserting 10000 records (only ~1,000 names in text)
				var startTime = DateTime.Now;
				using (db.BeginTransaction())
				{
					for (int j = 0; j < 10; j++)
					{
						for (int i = 0; i < 1000; i++)
						{
							var personRecord = new Person
							{
								first = firstnames[i],
								last = lastnames[i],
								department = _departmentKey
							};

							db.Save(personRecord);
						}
					}
					db.Transaction.Commit();
				}
				var elapsedTime = DateTime.Now - startTime;

				return elapsedTime.TotalSeconds;
			}
		}

		public double TestSelect()
		{
			using (var db = NHibernateHelper.OpenSession())
			{
				// select records from the person joined by department table
				var startTime = DateTime.Now;
				for (int i = 0; i < 1000; i++)
				{
					var query = (from p in db.Query<Person>()
								 join d in db.Query<Department>() on p.department equals d.id
								 select p).ToList();
				}
				var elapsedTime = DateTime.Now - startTime;

				return elapsedTime.TotalSeconds;
			}
		}

		public double TestUpdate()
		{
			using (var db = NHibernateHelper.OpenSession())
			{
				// update all records in the person table
				var startTime = DateTime.Now;
				using (db.BeginTransaction())
				{
					var query = (from p in db.Query<Person>() select p).ToList();
					foreach (var item in query)
					{
						item.last = item.last + "2";
						db.SaveOrUpdate(item);
					}
					db.Transaction.Commit();
				}

				var elapsedTime = DateTime.Now - startTime;

				return elapsedTime.TotalSeconds;
			}
		}

		public double TestDelete()
		{
			using (var db = NHibernateHelper.OpenSession())
			{
				// delete all records in the person table
				var startTime = DateTime.Now;
				var personQuery = (from pers in db.Query<Person>() select pers).ToList();
				using (db.BeginTransaction())
				{
					foreach (var item in personQuery)
					{
						db.Delete(item);
					}
					db.Transaction.Commit();
				}
				var elapsedTime = DateTime.Now - startTime;

				return elapsedTime.TotalSeconds;
			}
		}

		public void WriteLine(string text)
		{
			using (var writer = new StreamWriter("c:\\nhibernate_speed_tests.txt", true))
			{
				writer.WriteLine(text);
			}
		}
	}
}
