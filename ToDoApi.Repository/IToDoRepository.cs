using ToDoApi.Models;
using System.Collections.Generic;

namespace ToDoApi.Repository
{
    public interface IToDoRepository
    {
        IEnumerable<ToDo> GetAllToDos();

        ToDo GetToDo(string id);

        ToDo AddToDo(ToDo item);

        bool DeleteToDo(string id);

        bool UpdateToDo(string id, ToDo item);
    }
}
