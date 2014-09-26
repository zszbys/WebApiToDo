using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using NSubstitute;
using NUnit.Framework;
using ToDoApi.Controllers;
using ToDoApi.Models;
using ToDoApi.Repository;
  
namespace ToDoApi.Tests
{
    public class ToDosControllerTests
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

        #region GET

        [Test]
        public void Get_AllToDos_Should_Return_All_ToDos()
        {
            // Arrange
            var data = GetToDoSamplesList();
            var controller = new ToDosController(_repo);

            _repo.GetAllToDos().Returns(data);

            // Act
            var response = controller.GetAllToDos() as List<ToDo>;

            // Assert
            Assert.NotNull(response);
            Assert.AreEqual(data.Count, response.Count);
        }

        [Test]
        public void Get_Controller_GetAllToDos_Should_Call_Repo_GetAllToDos()
        {
            // Arrange
            var controller = new ToDosController(_repo);

            // Act
            controller.GetAllToDos();

            //Assert
            _repo.Received().GetAllToDos();
        }

        [Test]
        public void Get_GetTodoById_Should_Return_Correct_Todo()
        {
            // Arrange
            var data = GetToDoSamplesList();
            string id = "3";
            var controller = new ToDosController(_repo);

            _repo
                .GetToDo(Arg.Any<string>())
                .Returns(data.Single(t => t.Id == id));

            // Act
            var result = controller.GetToDoById(id);

            // Assert
            Assert.AreEqual(data[2].Comment, result.Comment);
        }

        [Test]
        public void Get_GetToDoById_Should_Call_Repo_GetToDo()
        {
            // Arrange
            _repo
                .GetToDo(Arg.Any<string>())
                .Returns(new ToDo() { Task = "task" });

            var controller = new ToDosController(_repo);
            string id = "3";

            // Act
            controller.GetToDoById(id);

            //Assert
            _repo.Received().GetToDo(id);
        }

        [Test]
        public void Get_GetToDoById_Throws_When_Repository_Returns_Null()
        {
            // Arrange
            _repo
                .GetToDo(Arg.Any<string>())
                .Returns(t => null);

            // Act
            var controller = new ToDosController(_repo);

            // Assert
            Assert.Throws<HttpResponseException>(() => controller.GetToDoById("1001"));
        }

        #endregion

        #region PUT

        [Test]
        public void Put_ControllerUpdate_Todo_Calls_RepositoryUpdate()
        {
            // Arrange
            var todo = new ToDo { Task = "Task here", Id = "111" };

            _repo
                .UpdateToDo(Arg.Any<string>(), Arg.Any<ToDo>())
                .Returns(true);

            var controller = new ToDosController(_repo);
            Setup_Controller_For_Tests(controller, HttpMethod.Put);

            // Act
            controller.UpdateToDo("111", todo);

            // Assert
            _repo.Received().UpdateToDo("111", todo);
        }

        [Test]
        public void Put_Failure_Returns_Correct_Status_Code_When_Repo_Returns_False()
        {
            // Arrange
            _repo.UpdateToDo(Arg.Any<string>(), Arg.Any<ToDo>())
                .Returns(false);

            var controller = new ToDosController(_repo);

            Setup_Controller_For_Tests(controller, HttpMethod.Post);

            // Act
            var response = controller.UpdateToDo("1", new ToDo(){Task = "task"});
            
            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region DELETE

        [Test]
        public void Delete_ToDo_Calls_Repo_Delete()
        {
            // Arrange
            var deletedId = "123";

            _repo.DeleteToDo(Arg.Any<string>())
                .Returns(true);

            var controller = new ToDosController(_repo);

            // Act
            controller.DeleteToDo("123");

            // Assert
            _repo.Received().DeleteToDo(deletedId);
        }

        [Test]
        public void Delete_ToDo_Returns_Correct_Status_Code()
        {
            // Arrange
            _repo
                .DeleteToDo(Arg.Any<string>())
                .Returns(true);

            var controller = new ToDosController(_repo);

            Setup_Controller_For_Tests(controller, HttpMethod.Delete);

            // Act
            var result = controller.DeleteToDo("123");

            // Assert
            Assert.IsInstanceOf<HttpResponseMessage>(result);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
        }

        #endregion

        #region POST

        [Test]
        public void Post_Success_Calls_Repository()
        {
            // Arrange
            ToDo addedToDo = new ToDo { Id = "99", Task = "Added task", Comment = "Sample comment here..." };

            _repo
                .AddToDo(Arg.Any<ToDo>())
                .Returns(addedToDo);

            var controller = new ToDosController(_repo);

            Setup_Controller_For_Tests(controller, HttpMethod.Post);

            // Act
            var response = controller.AddToDo(addedToDo);

            // Assert
            _repo.Received().AddToDo(addedToDo);
        }

        [Test]
        public void Post_Success_Returns_Correct_Status_Code()
        {
            // Arrange
            ToDo addedToDo = new ToDo { Id = "99", Task = "Added task", Comment = "Sample comment here..." };

            _repo
              .AddToDo(Arg.Any<ToDo>())
              .Returns(addedToDo);

            var controller = new ToDosController(_repo);

            Setup_Controller_For_Tests(controller, HttpMethod.Post);

            // Act
            var response = controller.AddToDo(addedToDo);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        }

        [Test]
        public void Post_Failure_Returns_Correct_Status_Code()
        {
            // Arrange 
            // Note : Task is Required
            ToDo addedToDo = new ToDo { Id = "99", Comment = "Sample comment here..." };

            // simulate exception
            _repo
                .AddToDo(addedToDo)
                .Returns(t => { throw new Exception(); });

            var controller = new ToDosController(_repo);

            Setup_Controller_For_Tests(controller, HttpMethod.Post);

            // Act
            var response = controller.AddToDo(addedToDo);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test]
        public void Post_Success_Returns_Correct_Uri()
        {
            // Arrange
            ToDo addedToDo = new ToDo { Id = "1234", Task = "Added task", Comment = "Sample comment here..." };

            _repo
              .AddToDo(Arg.Any<ToDo>())
              .Returns(addedToDo);

            var controller = new ToDosController(_repo);

            Setup_Controller_For_Tests(controller, HttpMethod.Post);

            // Act
            var response = controller.AddToDo(addedToDo);

            // Assert
            Assert.AreEqual("http://localhost/api/todos/1234", response.Headers.Location.ToString());
        }

        [Test]
        public void Post_Success_Returns_Json_Response()
        {
            // Arrange
            ToDo addedToDo = new ToDo { Id = "1234", Task = "Added task", Comment = "Sample comment here..." };

            _repo
              .AddToDo(Arg.Any<ToDo>())
              .Returns(addedToDo);

            var controller = new ToDosController(_repo);

            Setup_Controller_For_Tests(controller, HttpMethod.Post);

            // Act
            var response = controller.AddToDo(addedToDo);

            // Assert
            Assert.AreEqual(response.Content.Headers.ContentType.MediaType, "application/json");
        }

        #endregion

        private static void Setup_Controller_For_Tests(ApiController controller, HttpMethod httpMethod)
        {
            var config = new HttpConfiguration();
            //var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/todos");
            var request = new HttpRequestMessage(httpMethod, "http://localhost/api/todos");
            var route = config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
            var routeData = new HttpRouteData(route, new HttpRouteValueDictionary { { "controller", "todos" } });

            controller.ControllerContext = new HttpControllerContext(config, routeData, request);
            controller.Request = request;
            controller.Request.Properties[HttpPropertyKeys.HttpRouteDataKey] = routeData; // http://stackoverflow.com/questions/12246159/asp-net-web-api-url-link-not-returning-a-uristring-in-unit-test
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
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
