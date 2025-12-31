using API.Controllers.Commons;
using Application.DTOs.Commons;
using Application.DTOs.Identity;
using Application.Interfaces.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseApiController
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("Create")]
        [Authorize(Policy = "WriteAccount")]
        public async Task<IActionResult> Create([FromBody] CreateAccountDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _accountService.CreateAsync(dto);
            return HandleServiceResult(result);
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Policy = "DeleteAccount")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _accountService.Delete(id);
            return HandleServiceResult(result);
        }

        [HttpGet("GetList")]
        [Authorize(Policy = "ReadAccount")]
        public IActionResult GetListAsync([FromQuery] CommonQueryParameters parameters)
        {
            var result = _accountService.GetList(parameters);
            return HandleServiceResult(result);
        }

    }
}
