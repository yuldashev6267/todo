using System.ComponentModel.DataAnnotations;
using Todo.Database;
using Todo.Database.Entity;
using Todo.Service.Models;

namespace Todo.Service.Services
{
    public interface ITags
    {
        AddTagResult AddTag(AddTagModel model);

        GetTagByIdResult GetTagById(long id);
    }
    
    public class Tags : ITags
    {
        private readonly DatabaseContext _dbContext;

        public Tags(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public AddTagResult AddTag(AddTagModel model)
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
                tagEntity.Usage = 1;

                _dbContext.Tags.Add(tagEntity);
                _dbContext.SaveChanges();

                result.Success = true;
                result.Tag = tagEntity;
            }
            catch (Exception e)
            {
                result.Error.Append(e.Message);
            }

            return result;
        }

        public GetTagByIdResult GetTagById(long id)
        {
            var result = new GetTagByIdResult();

            try
            {
                var tagEntity = _dbContext.Tags.FirstOrDefault(t =>t.Id == id);
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
    }    
}

