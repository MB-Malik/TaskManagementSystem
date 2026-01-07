using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TskMngmntSys.Authorization;
using TskMngmntSys.Entities.Task;
using TskMngmntSys.Tasks.Dtos;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;



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

        [AbpAuthorize(PermissionNames.Pages_Tasks)]
        public async Task<List<TaskListDto>> GetMyTasks()
        {
            var userId = AbpSession.UserId;

            if (!userId.HasValue)
            {
                throw new AbpAuthorizationException("User is not logged in");
            }

            var tasks = await _taskRepository
                .GetAll()
                .Where(t => t.AssignedUserId == userId.Value)
                .Select(t => new TaskListDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    DueDate = t.DueDate
                })
                .ToListAsync();

            return tasks;
        }

        [AbpAuthorize(PermissionNames.Pages_Tasks)]
        public void ChangeStatus(ChangeTaskStatusDto input)
        {
            var userId = AbpSession.UserId;

            if (!userId.HasValue)
            {
                throw new AbpAuthorizationException("User is not logged in");
            }

            var task = _taskRepository.FirstOrDefault(input.TaskId);

            if (task == null)
            {
                throw new UserFriendlyException("Task not found");
            }

            if (task.AssignedUserId != userId.Value)
            {
                throw new AbpAuthorizationException(
                    "You can only change the status of your own tasks"
                );
            }

            task.Status = input.Status;

            _taskRepository.Update(task);
        }

        [AbpAuthorize(PermissionNames.Pages_Tasks)]
        public PagedResultDto<TaskOutputDto> GetTasksByUser(GetTasksByUserInput input)
        {
            var query = _taskRepository
                .GetAll()
                .Where(t => t.AssignedUserId == input.UserId);

            var totalCount = query.Count();

            var tasks = query
                .OrderBy(input.Sorting ?? "Id DESC")
                .PageBy(input)
                .Select(t => new TaskOutputDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status.ToString(),
                    AssignedUserId = t.AssignedUserId
                })
                .ToList();

            return new PagedResultDto<TaskOutputDto>(totalCount, tasks);
        }

    }
}
