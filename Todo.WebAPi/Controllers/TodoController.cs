using System.Text.Json.Serialization;
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
        public async Task<ActionResult> Create(CreateTodoRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _todo.AddTodo(new AddTodoModel()
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
        public async Task<ActionResult> Get(long id)
        {
            var result = await _todo.GetTodo(id);

            if (result.Success)
                return Ok(TodoContract.ConvertEntityToContract(result.Todo));

            if (result.NotFound)
                return NotFound();

            return Problem(result.Error.ToString());
        }

        [HttpPost]
        [Route("api/delete/{id:long}")]
        public async Task<ActionResult> Delete(long id)
        {
            var result = await _todo.DeleteTodo(id);

            if (result.Success)
                return Ok(TodoContract.ConvertEntityToContract(result.Todo));

            return Problem(result.Error.ToString());
        }

        [HttpGet]
        [Route("api/get/all")]
        public async Task<IActionResult> Get([FromQuery] GetAllRequestModel model)
        {
            var result = await _todo.GetAll(new GetAllModel()
            {
                Desc = model.Desc,
                Tag = model.Tag,
                Priority = model.Priority,
                Colour = model.Colour,
                Limit = model.Limit,
                Skip = model.Skip
            });

            if (result.Success)
                return Ok(result.Todos.Select(TodoContract.ConvertEntityToContract));

            return Problem(result.Error.ToString());
        }

        [HttpGet]
        [Route("api/get/count")]
        public async Task<IActionResult> GetCount([FromQuery] GetAllRequestModel model)
        {
            var result = await _todo.GetCount(new GetAllCountModel()
            {
                Desc = model.Desc,
                Tag = model.Tag,
                Priority = model.Priority,
                Colour = model.Colour,
            });

            if (result.Success)
                return Ok(result.Count);

            return Problem(result.Error.ToString());
        }
        
        [HttpGet]
        [Route("api/get")]
        public async Task<ActionResult<List<TodoContract>>> Get([FromQuery] string searchText)
        {
            var result = await _todo.SearchTodo(searchText);

            if (result.Success)
                return Ok(result.Todos.Select(TodoContract.ConvertEntityToContract));

            return Problem(result.Error.ToString());
        }

        [HttpPost]
        [Route("api/edit")]
        public async Task<ActionResult> Edit(EditTodoRequestModel requestModel)
        {
            var result = await _todo.EditTodo(new EditTodoModel()
            {
                Id = requestModel.Id,
                Title = requestModel.Title,
                Description = requestModel.Description,
                Colour = requestModel.Colour.HasValue ? requestModel.Colour.Value : null,
                Priority = requestModel.Priority.HasValue ? requestModel.Priority.Value : null
            });

            if (result.Success)
                return Ok(TodoContract.ConvertEntityToContract(result.Todo));

            if (result.NotFound)
                return NotFound();

            return Problem(result.Error.ToString());
        }

        [HttpPost]
        [Route("api/complete/{id:long}")]
        public async Task<ActionResult> Complete(long id)
        {
            var result = await _todo.CompletedTodo(id);

            if (result.Success)
                return Ok(TodoContract.ConvertEntityToContract(result.Todo));

            if (result.NotFound)
                return NotFound();

            return Problem(result.Error.ToString());
        }
    }
}