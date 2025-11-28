using ApplicationCore.Models.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces.Task
{
    public interface ITaskService
    {
        Task<IEnumerable<mTask>> GetTasksAsync(bool? isCompleted);
        Task<mTask> AddTaskAsync(mTask newTask);
        Task<bool> UpdateTaskAsync(mTask taskToUpdate);
        Task<bool> DeleteTaskAsync(int id);
    }
}
