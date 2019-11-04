using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Hierarchy;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HierarchyExp
{
    class Program
    {
        static void Main(string[] args)
        {
            var User = new User();
            User.Name = DbHierarchyServices.GetRoot().ToString();
            var news = new UnitTestContext()._context;
            news.Users.Add(User);
            news.SaveChanges();
            Console.WriteLine(User.Name.ToString());
        }
    }


    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public HierarchyId OrgNode { get; set; }
    }
}
