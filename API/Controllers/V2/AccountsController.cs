using API.Controllers.Commons;
using Application.DTOs.Commons;
using Application.DTOs.Identity;
using Application.Interfaces.Services.Identity;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class AccountsController : BaseApiController
    {
        private readonly IAccountService _accountService;
        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AccountDto>), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [Authorize(Policy = "WriteAccount")]
        public async Task<IActionResult> Create([FromBody] CreateAccountDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _accountService.CreateAsync(dto);
            return HandleServiceResult(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "DeleteAccount")]
        [ProducesResponseType(typeof(ApiResponse<AccountDto>), 204)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _accountService.Delete(id);
            return HandleServiceResult(result);
        }

        [HttpGet]
        [Authorize(Policy = "ReadAccount")]
        public async Task<IActionResult> GetListAsync([FromQuery] CommonQueryParameters parameters)
        {
            var result = await _accountService.GetListAsync(parameters);
            return HandleServiceResult(result);
        }

    }
}
