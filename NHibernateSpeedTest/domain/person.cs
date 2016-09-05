using FluentNHibernate.Mapping;

namespace NHibernateSpeedTest
{
    public class Person
    {
        public virtual int id { get; set; }
        public virtual string first { get; set; }
        public virtual string last { get; set; }
        public virtual int department { get; set; }
    }

    public class PersontMap : ClassMap<Person>
    {
        public PersontMap()
        {
            Id(u => u.id);
            Map(u => u.first).Nullable();
            Map(u => u.last).Nullable();
            Map(u => u.department);
        }
    }
}
