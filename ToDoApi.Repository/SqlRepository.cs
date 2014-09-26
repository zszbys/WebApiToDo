using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApi.Models;

namespace ToDoApi.Repository
{
    /*
     * Implement a Sql repo here and bind it at the composition root
     * (ToDoApi.NinjectHttpResolver.Load())
     * */

    public class SqlRepository : IToDoRepository
    {
        public IEnumerable<ToDo> GetAllToDos()
        {
            throw new NotImplementedException();
        }

        public ToDo GetToDo(string id)
        {
            throw new NotImplementedException();
        }

        public ToDo AddToDo(ToDo item)
        {
            throw new NotImplementedException();
        }

        public bool DeleteToDo(string id)
        {
            throw new NotImplementedException();
        }

        public bool UpdateToDo(string id, ToDo item)
        {
            throw new NotImplementedException();
        }
    }
}
