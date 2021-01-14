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
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentController> _logger;
        private readonly IMapper _mapper;

        public StudentController(IStudentService studentService, ILogger<StudentController> logger, IMapper mapper)
        {
            _studentService = studentService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IList<StudentModel>> GetAll()
        {
            IList<StudentDto> dtos = await _studentService.GetStudents();
            var models = _mapper.Map<IList<StudentModel>>(dtos);
            return models;
        }

        [HttpGet("{id}", Name = "Student Get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentModel>> GetById(int id)
        {
            if (id <= 0)
            {
                ModelState.AddModelError(nameof(id), "'id' must be positive");
                return BadRequest(ModelState);
            }

            try
            {
                StudentDto? dto = await _studentService.GetStudent(id);
                if (dto is null) return NotFound();
                var model = _mapper.Map<StudentModel>(dto);
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
        public async Task<IActionResult> Insert(StudentAddModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var dto = _mapper.Map<StudentDto>(model);
            try
            {
                int id = await _studentService.AddStudent(dto);
                return CreatedAtRoute("Student Get", new { Id = id }, new StudentModel(id, model.FullName));
            }
            catch (IncorrectIdException e)
            {
                _logger.LogWarning(e, "It seems, validation does not cover some errors.");
                return BadRequest(e.Message);
            }
        }

        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(StudentUpdateModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var dto = _mapper.Map<StudentDto>(model);
                await _studentService.UpdateStudent(dto);
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return BadRequest("Student with such id does not exist.");
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
                await _studentService.DeleteStudent(id);
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return BadRequest("Student with such id does not exist.");
            }
            catch (IncorrectIdException e)
            {
                _logger.LogWarning(e, "It seems, validation does not cover some errors.");
                return BadRequest(e.Message);
            }
        }
    }
}
