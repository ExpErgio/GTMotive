using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GtMotive.Estimate.Microservice.ApplicationCore.Persons;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GtMotive.Estimate.Microservice.Api.Controllers {
    [ApiController]
    [Route("api/persons")]
    [Produces("application/json")]
    public sealed class PersonsController : ControllerBase {
        private readonly IMediator _mediator;

        public PersonsController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Da de alta una persona, que es quien podrá alquilar vehículos de la flota.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePerson([FromBody] CreatePersonCommand command) {
            var person = await _mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created, person);
        }

        /// <summary>
        /// Lista todas las personas dadas de alta con sus datos.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<PersonResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListPersons() => Ok(await _mediator.Send(new ListPersonsQuery()));

        /// <summary>
        /// Elimina una persona. Rechaza la operación si la persona tiene un vehículo alquilado sin devolver.
        /// </summary>
        [HttpDelete("{personId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePerson(Guid personId) {
            await _mediator.Send(new DeletePersonCommand { PersonId = personId });
            return NoContent();
        }
    }
}
