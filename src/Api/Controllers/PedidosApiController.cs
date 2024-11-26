using Controllers;
using Core.Domain.Notificacoes;
using Core.WebApi.Controller;
using Gateways.Dtos.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize(Policy = "AdminRole")]
    [Route("producao")]
    public class PedidosApiController(IPedidoController pedidoController, INotificador notificador) : MainController(notificador)
    {
        [HttpPatch("status/{pedidoId:guid}")]
        public async Task<IActionResult> AlterarStatus([FromRoute] Guid pedidoId, [FromBody] PedidoStatusRequestDto pedidoStatusDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ErrorBadRequestModelState(ModelState);
            }

            try
            {
                var result = await pedidoController.AlterarStatusAsync(pedidoId, pedidoStatusDto, cancellationToken);

                if (!result)
                {
                    return BadRequest(new BaseApiResponse { Success = false, Errors = new List<string> { "Status inválido." } });
                }

                return CustomResponsePutPatch(pedidoStatusDto, result);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) if necessary
                return StatusCode(500, new BaseApiResponse { Success = false, Errors = new List<string> { "Ocorreu um erro ao processar a solicitação.", ex.Message } });
            }
        }
    }
}