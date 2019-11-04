using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HierarchyExp
{
    
    class UnitTestContext
    {
        public Context _context { get; set; }

        public UnitTestContext()
        {
            var connection = Effort.DbConnectionFactory.CreateTransient();
            _context = new Context(connection);
        }
    }
}
