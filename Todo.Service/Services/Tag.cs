using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Todo.Database;
using Todo.Database.Entity;
using Todo.Service.Models;

namespace Todo.Service.Services
{
    public interface ITags
    {
        Task<AddTagResult> AddTag(AddTagModel model);

        Task<GetTagByIdResult> GetTagById(long id);

        Task<GetTagByNameResult> GetTagByName(string name);

        Task<EditTagResult> EditTag(long id);
    }
    public class Tags : ITags
    {
        private readonly DatabaseContext _dbContext;

        public Tags(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AddTagResult> AddTag(AddTagModel model)
        {
            var result = new AddTagResult();

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
                var tagEntity = new TagEntity();
                tagEntity.Tag = model.Tag;
                tagEntity.Usage = 0;

                _dbContext.Tags.Add(tagEntity);
                await _dbContext.SaveChangesAsync();

                result.Success = true;
                result.Tag = tagEntity;
            }
            catch (Exception e)
            {
                result.Error.Append(e.Message);
            }

            return result;
        }

        public async  Task<GetTagByIdResult> GetTagById(long id)
        {
            var result = new GetTagByIdResult();

            try
            {
                var tagEntity =await  _dbContext.Tags.FirstOrDefaultAsync(t =>t.Id == id);
                if (tagEntity == null)
                {
                    result.NotFound = true;
                    return result;
                }

                result.Success = true;
                result.Tag = tagEntity;
            }
            catch (Exception e)
            {
                result.Error.Append(e.Message);
            }

            return result;
        }

        public async Task<GetTagByNameResult> GetTagByName(string name)
        {
            var result = new GetTagByNameResult();

            try
            {
                var tag = await _dbContext.Tags.FirstOrDefaultAsync(x => x.Tag.Equals(name));

                if (tag == null)
                {
                    result.NotFound = true;
                    return result;
                }

                result.Success = true;
                result.Tag = tag;
            }
            catch (Exception e)
            {
                result.Error.Append(e.Message);
            }

            return result;
        }

        public async Task<EditTagResult> EditTag(long id)
        {
            var result = new EditTagResult();

            try
            {
                var tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.Id.Equals(id));

                if (tag == null)
                {
                    result.NotFound = true;
                    return result;
                }

                tag.Usage++;
                _dbContext.Tags.Update(tag);
                await _dbContext.SaveChangesAsync();

                result.Success = true;
                result.Tag = tag;
            }
            catch (Exception e)
            {
                result.Error.Append(e.Message);
            }

            return result;
        }
    }    
}

