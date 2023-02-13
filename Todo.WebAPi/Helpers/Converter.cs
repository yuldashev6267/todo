namespace Todo.WebAPi.Helpers;

public static class Converter
{
    public static string ConvertToDateTime(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd hh:mm:ss");
    }   
}