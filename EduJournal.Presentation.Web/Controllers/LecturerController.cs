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
    public class LecturerController : ControllerBase
    {
        private readonly ILecturerService _service;
        private readonly ILogger<LecturerController> _logger;
        private readonly IMapper _mapper;

        public LecturerController(ILecturerService lecturerService, ILogger<LecturerController> logger, IMapper mapper)
        {
            _service = lecturerService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IList<LecturerModel>> GetAll()
        {
            IList<LecturerDto> dtos = await _service.GetLecturers();
            var models = _mapper.Map<IList<LecturerModel>>(dtos);
            return models;
        }

        [HttpGet("{id}", Name = "Lecture Get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LecturerModel>> GetById(int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError(nameof(id), "'id' must be positive.");
                return BadRequest(ModelState);
            }
            
            try
            {
                LecturerDto? dto = await _service.GetLecturer(id);
                if (dto == null) return NotFound();
                var model = _mapper.Map<LecturerModel>(dto);
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
        public async Task<IActionResult> Insert(LecturerAddModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var dto = _mapper.Map<LecturerDto>(model);
            try
            {
                int id = await _service.AddLecturer(dto);
                return CreatedAtRoute("Lecture Get", new { Id = id }, 
                    new LecturerModel(id, model.FullName, model.Email, Array.Empty<int>()));
            }
            catch (IncorrectIdException e)
            {
                _logger.LogWarning(e, "It seems, validation does not cover some errors.");
                return BadRequest(e.Message);
            }
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(LecturerUpdateModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var dto = _mapper.Map<LecturerDto>(model);
            try
            {
                await _service.UpdateLecturer(dto);
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return BadRequest("Lecturer with such id does not exist.");
            }
            catch (IncorrectIdException e)
            {
                _logger.LogWarning(e, "It seems, validation does not cover some errors.");
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError(nameof(id), "'id' must be positive.");
                return BadRequest(ModelState);
            }

            try
            {
                await _service.DeleteLecturer(id);
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return BadRequest("Lecturer with such id does not exist.");
            }
            catch (IncorrectIdException e)
            {
                _logger.LogWarning(e, "It seems, validation does not cover some errors.");
                return BadRequest(e.Message);
            }
        }
    }
}
