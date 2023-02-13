using System.Text.Json.Serialization;
using Todo.Database.Entity;

namespace Todo.WebAPi.Contracts;

public class TagContract
{
    [JsonPropertyName("id")]
    public long Id { get; set; }
    
    [JsonPropertyName("tag")]
    public string Tag { get; set; }
    
    [JsonPropertyName("usage")]
    public int Usage { get; set; }

    public static TagContract TagContractFromEntity(TagEntity entity)
    {
        var tagContract = new TagContract();
       
        tagContract.Id = entity.Id;
        tagContract.Tag = entity.Tag;
        tagContract.Usage = entity.Usage;

        return tagContract;
    }
}