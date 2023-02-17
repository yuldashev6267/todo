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
            Task<RemoveTagFromTodoResult> RemoveTagFromTodo(RemoveTagFromTodoModel model);                                                                                                       
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

                        if (model.TagIds.Length > 0)
                        {
                            todoEntity.Tags = Tags(model.TagIds);
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
                    var todo = await _dbContext.Todos.FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == id);
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
                    var todo = _dbContext.Todos.FirstOrDefault(t => t.Id == id && !t.IsDeleted);

                    if (todo == null)
                    {
                        result.NotFound = true;
                        return result;
                    }

                    todo.UpdatedAt = DateTime.Now;
                    todo.IsDeleted = true;
                    todo.DeletedAt = DateTime.Now;

                    _dbContext.Todos.Update(todo);
                     await  _dbContext.SaveChangesAsync();
                    
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
                    var todo = await _dbContext.Todos.Where(t => t.Id.Equals(model.Id)).FirstOrDefaultAsync();

                    if (todo == null)
                        result.NotFound = true;


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
                    var query = _dbContext.Todos.Where(x=>!x.IsDeleted).AsQueryable();

                    if (model.Priority.HasValue)
                        query = query.Where(t => t.Priority.Equals(model.Priority.Value));

                    if (model.Colour.HasValue)
                        query = query.Where(t => t.Colour.Equals(model.Colour.Value));
                    
                    if (!string.IsNullOrEmpty(model.Tag)) 
                        query = query.Where(t=>t.Tags.Any(tt=>tt.Tag.Equals(model.Tag)));
                    
                    
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
                    var query = _dbContext.Todos.Where(x=>!x.IsDeleted).AsQueryable();

                    if (model.Priority.HasValue)
                        query = query.Where(t => t.Priority.Equals(model.Priority.Value));

                    if (model.Colour.HasValue)
                        query = query.Where(t => t.Colour.Equals(model.Colour.Value));
                    
                    if (!string.IsNullOrEmpty(model.Tag)) 
                        query = query.Where(t=>t.Tags.Any(tt=>tt.Tag.Equals(model.Tag)));


                    result.Success = true;
                    result.Count = await query.CountAsync();
                }
                catch (Exception e)
                {
                    result.Error.Append(e.Message);
                }

                return result;
            }

            public async Task<RemoveTagFromTodoResult> RemoveTagFromTodo(RemoveTagFromTodoModel model)
            {
                var result = new RemoveTagFromTodoResult();
                
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
                    var todo = _dbContext.Todos.Find(model.TodoID);

                    if (todo == null)
                    {
                        result.NotFound = true;
                        return result;
                    }

                    var tag = todo.Tags.FirstOrDefault(t => t.Id.Equals(model.TagID));
                    todo.Tags.Remove(tag);
                    
                    _dbContext.Todos.Update(todo);
                    _dbContext.SaveChanges();

                    result.Success = true;
                    result.Todo = todo;

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
                    var todos = await _dbContext.Todos.Where(t => (t.Description.Contains(text) || t.Title.Contains(text)) && !t.IsDeleted)
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
                    var todo =await _dbContext.Todos.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

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

            private List<TagEntity> Tags(long[] ids)
            {
                List<TagEntity> tags =  new List<TagEntity>();
                
                for (var i = 0; i < ids.Length; i++)
                {
                    var tag = _tags.GetTagById(ids[i]);
                    if (tag.Success)
                    {
                        tags.Add(tag.Tag);
                    }
                }

                return tags;
            }
        }
}    


