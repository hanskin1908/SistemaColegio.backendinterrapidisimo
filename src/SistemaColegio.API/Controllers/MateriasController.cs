using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SistemaColegio.Application.Common.DTOs;
using SistemaColegio.Application.Features.Materias.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace SistemaColegio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MateriasController : ControllerBase
    {
        private readonly IMateriaService _materiaService;
        
        public MateriasController(IMateriaService materiaService)
        {
            _materiaService = materiaService;
        }
        
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerTodas()
        {
            var respuesta = await _materiaService.ObtenerTodasAsync();
            
            if (respuesta.Exito)
                return Ok(respuesta);
            
            return StatusCode(StatusCodes.Status500InternalServerError, respuesta);
        }
        
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var respuesta = await _materiaService.ObtenerPorIdAsync(id);
            
            if (!respuesta.Exito && respuesta.Mensaje.Contains("No se encontru00f3"))
                return NotFound(respuesta);
            
            if (respuesta.Exito)
                return Ok(respuesta);
            
            return StatusCode(StatusCodes.Status500InternalServerError, respuesta);
        }
        
        [HttpGet("estudiante/{estudianteId}")]
        [Authorize(Roles = "admin,student")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerPorEstudianteId(int estudianteId)
        {
            var respuesta = await _materiaService.ObtenerPorEstudianteIdAsync(estudianteId);
            
            if (respuesta.Exito)
                return Ok(respuesta);
            
            return StatusCode(StatusCodes.Status500InternalServerError, respuesta);
        }
        
        [HttpGet("profesor/{profesorId}")]
        [Authorize(Roles = "admin,professor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerPorProfesorId(int profesorId)
        {
            var respuesta = await _materiaService.ObtenerPorProfesorIdAsync(profesorId);
            
            if (respuesta.Exito)
                return Ok(respuesta);
            
            return StatusCode(StatusCodes.Status500InternalServerError, respuesta);
        }

        [HttpGet("profesor-materias/{profesorId}")]
        [Authorize(Roles = "professor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerMateriasPorProfesor(int profesorId)
        {
            var filtro = new ProfesorMateriasFilterDto
            {
                ProfesorId = profesorId
            };

            var respuesta = await _materiaService.ObtenerMateriasPorProfesorAsync(filtro);
            
            if (respuesta.Exito)
                return Ok(respuesta);
            
            return StatusCode(StatusCodes.Status500InternalServerError, respuesta);
        }
        [HttpGet("profesor-materias")]
        [Authorize(Roles = "professor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerMateriasDelProfesorAutenticado()
        {
            // Obtener el claim "profesorId" del usuario autenticado
            var profesorIdClaim = User.Claims.FirstOrDefault(c => c.Type == "profesorId");
            if (profesorIdClaim == null)
            {
                return Unauthorized(new { Exito = false, Mensaje = "No se encontró el claim de profesorId en el token." });
            }
            if (!int.TryParse(profesorIdClaim.Value, out int profesorId))
            {
                return Unauthorized(new { Exito = false, Mensaje = "El claim de profesorId no es válido." });
            }

            var filtro = new ProfesorMateriasFilterDto
            {
                ProfesorId = profesorId
            };

            var respuesta = await _materiaService.ObtenerMateriasPorProfesorAsync(filtro);
            if (respuesta.Exito)
                return Ok(respuesta);
            return StatusCode(StatusCodes.Status500InternalServerError, respuesta);
        }

        
        [HttpGet("codigo/{codigo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerPorCodigo(string codigo)
        {
            var respuesta = await _materiaService.ObtenerPorCodigoAsync(codigo);
            
            if (!respuesta.Exito && respuesta.Mensaje.Contains("No se encontru00f3"))
                return NotFound(respuesta);
            
            if (respuesta.Exito)
                return Ok(respuesta);
            
            return StatusCode(StatusCodes.Status500InternalServerError, respuesta);
        }
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Crear([FromBody] MateriaDto materiaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var respuesta = await _materiaService.CrearAsync(materiaDto);
            
            if (!respuesta.Exito && respuesta.Mensaje.Contains("Ya existe"))
                return BadRequest(respuesta);
            
            if (respuesta.Exito)
                return CreatedAtAction(nameof(ObtenerPorId), new { id = respuesta.Data.Id }, respuesta);
            
            return StatusCode(StatusCodes.Status500InternalServerError, respuesta);
        }
        
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Actualizar(int id, [FromBody] MateriaDto materiaDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var respuesta = await _materiaService.ActualizarAsync(id, materiaDto);
            
            if (!respuesta.Exito && respuesta.Mensaje.Contains("No se encontru00f3"))
                return NotFound(respuesta);
            
            if (!respuesta.Exito && respuesta.Mensaje.Contains("Ya existe"))
                return BadRequest(respuesta);
            
            if (respuesta.Exito)
                return Ok(respuesta);
            
            return StatusCode(StatusCodes.Status500InternalServerError, respuesta);
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Eliminar(int id)
        {
            var respuesta = await _materiaService.EliminarAsync(id);
            
            if (!respuesta.Exito && respuesta.Mensaje.Contains("No se encontru00f3"))
                return NotFound(respuesta);
            
            if (!respuesta.Exito && respuesta.Mensaje.Contains("No se puede eliminar"))
                return BadRequest(respuesta);
            
            if (respuesta.Exito)
                return Ok(respuesta);
            
            return StatusCode(StatusCodes.Status500InternalServerError, respuesta);
        }

        [HttpGet("multiple")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerPorIds([FromQuery] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest(new { Exito = false, Mensaje = "Debe proporcionar al menos un ID de materia" });
            
            var respuesta = await _materiaService.ObtenerPorIdsAsync(ids);
            
            if (respuesta.Exito)
                return Ok(respuesta);
            
            return StatusCode(StatusCodes.Status500InternalServerError, respuesta);
        }
    }
}