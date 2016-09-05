using FluentNHibernate.Mapping;

namespace NHibernateSpeedTest
{
    public class Department
    {
        public virtual int id { get; set; }
        public virtual string name { get; set; }
    }

    public class DepartmentMap : ClassMap<Department>
    {
        public DepartmentMap()
        {
            Id(u => u.id);
            Map(u => u.name).Nullable();
        }
    }
}
