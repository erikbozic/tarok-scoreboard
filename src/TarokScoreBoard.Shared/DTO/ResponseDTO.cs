namespace TarokScoreBoard.Shared.DTO
{
  public class ResponseDTO
  {
    public static ResponseDTO<T> Create<T>(T data, string message = "")
    {
      return new ResponseDTO<T>(data, message);
    }
  }

  public class ResponseDTO<T>
  {
    public ResponseDTO(T data, string message = "")
    {
      Data = data;
      Message = message;
    }

    public T Data { get; set; }

    public string Message { get; private set; }
  }
}
