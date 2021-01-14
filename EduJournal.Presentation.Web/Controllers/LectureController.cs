using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EduJournal.BLL;
using EduJournal.BLL.DTO;
using EduJournal.BLL.Services.Entity;
using EduJournal.DAL.Exceptions;
using EduJournal.Presentation.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EduJournal.Presentation.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LectureController : ControllerBase
    {
        private readonly ILectureService _lectureService;
        private readonly ILogger<LectureController> _logger;
        private readonly IMapper _mapper;

        public LectureController(ILectureService lectureService, ILogger<LectureController> logger, IMapper mapper)
        {
            _lectureService = lectureService;
            _logger = logger;
            _mapper = mapper;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IList<LectureModel>>> GetAll(int courseId = default)
        {
            if (courseId < 0)
            {
                ModelState.AddModelError(nameof(courseId), "");
                return BadRequest(ModelState);
            }

            try
            {
                IList<LectureDto> dtos = courseId == default
                    ? await _lectureService.GetAll()
                    : await _lectureService.GetAllByCourse(courseId);
                var models = _mapper.Map<LectureModel[]>(dtos);
                return models;
            }
            catch (IncorrectIdException e)
            {
                _logger.LogWarning(e, "It seems, validation does not cover some errors.");
                return BadRequest(e.Message);
            }
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LectureModel>> GetById(int id)
        {
            

            try
            {
                LectureDto? dto = await _lectureService.GetLecture(id);
                if (dto is null) return NotFound();
                var model = _mapper.Map<LectureModel>(dto);
                return model;
            }
            catch (IncorrectIdException e)
            {
                _logger.LogWarning(e, "It seems, validation does not cover some errors.");
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Insert(LectureAddModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var dto = _mapper.Map<LectureDto>(model);
            try
            {
                int id = await _lectureService.AddLecture(dto);
                return CreatedAtRoute("Lecture Get", new { Id = id }, new LectureModel(id, model.Name, model.CourseId));
            }
            catch (IncorrectIdException e)
            {
                _logger.LogWarning(e, "It seems, validation does not cover some errors.");
                return BadRequest(e.Message);
            }
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(LectureUpdateModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var dto = _mapper.Map<LectureDto>(model);
            try
            {
                await _lectureService.UpdateLecture(dto);
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return BadRequest("Lecture with such id does not exist.");
            }
            catch (IncorrectIdException e)
            {
                _logger.LogWarning(e, "It seems, validation does not cover some errors.");
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError(nameof(id), "'id' must be positive.");
                return BadRequest(ModelState);
            }

            try
            {
                await _lectureService.DeleteLecture(id);
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return BadRequest("Lecture with such id does not exist.");
            }
            catch (IncorrectIdException e)
            {
                _logger.LogWarning(e, "It seems, validation does not cover some errors.");
                return BadRequest(e.Message);
            }
        }
    }
}
