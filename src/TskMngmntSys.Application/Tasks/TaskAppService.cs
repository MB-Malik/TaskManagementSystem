using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TskMngmntSys.Authorization;
using TskMngmntSys.Entities.Task;
using TskMngmntSys.Tasks.Dtos;

namespace TskMngmntSys.Tasks
{
    [AbpAuthorize(PermissionNames.Pages_Tasks)]
    public class TaskAppService : ApplicationService
    {
        private readonly IRepository<TaskItem, int> _taskRepository;

        public TaskAppService(IRepository<TaskItem, int> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [AbpAuthorize(PermissionNames.Pages_Tasks_Create)]
        public void Create(CreateTaskDto input)
        {
            var task = new TaskItem
            {
                Title = input.Title,
                Description = input.Description,
                DueDate = input.DueDate,
                Status = TaskState.Open
            };

            _taskRepository.Insert(task);
        }

        [AbpAuthorize(PermissionNames.Pages_Tasks_View)]
        public List<TaskDto> GetAll()
        {
            return _taskRepository.GetAll()
                .Select(t => new TaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    DueDate = t.DueDate
                }).ToList();
        }


        [AbpAuthorize(PermissionNames.Pages_Tasks_Delete)]
        public void Delete(int id)
        {
            _taskRepository.Delete(id);
        }

        [AbpAuthorize(PermissionNames.Pages_Tasks_Edit)]
        public void Update(UpdateTaskDto input)
        {
            var task = _taskRepository.FirstOrDefault(input.Id);

            if (task == null)
            {
                throw new UserFriendlyException("Task not found");
            }

            task.Title = input.Title;
            task.Description = input.Description;
            task.Status = input.Status;
            task.DueDate = input.DueDate;

            _taskRepository.Update(task); 
        }

        [AbpAuthorize(PermissionNames.Pages_Tasks_Edit)]
        public void AssignTask(AssignTaskDto input)
        {
            var task = _taskRepository.FirstOrDefault(input.TaskId);

            if (task == null)
            {
                throw new UserFriendlyException("Task not found");
            }

            task.AssignedUserId = input.UserId;

            _taskRepository.Update(task);
        }
    }
}
