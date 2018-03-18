namespace TarokScoreBoard.Shared.DTO
{
  public class ResponseDTO
  {
    public static ResponseDTO<T> Create<T>(T data, string message = "", object errors = null) where T : class
    {
      return new ResponseDTO<T>(data, message, errors);
    }
  }

  public class ResponseDTO<T> where T : class
  {
    public ResponseDTO()
    {

    }
    public ResponseDTO(T data, string message = "", object errors = null)
    {
      Data = data;
      Message = message;
      Errors = errors;
    }

    public T Data { get;  set; }

    public string Message { get;  set; }

    public object Errors { get;  set; }
  }
}
