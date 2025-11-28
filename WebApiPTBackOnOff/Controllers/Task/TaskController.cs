using ApplicationCore.Interfaces.Auth;
using ApplicationCore.Interfaces.Task;
using ApplicationCore.Models.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiPTBackOnOff.Shared.Utils;

namespace WebApiPTBackOnOff.Controllers.Task
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet("GetTasks")]
        public async Task<ActionResult<IEnumerable<mTask>>> GetTasks([FromQuery] bool? status)
        {
            try
            {
                var tasks = await _taskService.GetTasksAsync(status);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                await GenericUtils.Log("TaskController: Error en el servicio GetTasks", ex);
                throw;
            }
        }

        [HttpPost("CrearTask")]
        public async Task<ActionResult<mTask>> PostTask(mTask task)
        {
            try
            {
                var createdTask = await _taskService.AddTaskAsync(task);
                return CreatedAtAction(nameof(GetTasks), new { id = createdTask.iIDTask }, createdTask);
            }
            catch (Exception ex)
            {
                await GenericUtils.Log("TaskController: Error en el servicio PostTask", ex);
                throw;
            }
        }

        [HttpPut("ActualizarTask/{id}")]
        public async Task<IActionResult> PutTask(int id, mTask task)
        {
            try
            {
                if (id != task.iIDTask)
                {
                    return BadRequest(new { message = "El ID de la ruta no coincide con el ID del cuerpo." });
                }

                var updated = await _taskService.UpdateTaskAsync(task);

                if (!updated)
                {
                    return NotFound(new { message = "Tarea no encontrada." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                await GenericUtils.Log("TaskController: Error en el servicio PutTask", ex);
                throw;
            }
        }

        [HttpDelete("EliminarTask/{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var deleted = await _taskService.DeleteTaskAsync(id);

                if (!deleted)
                {
                    return NotFound(new { message = "Tarea no encontrada." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                await GenericUtils.Log("TaskController: Error en el servicio DeleteTaskAsync", ex);
                throw;
            }
        }
    }
}
