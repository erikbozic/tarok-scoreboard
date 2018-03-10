namespace TarokScoreBoard.Shared.DTO
{
  public class ResponseDTO
  {
    public static ResponseDTO<T> Create<T>(T data, string message = "", object errors = null)
    {
      return new ResponseDTO<T>(data, message, errors);
    }
  }

  public class ResponseDTO<T>
  {
    public ResponseDTO(T data, string message = "", object errors = null)
    {
      Data = data;
      Message = message;
      Errors = errors;
    }

    public T Data { get; private set; }

    public string Message { get; private set; }

    public object Errors { get; private set; }
  }
}
