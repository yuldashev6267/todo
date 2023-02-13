using System.Text;

namespace Todo.Service.Models
{
    public class BaseResultModel
    {
           public bool Success { get; set; }
           
           public StringBuilder Error { get; set; }

           public void AppendError(String error)
           {
               Error ??= new StringBuilder();
               Error.AppendFormat(error, ";");
           }
    }    
}
