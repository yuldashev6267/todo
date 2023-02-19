using Todo.Database.Entity;

namespace Todo.Tests.Mock;

public class TagMockData
{
     public static List<TagEntity> Tags()
     {
          return new List<TagEntity>()
          {
               new TagEntity()
               {
                    Id = 1,
                    Tag = "this is test tag",
                    Usage = 1,
               },
               new TagEntity()
               {
                    Id = 2,
                    Tag = "this is test tag 1",
                    Usage = 1,
               },
               new TagEntity()
               {
                    Id = 3,
                    Tag = "this is test tag 2",
                    Usage = 10,
               }
               
          };
     }
}