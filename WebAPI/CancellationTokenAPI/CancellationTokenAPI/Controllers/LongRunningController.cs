using Microsoft.AspNetCore.Mvc;

namespace CancellationTokenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LongRunningController : Controller
    {
        private readonly ILogger<LongRunningController> _logger;
        public LongRunningController(ILogger<LongRunningController> logger)
        {
            _logger = logger;
        }

        [HttpGet("longoperation")]
        public async Task<IResult> LognOperation(CancellationToken ct = default)
        {
            var compleated = await LoginOperationAsync(ct);

            if (compleated)
                return Results.StatusCode(499);
            
            return compleated ? Results.Ok() : Results.StatusCode(500);
        }

        async Task<bool> LoginOperationAsync (CancellationToken cancellationToken)
        {
            for (int i = 0; i < 10; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogError("trigger CancellationRequested");
                    return false;
                }

                await Task.Delay(3 * 1000);
            }

            return true;
        }
    }
}
