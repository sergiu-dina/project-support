using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models.Services
{
    public interface IDependencyData
    {
        IEnumerable<Dependency> GetAll();
        Dependency Get(int id);
        void Add(Dependency task);
        void Update(Dependency task);
        void Delete(int id);
    }
}
