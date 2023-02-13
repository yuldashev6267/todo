using Microsoft.AspNetCore.Mvc;
using Todo.Service.Models;
using Todo.Service.Services;
using Todo.WebAPi.RequestModels;

namespace Todo.WebAPi.Controllers
{
    [ApiController]
    public class TagController : ControllerBase
    {
        protected readonly ITags _tags;

        public TagController(ITags tags)
        {
            _tags = tags;
        }

        [HttpPost]
        [Route("api/create/tag")]
        public ActionResult Create(CreateTagRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var result = _tags.AddTag(new AddTagModel()
            {
                Tag = model.Tag
            });

            if (result.Success)
                return Ok(result.Tag);

            return Problem(result.Error.ToString());
            
        }
    }    
}

