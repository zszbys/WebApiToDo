using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using ToDoApi.Models;

namespace ToDoApi.Repository
{
    public class MongoRepository : IToDoRepository
    {
        readonly MongoCollection<ToDo> _todos;

        //public MongoRepository()
        //    : this("")
        //{
        //}

        public MongoRepository(string connection)
        {
            if (string.IsNullOrWhiteSpace(connection))
            {
                throw new ArgumentException("connection");
            }

            var mongoClient = new MongoClient(connection);
            MongoServer mongoServer = mongoClient.GetServer();
            MongoDatabase mongoDatabase = mongoServer.GetDatabase("nodeToDo", new WriteConcern());
            _todos = mongoDatabase.GetCollection<ToDo>("todos");
        }

        public IEnumerable<ToDo> GetAllToDos()
        {
            return _todos.FindAll();
        }

        public ToDo GetToDo(string id)
        {
            var query = Query.EQ("_id", new ObjectId(id));
            return _todos.FindOne(query);
        }

        public ToDo AddToDo(ToDo todo)
        {
            var result = _todos.Save(
                todo,
                new MongoInsertOptions
                {
                    WriteConcern = WriteConcern.Acknowledged
                });

            if (!result.Ok)
            {
                throw new Exception("Could not post todo");
            }

            return todo;
        }

        public bool DeleteToDo(string id)
        {
            var query = Query.EQ("_id", new ObjectId(id));
            var result = _todos.Remove(
               query,
               RemoveFlags.None,
               WriteConcern.Acknowledged);

            if (!result.Ok)
            {
                return false;
            }
            return true;
        }

        public bool UpdateToDo(string id, ToDo item)
        {
            var query = Query.EQ("_id", new ObjectId(id));
            var update = Update.Set("task", item.Task).Set("comment", item.Comment);

            var updateResult = _todos.Update(
                     query,
                     update,
                     new MongoUpdateOptions
                     {
                         WriteConcern = WriteConcern.Acknowledged
                     });

            if (updateResult.DocumentsAffected == 0)
            {
                return false;
            }
            return true;
        }
    }
}
