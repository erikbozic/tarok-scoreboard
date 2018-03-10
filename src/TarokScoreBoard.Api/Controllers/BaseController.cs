using Microsoft.AspNetCore.Mvc;
using TarokScoreBoard.Shared.DTO;

namespace TarokScoreBoard.Api.Controllers
{

  [ApiController()]
  public abstract class BaseController : ControllerBase
  {
    public override OkObjectResult Ok(object value) => base.Ok(ResponseDTO.Create(value));
  }
}
