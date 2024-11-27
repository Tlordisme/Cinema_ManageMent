using CM.ApplicationService.Food.Abstracts;
using CM.Dtos.Food;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CM.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComboController : ControllerBase
    {
        private readonly IComboService _comboService;

        public ComboController(IComboService comboService)
        {
            _comboService = comboService;
        }

        // POST: api/combo
        [HttpPost]
        public ActionResult<string> CreateCombo([FromBody] AddOrUpdateComboDto comboDto)
        {
            try
            {
                var comboId = _comboService.CreateCombo(comboDto);
                return Ok(new { Id = comboId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/combo/{id}
        [HttpGet("{id}")]
        public ActionResult<ComboDto> GetComboById(string id)
        {
            try
            {
                var combo = _comboService.GetComboById(id);
                return Ok(combo);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/combo
        [HttpGet]
        public ActionResult<List<ComboDto>> GetAllCombos()
        {
            try
            {
                var combos = _comboService.GetAllCombos();
                return Ok(combos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/combo/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateCombo(string id, [FromBody] ComboDto comboDto)
        {
            try
            {
                comboDto.Id = id;  // Ensure that the DTO ID matches the parameter
                _comboService.UpdateCombo(comboDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/combo/{id}
        [HttpDelete("{id}")]
        public ActionResult DeleteCombo(string id)
        {
            try
            {
                _comboService.DeleteCombo(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
