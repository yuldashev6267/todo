using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Todo.Database;
using Todo.Database.Entity;
using Todo.Service.Models;

namespace Todo.Service.Services
{
    public interface ITodo
    {
        Task<AddTodoResult> AddTodo(AddTodoModel model);
        Task<GetTodoResult> GetTodo(long id);
        Task<DeleteTodoResult> DeleteTodo(long id);
        Task<GetAllResult> GetAll(GetAllModel model);
        Task<GetAllCountResult> GetCount(GetAllCountModel model); 
        Task<SearchTodoResult> SearchTodo(string text);
        Task<EditTodoResult> EditTodo(EditTodoModel model);
        Task<CompletedTodoResult> CompletedTodo(long id);
    }

    public class TodoService : ITodo
    {
        private readonly DatabaseContext _dbContext;

        private readonly ITags _tags;

        public TodoService(DatabaseContext dbContext, ITags tags)
        {
            _dbContext = dbContext;
            _tags = tags;
        }

        public async Task<AddTodoResult> AddTodo(AddTodoModel model)
        {
            var result = new AddTodoResult();

            #region validate request model

            {
                var context = new ValidationContext(model, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();

                var isValid = Validator.TryValidateObject(model, context, validationResults, true);

                if (!isValid)
                {
                    foreach (var validationResult in validationResults)
                    {
                        result.Error.AppendJoin(";", validationResult.ErrorMessage);
                    }

                    return result;
                }
            }

            #endregion

            try
            {
                #region MyRegion

                var todoEntity = new TodoEntity();
                {
                    todoEntity.CreatedAt = DateTime.Now;
                    todoEntity.UpdatedAt = DateTime.Now;
                    todoEntity.IsDeleted = false;
                    todoEntity.IsCompleted = false;
                    todoEntity.Title = model.Title;
                    todoEntity.Description = model.Description;

                    if (model.Colour.HasValue)
                    {
                        todoEntity.Colour = model.Colour.Value;
                    }

                    if (model.Priority.HasValue)
                    {
                        todoEntity.Priority = model.Priority.Value;
                    }

                    if (model.Tags.Length > 0)
                    {
                        todoEntity.Tags = await Tags(model.Tags);
                    }
                }

                _dbContext.Todos.Add(todoEntity);
                await _dbContext.SaveChangesAsync();

                #endregion

                result.Success = true;
                result.Todo = todoEntity;
            }
            catch (Exception e)
            {
                result.Error.Append(e.Message);
            }

            return result;
        }

        public async Task<GetTodoResult> GetTodo(long id)
        {
            var result = new GetTodoResult();

            try
            {
                var todo = await _dbContext.Todos.Where(t => !t.IsDeleted && t.Id == id).Include(t => t.Tags)
                    .FirstOrDefaultAsync();
                if (todo == null)
                {
                    result.NotFound = true;
                    return result;
                }

                result.Success = true;
                result.Todo = todo;
            }
            catch (Exception e)
            {
                result.Error.Append(e.Message);
            }

            return result;
        }

        public async Task<DeleteTodoResult> DeleteTodo(long id)
        {
            var result = new DeleteTodoResult();

            try
            {
                var todo = await _dbContext.Todos.Where(t => t.Id == id && !t.IsDeleted).Include(t => t.Tags)
                    .FirstOrDefaultAsync();

                if (todo == null)
                {
                    result.NotFound = true;
                    return result;
                }

                todo.UpdatedAt = DateTime.Now;
                todo.IsDeleted = true;
                todo.DeletedAt = DateTime.Now;

                _dbContext.Todos.Update(todo);
                await _dbContext.SaveChangesAsync();

                result.Todo = todo;
                result.Success = true;
            }
            catch (Exception e)
            {
                result.Error.Append(e.Message);
            }

            return result;
        }

        public async Task<EditTodoResult> EditTodo(EditTodoModel model)
        {
            var result = new EditTodoResult();

            #region validate request model

            {
                var context = new ValidationContext(model, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();

                var isValid = Validator.TryValidateObject(model, context, validationResults, true);

                if (!isValid)
                {
                    foreach (var validationResult in validationResults)
                    {
                        result.Error.AppendJoin(";", validationResult.ErrorMessage);
                    }

                    return result;
                }
            }

            #endregion

            try
            {
                var todo = await _dbContext.Todos.Where(t => t.Id.Equals(model.Id)).Include(t=>t.Tags).FirstOrDefaultAsync();

                if (todo == null)
                    result.NotFound = true;

                #region tag
                
                // if tags length greater than actual todos tags length 
                if (model.Tags.Length > todo.Tags.Count)
                {
                    for (var i = 0; i < model.Tags.Length; i++)
                    {
                        var isConsist = todo.Tags.Any(t => t.Tag.Equals(model.Tags[i]));

                        if (!isConsist)
                        {
                            var tag = await _tags.AddTag(new AddTagModel()
                            {
                                Tag = model.Tags[i]
                            });
                            await _tags.EditTag(tag.Tag.Id);
                            todo.Tags.Add(tag.Tag);
                        }
                    }
                }
                
                // tag is removed
                if (model.Tags.Length < todo.Tags.Count())
                {
                    foreach (var tag in todo.Tags)
                    {
                        var isConsist = model.Tags.Any(t=>t.Equals(tag.Tag));

                        if (!isConsist)
                        {
                            todo.Tags.Remove(tag);
                        } 
                    }
                }
                #endregion
                
                todo.UpdatedAt = DateTime.Now;
                todo.Title = model.Title;
                todo.Description = model.Description;

                if (model.Colour.HasValue)
                    todo.Colour = model.Colour.Value;

                if (model.Priority.HasValue)
                    todo.Priority = model.Priority.Value;

                _dbContext.Todos.Update(todo);
                await _dbContext.SaveChangesAsync();

                result.Success = true;
                result.Todo = todo;
            }
            catch (Exception e)
            {
                result.Error.Append(e.Message);
            }

            return result;
        }

        public async Task<GetAllResult> GetAll(GetAllModel model)
        {
            var result = new GetAllResult();

            try
            {
                var query = _dbContext.Todos.Where(x => !x.IsDeleted).Include(t=>t.Tags).AsQueryable();

                if (model.Priority.HasValue)
                    query = query.Where(t => t.Priority.Equals(model.Priority.Value));

                if (model.Colour.HasValue)
                    query = query.Where(t => t.Colour.Equals(model.Colour.Value));

                if (!string.IsNullOrEmpty(model.Tag))
                    query = query.Where(t => t.Tags.Any(tt => tt.Tag.Equals(model.Tag)));


                if (model.Skip.HasValue)
                    query = query.Skip(Math.Max(0, model.Skip.Value));

                if (model.Limit.HasValue)
                    query = query.Take(Math.Max(1, model.Limit.Value));

                result.Success = true;
                result.Todos = await query.AsNoTracking().ToListAsync();
            }
            catch (Exception e)
            {
                result.Error.Append(e.Message);
            }

            return result;
        }

        public async Task<GetAllCountResult> GetCount(GetAllCountModel model)
        {
            var result = new GetAllCountResult();

            try
            {
                var query = _dbContext.Todos.Where(x => !x.IsDeleted).AsQueryable();

                if (model.Priority.HasValue)
                    query = query.Where(t => t.Priority.Equals(model.Priority.Value));

                if (model.Colour.HasValue)
                    query = query.Where(t => t.Colour.Equals(model.Colour.Value));

                if (!string.IsNullOrEmpty(model.Tag))
                    query = query.Where(t => t.Tags.Any(tt => tt.Tag.Equals(model.Tag)));


                result.Success = true;
                result.Count = await query.CountAsync();
            }
            catch (Exception e)
            {
                result.Error.Append(e.Message);
            }

            return result;
        }
        
        public async Task<SearchTodoResult> SearchTodo(string text)
        {
            var result = new SearchTodoResult();

            try
            {
                var todos = await _dbContext.Todos
                    .Where(t => (t.Description.Contains(text) || t.Title.Contains(text)) && !t.IsDeleted).Include(t=>t.Tags)
                    .Take(10).ToListAsync();

                result.Success = true;
                result.Todos = todos;
            }
            catch (Exception e)
            {
                result.Error.Append(e.Message);
            }

            return result;
        }

        public async Task<CompletedTodoResult> CompletedTodo(long id)
        {
            var result = new CompletedTodoResult();

            try
            {
                var todo = await _dbContext.Todos.Where(t => t.Id == id && !t.IsDeleted).Include(t=>t.Tags).FirstOrDefaultAsync();

                if (todo == null)
                {
                    result.NotFound = true;
                    return result;
                }

                todo.UpdatedAt = DateTime.Now;
                todo.IsCompleted = true;

                _dbContext.Todos.Update(todo);
                await _dbContext.SaveChangesAsync();

                result.Todo = todo;
                result.Success = true;
            }
            catch (Exception e)
            {
                result.Error.Append(e.Message);
            }

            return result;
        }

        private async Task<List<TagEntity>> Tags(string[] tags)
        {
            List<TagEntity> tagsList = new List<TagEntity>();

            for (var i = 0; i < tags.Length; i++)
            {
                var tag = await _tags.GetTagByName(tags[i]);
                if (tag.Success)
                {
                    await _tags.EditTag(tag.Tag.Id);
                    tagsList.Add(tag.Tag);
                }
                else if (tag.NotFound)
                {
                    var newTag = await _tags.AddTag(new AddTagModel()
                    {
                        Tag = tags[i]
                    });
                    await _tags.EditTag(newTag.Tag.Id);
                    tagsList.Add(newTag.Tag);
                }
            }

            return tagsList;
        }
    }
}