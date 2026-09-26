using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Vehicles;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.Controllers {
    [ApiController]
    [Route("api/vehicles")]
    [Produces("application/json")]
    public sealed class VehiclesController : ControllerBase {
        private readonly IMediator _mediator;

        public VehiclesController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Da de alta un vehículo en la flota. Rechaza el alta si el vehículo tiene más de 5 años de antigüedad según su año de fabricación.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleCommand command) {
            var vehicle = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, vehicle);
        }

        /// <summary>
        /// Lista todos los vehículos de la flota con sus datos, indicando cuáles están disponibles y cuáles están alquilados junto con la persona que los tiene.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<VehicleResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListVehicles() => Ok(await _mediator.Send(new ListVehiclesQuery()));

        /// <summary>
        /// Alquila un vehículo disponible y lo asocia a una persona. Rechaza la operación si la persona ya tiene otro vehículo alquilado o si el vehículo no está disponible.
        /// </summary>
        [HttpPost("{vehicleId:guid}/rent")]
        [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RentVehicle(Guid vehicleId, [FromBody] RentVehicleCommand command) {
            ArgumentNullException.ThrowIfNull(command);
            command.VehicleId = vehicleId;
            return Ok(await _mediator.Send(command));
        }

        /// <summary>
        /// Devuelve un vehículo a la flota y lo desasocia de la persona que lo tenía alquilado.
        /// </summary>
        [HttpPost("{vehicleId:guid}/return")]
        [ProducesResponseType(typeof(VehicleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReturnVehicle(Guid vehicleId) => Ok(await _mediator.Send(new ReturnVehicleCommand { VehicleId = vehicleId }));
    }
}
