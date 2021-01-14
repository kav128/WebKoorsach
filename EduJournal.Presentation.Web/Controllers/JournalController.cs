using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EduJournal.BLL.DTO;
using EduJournal.BLL.Services.Entity;
using EduJournal.Presentation.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EduJournal.Presentation.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JournalController : ControllerBase
    {
        private readonly IJournalService _journalService;
        private readonly ILogger<JournalService> _logger;
        private readonly IMapper _mapper;

        public JournalController(IJournalService journalService, ILogger<JournalService> logger, IMapper mapper)
        {
            _journalService = journalService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IList<JournalRecordModel>> GetAll(int lectureId = default, int studentId = default, int courseId = default)
        {
            IList<JournalRecordDto> dtos = await _journalService.GetRecords(lectureId, studentId, courseId);
            var models = _mapper.Map<IList<JournalRecordModel>>(dtos);
            return models;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Insert(JournalRecordModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dto = _mapper.Map<JournalRecordDto>(model);
            await _journalService.SaveRecord(dto);
            return Ok();
        }
    }
}
