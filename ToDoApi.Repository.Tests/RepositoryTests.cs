using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute.Core;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using ToDoApi.Models;
using ToDoApi.Repository;
using NSubstitute;


namespace ToDoApi.Repository.Tests
{
    [TestFixture]
    public class RepositoryTests
    {
        private IToDoRepository _repo;

        [SetUp]
        public void Setup()
        {
            _repo = Substitute.For<IToDoRepository>();    
        }

        [TearDown]
        public void TearDown()
        {
            _repo = null;
        }

        [Test]
        public void GetAllTodos_ShouldReturnAll()
        {
            // Arrange
            var data = GetToDoSamplesList();

            _repo.GetAllToDos().Returns(data);

            // Act 
            var todos = _repo.GetAllToDos();

            // Assert
            Assert.That(todos, Is.EqualTo(data));
        }

        [Test]
        public void GetTodoById_ShouldReturnSingleToDoForGivenId()
        {
            // Arrange
            var data = GetToDoSamplesList();
            var todoId = "1";

            _repo
                .GetToDo(Arg.Any<string>())
                .Returns(data.Single(t => t.Id == todoId));

            // Act
            var todo = _repo.GetToDo(todoId);
            
            // Assert
            Assert.That(todo, Is.EqualTo(data.Single(t => t.Id == todoId)));
        }

        [Test]
        public void AddToDo_ShouldAddOneToCollection()
        {
            // Arrange
            var data = GetToDoSamplesList();
            var initialToDosCount = data.Count();
            ToDo addedToDo = new ToDo {Id = "99", Task = "Added task"};

            _repo
               .When(t => _repo.AddToDo(addedToDo))
               .Do(t => data.Add(addedToDo));

            // Act
            _repo.AddToDo(addedToDo);

            // Assert
            Assert.AreEqual(initialToDosCount+1, data.Count());
        }

        [Test]
        public void AddToDo_ShouldReturnAddedToDo()
        {
            // Arrange
            ToDo addedToDo = new ToDo { Id = "99", Task = "Added task" };

            _repo
                .AddToDo(Arg.Any<ToDo>())
                .Returns(addedToDo);

            // Act
            var todo = _repo.AddToDo(addedToDo);

            // Assert
            Assert.AreEqual(todo, addedToDo);
        }

        [Test]
        public void DeleteToDo_ShouldRemoveFromCollection()
        {
            // Arrange
            var data = GetToDoSamplesList();
            var initialToDosCount = data.Count();
            var todoId = "1";
            int index = data.FindIndex(t => t.Id == todoId);
 
            _repo
                .When(t => _repo.DeleteToDo(Arg.Any<string>()))
                .Do(t => data.RemoveAt(index));

            // Act
            _repo.DeleteToDo(todoId);

            // Assert
            Assert.AreEqual(initialToDosCount - 1, data.Count());
        }

        [Test]
        public void DeleteNonExistentToDo_ShouldReturnFalse()
        {
            // Arrange
            var data = GetToDoSamplesList();
            var todoId = "199";

            //var test = data.FindIndex(t => t.Id == todoId);
            var test = data.FirstOrDefault(t => t.Id == todoId); // returns null
            var exists = test != null;

            _repo.DeleteToDo(todoId).Returns(exists);

            // Act
            var result = _repo.DeleteToDo(todoId);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DeleteExistingToDo_ShouldReturnTrue()
        {
            // Arrange
            var data = GetToDoSamplesList();
            var todoId = "1";

            var test = data.FirstOrDefault(t => t.Id == todoId); // returns null
            var exists = test != null;

            _repo.DeleteToDo(todoId).Returns(exists);

            // Act
            var result = _repo.DeleteToDo(todoId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void UpdateExistingToDo_ShouldReturnTrue()
        {
            // Arrange
            var data = GetToDoSamplesList();
            var todoId = "1";

            int index = data.FindIndex(t => t.Id == todoId);
            var todo = data.FirstOrDefault(t => t.Id == todoId);

            _repo.UpdateToDo(Arg.Any<string>(), Arg.Any<ToDo>()).Returns(true);


            todo.Comment = "updated comment";
            var result = _repo.UpdateToDo(todoId, todo);

            Assert.IsTrue(result);
        }


        private static List<ToDo> GetToDoSamplesList()
        {
            return new List<ToDo> 
            { 
                new ToDo { Id = "1", Task = "Task number 1", Comment = "Urgent", Done = true, DateSaved = new DateTime(2014,09,20,09,32,56)}, 
                new ToDo { Id = "2", Task = "Task number 2", Comment = "Important", Done = false, DateSaved = new DateTime(2014,09,20,09,32,56) }, 
                new ToDo { Id = "3", Task = "Task number 3", Comment = "Minor", Done = true, DateSaved = new DateTime(2014,09,20,09,32,56) } ,
                new ToDo { Id = "4", Task = "Task number 4", Comment = "Due end of month", Done = false, DateSaved = new DateTime(2014,09,20,09,32,56) }, 
                new ToDo { Id = "5", Task = "Task number 5", Comment = "Postponed until next year", Done = false, DateSaved = new DateTime(2014,09,20,09,32,56) } 
            };
        }
    
    }
}
