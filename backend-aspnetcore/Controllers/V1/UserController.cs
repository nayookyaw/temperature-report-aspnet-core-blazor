using BackendAspNetCore.Dtos.Response;
using BackendAspNetCore.RequestBody.User;
using BackendAspNetCore.Services.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace BackendAspNetCore.Controllers.V1;

[ApiController]
[ApiVersion("2.0")]
[Route("v{version:apiVersion}/user")]
public class UserController(IUserService iUserService) : ControllerBase
{
    private readonly IUserService _iUserService = iUserService;

    [HttpGet]
    public async Task<IActionResult> GetAllUser()
    {
        var response = await _iUserService.GetAllUser();
        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] AddUserRequestBody input)
    {
        var response = await _iUserService.AddUser(input);
        return StatusCode(response.StatusCode, response);
    }

    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody]UpdateUserRequestBody input)
    {
        var response = await _iUserService.UpdateUser(userId,input);
        return StatusCode(response.StatusCode, response);
    }
}