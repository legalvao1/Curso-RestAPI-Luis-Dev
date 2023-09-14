using AwsomeDevEvents.API.Entities;
using AwsomeDevEvents.API.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AwsomeDevEvents.API.Controllers
{
    [Route("api/dev-events")]
    [ApiController]
    public class DevEventsController : ControllerBase
    {
        private readonly DevEventsDbContext _context;
        public DevEventsController(DevEventsDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obter todos os eventos
        /// </summary>
        /// <returns>Coleção de eventos</returns>
        /// <response code="200">Sucesso</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAll()
        {
            var devEvents = _context.DevEvents.Where(d => !d.IsDeleted).ToList();
            return Ok(devEvents);
        }

        /// <summary>
        /// Obter um evento
        /// </summary>
        /// <param name="id">Identificador do evento</param>
        /// <returns>Dados do evento</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(Guid id)
        {
            var devEvent = _context.DevEvents
                .Include(d => d.Speakers)
                .SingleOrDefault(d => d.Id.Equals(id));
            return devEvent is null ? NotFound() : Ok(devEvent);
        }

        /// <summary>
        /// Cadastrar um evento
        /// </summary>
        /// <remarks>
        /// {"title":"string","description":"string","startDate":"2023-09-14T16:30:14.860Z","endDate":"2023-09-14T16:30:14.860Z"}
        /// </remarks>
        /// <param name="devEvent">Dados do evento</param>
        /// <returns>Objeto recém criado</returns>
        /// <response code="201">Sucesso</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult Post(DevEvent devEvent)
        {
            _context.DevEvents.Add(devEvent);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = devEvent.Id }, devEvent);
        }

        /// <summary>
        /// Atualizar um evento
        /// </summary>
        /// <remarks>
        /// {"id":"3fa85f64-5717-4562-b3fc-2c963f66afa6","title":"string","description":"string","startDate":"2023-09-14T16:30:14.860Z","endDate":"2023-09-14T16:30:14.860Z"}
        /// </remarks>
        /// <param name="id">Identificador do evento</param>
        /// <param name="input">Dados do evento</param>
        /// <returns>Nada</returns>
        /// <response code="204">Sucesso</response>
        /// <response code="404">Não encontrado</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update(Guid id, DevEvent input)
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id.Equals(id));

            if (devEvent is null)
            {
                return NotFound();
            }
            devEvent.Update(input.Title, input.Description, input.StartDate, input.EndDate);

            _context.DevEvents.Update(devEvent);
            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Deletar um evento
        /// </summary>
        /// <param name="id">Identificador do evento</param>
        /// <returns>Nada</returns>
        /// <response code="204">Sucesso</response>
        /// <response code="404">Não encontrado</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(Guid id)
        {
            var devEvent = _context.DevEvents.SingleOrDefault(d => d.Id.Equals(id));

            if (devEvent is null)
            {
                return NotFound();
            }
            devEvent.Delete();

            _context.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Cadastrar um palestrante
        /// </summary>
        /// <remarks>
        /// {"id":"3fa85f64-5717-4562-b3fc-2c963f66afa6","name":"string","talkTitle":"string","talkDescription":"string","linkeInProfile":"string","devEventId":"3fa85f64-5717-4562-b3fc-2c963f66afa6"}
        /// </remarks>
        /// <param name="id">Identificador do evento</param>
        /// <param name="speaker">Informações do palestrante</param>
        /// <returns>Nada</returns>
        /// <response code="204">Sucesso</response>
        /// <response code="404">Evvento não encontrado</response>
        [HttpPost("{id}/speakers")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult PostSpeaker(Guid id, DevEventSpeaker speaker)
        {
            speaker.DevEventId = id;

            var devEvent = _context.DevEvents.Any(d => d.Id.Equals(id));

            if (!devEvent)
            {
                return NotFound();
            }
            _context.DevEventSpeaker.Add(speaker);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
