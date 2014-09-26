using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Newtonsoft.Json;
using ToDoApi.Models;
using ToDoApi.Repository;

namespace ToDoApi.Controllers
{
    public class ToDosController : ApiController
    {
        private readonly IToDoRepository _repo;

        public ToDosController(IToDoRepository repo)
        {
            _repo = repo;
        }

        // api/todos/
        public IEnumerable<ToDo> GetAllToDos()
        {
            return _repo.GetAllToDos();
        }

        // api/todos/1
        public ToDo GetToDoById(string id)
        {
            var todo = _repo.GetToDo(id);
            if (todo == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            return todo;
        }

        // api/todos/
        public HttpResponseMessage AddToDo(ToDo todo)
        {
            // Usage
            // http://stackoverflow.com/a/7173011
            // curl "http://localhost:56103/api/todos" -H "Content-Type: application/json" {} --data @vehicleDescriptors.json

            // Model state errors
            // $ curl -H "Content-Type: application/json" -d '{"tAask":"xyzTaskA","comment":"xyzCommentA","done":true}' http://localhost:56103/api/todos
            // Good request
            // $ curl -H "Content-Type: application/json" -d '{"task":"xyzTaskA","comment":"xyzCommentA","done":true}' http://localhost:56103/api/todos
            // Good request to node hosted api
            // $ curl -H "Content-Type: application/json" -d '{"task":"xyzTaskA","comment":"xyzCommentA","done":true}' http://localhost:8080/api/todos

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Model Errors");
            }

            try
            {
                var result = _repo.AddToDo(todo);

                //var response = Request.CreateResponse<ToDo>(HttpStatusCode.Created, todo); // use this if you want to return the newly created object
                var response  = Request.CreateResponse(HttpStatusCode.Created);

                // get link to newly created resource
                string uri = Url.Link("DefaultApi", new { id = result.Id });
                if (uri != null) response.Headers.Location = new Uri(uri);

                // create anonymous object to hold json response, (containing link to newly created resource) //http://stackoverflow.com/a/12240946
                var content = new { message = "Todo created", uri };

                response.Content = new StringContent(JsonConvert.SerializeObject(content, Formatting.Indented));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                
                return response;
            }
            catch (Exception ex)
            {
                // could not post, return error message
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        // api/todos/1
        public HttpResponseMessage UpdateToDo(string id, ToDo todo)
        {
            try
            {
                var result = _repo.UpdateToDo(id, todo);

                if (!result) { 
                    throw new Exception("Unable to update todo");
                }

                var response = Request.CreateResponse(HttpStatusCode.OK);

                // link to updated resource
                string uri = Url.Link("DefaultApi", new { id = id });
                if (uri != null) response.Headers.Location = new Uri(uri);

                // create anonymous object to hold json response, (containing link to updated resource) //http://stackoverflow.com/a/12240946
                var content = new { message = "Todo successfully updated", uri };

                response.Content = new StringContent(JsonConvert.SerializeObject(content, Formatting.Indented));
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                return response;
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
            }
        }

        // api/todos/1
        public HttpResponseMessage DeleteToDo(string id)
        {
            // Usage:
            // curl -X DELETE http://localhost:56103/api/todos/540c0a559bcc7b4034000002

            try
            {
                var result = _repo.DeleteToDo(id);

                if (!result)
                {
                    throw new Exception("Unable to delete todo");
                }

                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex.Message);
            }

        }

    }
}
