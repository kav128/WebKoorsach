using System;
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
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CourseController> _logger;
        private readonly IMapper _mapper;

        public CourseController(ICourseService courseService, ILogger<CourseController> logger, IMapper mapper)
        {
            _courseService = courseService;
            _logger = logger;
            _mapper = mapper;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IList<CourseModel>> GetAll()
        {
            IList<CourseDto> dtos = await _courseService.GetAll();
            CourseModel[] models = _mapper.Map<CourseModel[]>(dtos);
            return models;
        }

        [HttpGet("{id}", Name = "Course Get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CourseModel>> GetById(int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError(nameof(id), "'id' must be positive");
                return BadRequest(ModelState);
            }

            try
            {
                CourseDto? dto = await _courseService.GetCourse(id);
                if (dto is null) return NotFound();
                var model = _mapper.Map<CourseModel>(dto);
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
        public async Task<IActionResult> Insert(CourseAddModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var dto = _mapper.Map<CourseDto>(model);
            try
            {
                int id = await _courseService.AddCourse(dto);
                return CreatedAtRoute("Course Get", new { Id = id },
                    new CourseModel(id, model.Name, model.LecturerId, Array.Empty<int>()));
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
        public async Task<IActionResult> Update(CourseUpdateModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var dto = _mapper.Map<CourseDto>(model);
            try
            {
                await _courseService.UpdateCourse(dto);
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return BadRequest("Course with such id does not exist.");
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
                ModelState.AddModelError(nameof(id), "'id' must be positive");
                return BadRequest(ModelState);
            }

            try
            {
                await _courseService.DeleteCourse(id);
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return BadRequest("Course with such id does not exist.");
            }
            catch (IncorrectIdException e)
            {
                _logger.LogWarning(e, "It seems, validation does not cover some errors.");
                return BadRequest(e.Message);
            }
        }
    }
}
