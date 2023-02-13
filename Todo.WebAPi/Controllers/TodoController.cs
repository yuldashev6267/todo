using Microsoft.AspNetCore.Mvc;
using Todo.Core;
using Todo.Service.Models;
using Todo.Service.Services;
using Todo.WebAPi.Contracts;
using Todo.WebAPi.RequestModels;

namespace Todo.WebAPi.Controllers
{
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodo _todo;

        public TodoController(ITodo todo)
        {
            _todo = todo;
        }

        [HttpPost]
        [Route("api/create")]
        public ActionResult Create(CreateTodoRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var result = _todo.AddTodo(new AddTodoModel()
            {
                Title = model.Title,
                Description = model.Description,
                Colour = model.Colour.HasValue ? model.Colour.Value : null,
                Priority = model.Priority.HasValue ? model.Priority.Value : null,
                TagIds = model.TagIds
            });

            if (result.Success)
                return Ok(TodoContract.ConvertEntityToContract(result.Todo));

            return Problem(result.Error.ToString());

        }
        
        [HttpGet]
        [Route("api/get/{id:long}")]
        public ActionResult Get(long id)
        {
            var result = _todo.GetTodo(id);

            if (result.Success)
                return Ok(TodoContract.ConvertEntityToContract(result.Todo));

            if (result.NotFound)
                return NotFound();

            return Problem(result.Error.ToString());

        }
        
        [HttpPost]
        [Route("api/delete/{id:long}")]
        public ActionResult Delete(long id)
        {
            var result = _todo.DeleteTodo(id);

            if (result.Success)
                return Ok(TodoContract.ConvertEntityToContract(result.Todo));

            return Problem(result.Error.ToString());

        }

        [HttpGet]
        [Route("api/get/all")]
        public ActionResult<List<TodoContract>> Get([FromQuery]GetAllRequestModel model)
        {
            var result = _todo.GetAll(new GetAllModel()
            {
                Desc = model.Desc,
                Tag = model.Tag,
                Priority = model.Priority,
                Limit = model.Limit,
                Skip =model.Skip
            });

            if (result.Success)
                return Ok(result.Todos.Select( TodoContract.ConvertEntityToContract));

            return Problem(result.Error.ToString());
        }

        [HttpGet]
        [Route("api/get")]
        public ActionResult<List<TodoContract>> Get([FromQuery] string searchText)
        {
            var result = _todo.SearchTodo(searchText);

            if (result.Success)
                return Ok(result.Todos.Select(TodoContract.ConvertEntityToContract));

            return Problem(result.Error.ToString());
        }
        
        [HttpPost]
        [Route("api/edit")]
        public ActionResult Edit(EditTodoRequestModel requestModel)
        {
            var result = _todo.EditTodo(new EditTodoModel()
            {
                Id = requestModel.Id,
                Title = requestModel.Title,
                Description = requestModel.Description,
                Colour = requestModel.Colour.HasValue ? requestModel.Colour.Value : null,
                Priority = requestModel.Priority.HasValue ? requestModel.Priority.Value : null
            });

            if (result.Success)
                return Ok(TodoContract.ConvertEntityToContract(result.Todo));

            return Problem(result.Error.ToString());
        }
    }    
}
