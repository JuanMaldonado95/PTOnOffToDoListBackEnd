using ApplicationCore.Entities.PTOnOff.Task;
using ApplicationCore.Interfaces.Task;
using ApplicationCore.Models.Task;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiPTBackOnOff.Shared.Utils;

namespace Infrastructure.Services.Task
{
    public class TaskService : ITaskService
    {
        private readonly DbContextPTOnOff _context;

        public TaskService(DbContextPTOnOff context)
        {
            _context = context;
        }

        #region Mapeo entre Modelos (mTask) y Entidades (tblTask)

        // Mapeo (Helper): Convierte la Entidad de BD (tblTask) al Modelo de Aplicación (mTask)
        private mTask MapToModel(tblTask entity)
        {
            return new mTask
            {
                iIDTask = entity.iIDTask,
                tTitle = entity.tTitle,
                bIsCompleted = entity.bIsCompleted,
                dtDateTimeRegister = entity.dtDateTimeRegister
            };
        }

        // Mapeo (Helper): Convierte el Modelo de Aplicación (mTask) a la Entidad de BD (tblTask)
        private tblTask MapToEntity(mTask model)
        {
            int idTask = model.iIDTask ?? 0;
            int idUser = model.iIDUser ?? 0;
            return new tblTask
            {
                iIDTask = idTask,
                iIDUser = idUser,
                tTitle = model.tTitle ?? string.Empty,
                bIsCompleted = model.bIsCompleted,
                dtDateTimeRegister = model.dtDateTimeRegister
            };
        }

        #endregion

        public async Task<IEnumerable<mTask>> GetTasksAsync(bool? isCompleted)
        {
            try
            {
                var query = _context.tblTask.AsQueryable();

                if (isCompleted.HasValue)
                {
                    query = query.Where(t => t.bIsCompleted == isCompleted.Value);
                }

                var entities = await query
                    .OrderByDescending(t => t.dtDateTimeRegister)
                    .AsNoTracking()
                    .ToListAsync();

                return entities.Select(MapToModel);
            }
            catch (Exception ex)
            {
                await GenericUtils.Log("TaskService: Error en GetTasksAsync", ex);
                throw;
            }
        }

        public async Task<mTask> AddTaskAsync(mTask newTask)
        {
            try
            {
                var entity = MapToEntity(newTask);

                _context.tblTask.Add(entity);
                await _context.SaveChangesAsync();

                return MapToModel(entity);
            }
            catch (Exception ex)
            {
                await GenericUtils.Log("TaskService: Error en AddTaskAsync", ex);
                throw;
            }
        }

        public async Task<bool> UpdateTaskAsync(mTask taskToUpdate)
        {
            try
            {
                var entity = await _context.tblTask
                    .FirstOrDefaultAsync(t => t.iIDTask == taskToUpdate.iIDTask);

                if (entity == null) return false;

                entity.tTitle = taskToUpdate.tTitle;
                entity.bIsCompleted = taskToUpdate.bIsCompleted;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                await GenericUtils.Log("TaskService: Error en UpdateTaskAsync (concurrency)", ex);
                return false;
            }
            catch (Exception ex)
            {
                await GenericUtils.Log("TaskService: Error en UpdateTaskAsync", ex);
                throw;
            }
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            try
            {
                var entity = await _context.tblTask
                    .FirstOrDefaultAsync(t => t.iIDTask == id);

                if (entity == null) return false;

                _context.tblTask.Remove(entity);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                await GenericUtils.Log("TaskService: Error en DeleteTaskAsync", ex);
                throw;
            }
        }
    }
}
