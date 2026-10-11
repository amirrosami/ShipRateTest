using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShipRate.Application.DTOs;
using ShipRate.Application.DTOs.AppTasks;
using ShipRate.Application.Interfaces;
using ShipRate.Domain.Entities.Operations.TasksManage;
using ShipRate.Infrastructure.Data;

namespace ShipRate.Application.Services
{
    public class AppTaskService : IAppTaskService
    {
        private readonly ApplicationDbContext _db;

        public AppTaskService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<AppTaskIndexViewModel> GetIndexDataAsync(
            AppTaskFilterDto filter,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            filter ??= new AppTaskFilterDto();

            if (filter.PageNumber <= 0)
                filter.PageNumber = 1;

            if (filter.PageSize <= 0)
                filter.PageSize = 10;

            var result = await SearchAsync(filter, currentUserId, cancellationToken);

            return new AppTaskIndexViewModel
            {
                Search = new AppTaskSearchViewModel
                {
                    Filter = filter,
                    TaskStatuses = await GetTaskStatusSelectListAsync(cancellationToken),
                    Priorities = await GetPrioritySelectListAsync(cancellationToken),
                    Users = await GetUserSelectListAsync(cancellationToken),
                    PageSizes = GetPageSizeSelectList(filter.PageSize)
                },
                Result = result
            };
        }

        public async Task<PagedResultDto<AppTaskListItemDto>> SearchAsync(
            AppTaskFilterDto filter,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            filter ??= new AppTaskFilterDto();

            if (filter.PageNumber <= 0)
                filter.PageNumber = 1;

            if (filter.PageSize <= 0)
                filter.PageSize = 10;

            var currentAssignments = _db.AppTaskAssignments
                .Where(x => x.IsCurrent)
                .Select(x => new
                {
                    x.AppTaskId,
                    x.AssignedToUserId
                });

            var query =
                from t in _db.AppTasks.AsNoTracking()
                join ca in currentAssignments on t.Id equals ca.AppTaskId into caJoin
                from currentAssign in caJoin.DefaultIfEmpty()
                join owner in _db.Users.AsNoTracking() on t.OwnerUserId equals owner.Id into ownerJoin
                from ownerUser in ownerJoin.DefaultIfEmpty()
                join assignee in _db.Users.AsNoTracking() on currentAssign.AssignedToUserId equals assignee.Id into assigneeJoin
                from assigneeUser in assigneeJoin.DefaultIfEmpty()
                join etypes in _db.EntityTypes.AsNoTracking() on t.EntityTypeId equals etypes.Id into etypesJoin
                from etype in etypesJoin.DefaultIfEmpty()
                select new AppTaskListItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    TaskDesciption = t.TaskDesciption,
                    EntityTypeId = t.EntityTypeId,
                    RelatedEntityId = t.RelatedEntityId,
                    TaskStatusId = t.TaskStatusId,
                    PriorityId = t.PriorityId,
                    OwnerUserId = t.OwnerUserId,
                    OwnerUserName = ownerUser != null ? ownerUser.FullName : null,
                    CurrentAssignedToUserId = currentAssign != null ? currentAssign.AssignedToUserId : null,
                    CurrentAssignedToUserName = assigneeUser != null ? assigneeUser.FullName : null,
                    DueDate = t.DueDate,
                    CreatedDate = t.CreatedAt,
                    StartedDate = t.StartedDate,
                    CompletedDate = t.CompletedDate,
                    EntityTypeName = t.EntityType.EntityTypeName
                };

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var keyword = filter.Keyword.Trim();

                query = query.Where(x =>
                    x.Title.Contains(keyword) ||
                    (x.TaskDesciption != null && x.TaskDesciption.Contains(keyword)));
            }

            if (filter.TaskStatusId.HasValue)
                query = query.Where(x => x.TaskStatusId == filter.TaskStatusId.Value);

            if (filter.PriorityId.HasValue)
                query = query.Where(x => x.PriorityId == filter.PriorityId.Value);

            if (filter.OwnerUserId.HasValue)
                query = query.Where(x => x.OwnerUserId == filter.OwnerUserId.Value);

            if (filter.AssignedToUserId.HasValue)
                query = query.Where(x => x.CurrentAssignedToUserId == filter.AssignedToUserId.Value);

            if (filter.EntityTypeId.HasValue)
                query = query.Where(x => x.EntityTypeId == filter.EntityTypeId.Value);

            if (filter.RelatedEntityId.HasValue)
                query = query.Where(x => x.RelatedEntityId == filter.RelatedEntityId.Value);

            if (filter.MyTasksOnly == true)
                query = query.Where(x => x.OwnerUserId == currentUserId || x.CurrentAssignedToUserId == currentUserId);

            if (filter.IncludeCompleted != true)
                query = query.Where(x => !x.CompletedDate.HasValue);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.CompletedDate.HasValue)
                .ThenByDescending(x => x.PriorityId)
                .ThenBy(x => x.DueDate)
                .ThenByDescending(x => x.CreatedDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            var taskStatusMap = await GetTaskStatusesAsync(cancellationToken);
            var priorityMap = await GetPrioritiesAsync(cancellationToken);
            var entityTypeMap = await GetEntityTypesAsync(cancellationToken);

            foreach (var item in items)
            {
                item.TaskStatusName = item.TaskStatusId.HasValue && taskStatusMap.ContainsKey(item.TaskStatusId.Value)
                    ? taskStatusMap[item.TaskStatusId.Value]
                    : null;

                item.PriorityName = item.PriorityId.HasValue && priorityMap.ContainsKey(item.PriorityId.Value)
                    ? priorityMap[item.PriorityId.Value]
                    : null;

                item.EntityTypeName = item.EntityTypeId.HasValue && entityTypeMap.ContainsKey(item.EntityTypeId.Value)
                    ? entityTypeMap[item.EntityTypeId.Value]
                    : null;
            }

            return new PagedResultDto<AppTaskListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<CreateAppTaskViewModel> GetCreateDataAsync(
            int? taskId,
            int? entityTypeId,
            Guid? relatedEntityId,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            return new CreateAppTaskViewModel
            {
                Input = new CreateAppTaskDto
                {
                    TaskId = taskId,
                    EntityTypeId = entityTypeId,
                    RelatedEntityId = relatedEntityId,
                    OwnerUserId = currentUserId
                },
                Priorities = await GetPrioritySelectListAsync(cancellationToken),
                SourceTypes = await GetSourceTypeSelectListAsync(cancellationToken),
                Users = await GetUserSelectListAsync(cancellationToken),
                Tasks = await GetTaskSelectListAsync(cancellationToken),
            };
        }

        public async Task<Guid> CreateAsync(
            CreateAppTaskDto input,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var openStatusId = 1;

            var entity = new AppTask
            {
                Title = input.Title.Trim(),
                TaskDesciption = input.TaskDesciption?.Trim(),
                EntityTypeId = input.EntityTypeId,
                RelatedEntityId = input.RelatedEntityId,
                OwnerUserId = currentUserId,
                TaskStatusId = openStatusId,
                PriorityId = input.PriorityId,
                SourceTypeId = input.SourceTypeId,
                DueDate = input.DueDate,
                CreatedAt = DateTime.Now,
                ResultNotes = "-",
                CreateUserId = currentUserId,
            };

            _db.AppTasks.Add(entity);
            await _db.SaveChangesAsync(cancellationToken);

            if (input.AssignedToUserIds != null && input.AssignedToUserIds.Any())
            {
                var orderedUsers = input.AssignedToUserIds.Distinct().ToList();

                for (int i = 0; i < orderedUsers.Count; i++)
                {
                    _db.AppTaskAssignments.Add(new AppTaskAssignment
                    {
                        AppTaskId = entity.Id,
                        AssignedToUserId = orderedUsers[i],
                        AssignedByUserId = currentUserId,
                        AssignedAt = DateTime.Now,
                        IsCurrent = i == 0,
                        Note = i == 0 ? "Initial assignment" : "Assignment history"
                    });
                }
            }

            if (input.CheckLists != null && input.CheckLists.Any())
            {
                foreach (var item in input.CheckLists)
                {
                    if (string.IsNullOrWhiteSpace(item.CheckListItemTitle))
                        continue;

                    _db.AppTaskCheckLists.Add(new AppTaskCheckList
                    {
                        AppTaskId = entity.Id,
                        CheckListItemTitle = item.CheckListItemTitle.Trim(),
                        Description = item.Description?.Trim(),
                        IsMandatory = item.IsMandatory,
                        IsChecked = false,
                        SortOrder = item.SortOrder
                    });
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }

        public async Task<AppTaskDetailsDto> GetDetailsAsync(
            Guid id,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            var task = await _db.AppTasks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (task == null)
                throw new Exception("Task not found.");

            var owner = await _db.Users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == task.OwnerUserId, cancellationToken);

            var currentAssignment = await _db.AppTaskAssignments
                .AsNoTracking()
                .Where(x => x.AppTaskId == task.Id && x.IsCurrent)
                .OrderByDescending(x => x.AssignedAt)
                .FirstOrDefaultAsync(cancellationToken);

            var assignee = currentAssignment == null
                ? null
                : await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == currentAssignment.AssignedToUserId, cancellationToken);

            var assignments = await (
                from a in _db.AppTaskAssignments.AsNoTracking()
                join au in _db.Users.AsNoTracking() on a.AssignedToUserId equals au.Id into auJoin
                from assignedUser in auJoin.DefaultIfEmpty()
                join bu in _db.Users.AsNoTracking() on a.AssignedByUserId equals bu.Id into buJoin
                from byUser in buJoin.DefaultIfEmpty()
                where a.AppTaskId == task.Id
                orderby a.AssignedAt descending
                select new AppTaskAssignmentDto
                {
                    Id = a.Id,
                    AppTaskId = a.AppTaskId,
                    AssignedToUserId = a.AssignedToUserId,
                    AssignedToUserName = assignedUser != null ? assignedUser.FullName : null,
                    AssignedByUserId = a.AssignedByUserId.Value,
                    AssignedByUserName = byUser != null ? byUser.FullName : null,
                    AssignedDate = a.AssignedAt,
                    IsCurrent = a.IsCurrent,
                    Notes = a.Note
                }).ToListAsync(cancellationToken);

            var comments = await (
                from c in _db.AppTaskComments.AsNoTracking()
                join u in _db.Users.AsNoTracking() on c.CreateUserId equals u.Id into userJoin
                from user in userJoin.DefaultIfEmpty()
                where c.AppTaskId == task.Id
                orderby c.CreatedAt descending
                select new AppTaskCommentDto
                {
                    Id = c.Id,
                    AppTaskId = c.AppTaskId.Value,
                    CommentText = c.CommentText,
                    CommentTypeId = c.CommentTypeId,
                    IsInternal = c.IsInternal,
                    CreatedByUserId = c.CreateUserId,
                    CreatedByUserName = user != null ? user.FullName : null,
                    CreatedDate = c.CreatedAt
                }).ToListAsync(cancellationToken);

            var checklists = await (
                from c in _db.AppTaskCheckLists.AsNoTracking()
                join u in _db.Users.AsNoTracking() on c.CheckedByUserId equals u.Id into userJoin
                from user in userJoin.DefaultIfEmpty()
                where c.AppTaskId == task.Id
                orderby c.SortOrder, c.Id
                select new AppTaskChecklistItemDto
                {
                    Id = c.Id,
                    AppTaskId = c.AppTaskId,
                    CheckListItemTitle = c.CheckListItemTitle,
                    Description = c.Description,
                    IsMandatory = c.IsMandatory,
                    IsChecked = c.IsChecked,
                    SortOrder = c.SortOrder,
                    CheckedByUserId = c.CheckedByUserId,
                    CheckedByUserName = user != null ? user.FullName : null,
                    CheckedDate = c.CheckedDate
                }).ToListAsync(cancellationToken);

            var taskStatusMap = await GetTaskStatusesAsync(cancellationToken);
            var priorityMap = await GetPrioritiesAsync(cancellationToken);
            var sourceTypeMap = await GetSourceTypesAsync(cancellationToken);
            var entityTypeMap = await GetEntityTypesAsync(cancellationToken);

            var hasOpenMandatoryChecklist = checklists.Any(x => x.IsMandatory && !x.IsChecked);

            var canComplete = !task.CompletedDate.HasValue &&
                              (task.OwnerUserId == currentUserId || (currentAssignment != null && currentAssignment.AssignedToUserId == currentUserId));

            var canComment = !task.CompletedDate.HasValue;
            var canAssign = !task.CompletedDate.HasValue;

            var model = new AppTaskDetailsDto
            {
                Id = task.Id,
                Title = task.Title,
                TaskDesciption = task.TaskDesciption,
                EntityTypeId = task.EntityTypeId,
                EntityTypeName = task.EntityTypeId.HasValue && entityTypeMap.ContainsKey(task.EntityTypeId.Value)
                    ? entityTypeMap[task.EntityTypeId.Value]
                    : null,
                RelatedEntityId = task.RelatedEntityId,
                TaskStatusId = task.TaskStatusId,
                TaskStatusName = taskStatusMap.ContainsKey(task.TaskStatusId)
                    ? taskStatusMap[task.TaskStatusId]
                    : null,
                PriorityId = task.PriorityId,
                PriorityName = task.PriorityId.HasValue && priorityMap.ContainsKey(task.PriorityId.Value)
                    ? priorityMap[task.PriorityId.Value]
                    : null,
                SourceTypeId = task.SourceTypeId,
                SourceTypeName = task.SourceTypeId.HasValue && sourceTypeMap.ContainsKey(task.SourceTypeId.Value)
                    ? sourceTypeMap[task.SourceTypeId.Value]
                    : null,
                OwnerUserId = task.OwnerUserId,
                OwnerUserName = owner?.FullName,
                CurrentAssignedToUserId = currentAssignment?.AssignedToUserId,
                CurrentAssignedToUserName = assignee?.FullName,
                CreatedDate = task.CreatedAt,
                StartedDate = task.StartedDate,
                DueDate = task.DueDate,
                CompletedDate = task.CompletedDate,
                CanComplete = canComplete,
                CanComment = canComment,
                CanAssign = canAssign,
                HasOpenMandatoryChecklist = hasOpenMandatoryChecklist,
                Assignments = assignments,
                Comments = comments,
                CheckLists = checklists
            };

            model.Timeline = BuildTimeline(model);
            model.Users = await GetUsersSelectListAsync();
            return model;
        }

        public async System.Threading.Tasks.Task AssignAsync(
            AssignAppTaskDto input,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            var task = await _db.AppTasks
                .FirstOrDefaultAsync(x => x.Id == input.AppTaskId, cancellationToken);

            if (task == null)
                throw new Exception("Task not found.");

            if (task.CompletedDate.HasValue)
                throw new Exception("Completed task cannot be assigned.");

            var currentAssignments = await _db.AppTaskAssignments
                .Where(x => x.AppTaskId == input.AppTaskId && x.IsCurrent)
                .ToListAsync(cancellationToken);

            foreach (var item in currentAssignments)
                item.IsCurrent = false;

            _db.AppTaskAssignments.Add(new AppTaskAssignment
            {
                AppTaskId = input.AppTaskId,
                AssignedToUserId = input.AssignedToUserId,
                AssignedByUserId = currentUserId,
                AssignedAt = DateTime.Now,
                IsCurrent = true,
                Note = input.Notes
            });

            //if (!task.StartedDate.HasValue)
            //    task.StartedDate = DateTime.Now;

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async System.Threading.Tasks.Task AddCommentAsync(
            AddAppTaskCommentDto input,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            var task = await _db.AppTasks
                .FirstOrDefaultAsync(x => x.Id == input.AppTaskId, cancellationToken);

            if (task == null)
                throw new Exception("Task not found.");

            if (task.CompletedDate.HasValue)
                throw new Exception("Cannot add comment to completed task.");

            _db.AppTaskComments.Add(new AppTaskComment
            {
                AppTaskId = input.AppTaskId,
                CommentText = input.CommentText.Trim(),
                CommentTypeId = input.CommentTypeId,
                IsInternal = input.IsInternal,
                CreateUserId = currentUserId,
                CreatedAt = DateTime.Now
            });

            if (!task.StartedDate.HasValue)
                task.StartedDate = DateTime.Now;

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async System.Threading.Tasks.Task CompleteAsync(
            CompleteAppTaskDto input,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            var task = await _db.AppTasks
                .FirstOrDefaultAsync(x => x.Id == input.AppTaskId, cancellationToken);

            if (task == null)
                throw new Exception("Task not found.");

            if (task.CompletedDate.HasValue)
                throw new Exception("Task already completed.");

            var currentAssignment = await _db.AppTaskAssignments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.AppTaskId == task.Id && x.IsCurrent, cancellationToken);

            var canComplete = task.OwnerUserId == currentUserId ||
                              (currentAssignment != null && currentAssignment.AssignedToUserId == currentUserId);

            if (!canComplete)
                throw new Exception("Only current assignee or owner can complete task.");

            var hasOpenMandatoryChecklist = await _db.AppTaskCheckLists
                .AnyAsync(x => x.AppTaskId == task.Id && x.IsMandatory && !x.IsChecked, cancellationToken);

            if (hasOpenMandatoryChecklist)
                throw new Exception("Mandatory checklist items must be completed first.");

            var completedStatusId = 3;

            if (!task.StartedDate.HasValue)
                task.StartedDate = DateTime.Now;

            task.CompletedDate = DateTime.Now;
            task.TaskStatusId = completedStatusId;

            if (!string.IsNullOrWhiteSpace(input.ResultNotes))
            {
                _db.AppTaskComments.Add(new AppTaskComment
                {
                    AppTaskId = task.Id,
                    CommentText = input.ResultNotes.Trim(),
                    CommentTypeId = 2,
                    IsInternal = false,
                    CreateUserId = currentUserId,
                    CreatedAt = DateTime.Now
                });
            }

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async System.Threading.Tasks.Task ToggleCheckListItemAsync(
            ToggleAppTaskChecklistDto input,
            Guid currentUserId,
            CancellationToken cancellationToken = default)
        {
            var task = await _db.AppTasks
                .FirstOrDefaultAsync(x => x.Id == input.AppTaskId, cancellationToken);

            if (task == null)
                throw new Exception("Task not found.");

            if (task.CompletedDate.HasValue)
                throw new Exception("Completed task checklist cannot be changed.");

            var item = await _db.AppTaskCheckLists
                .FirstOrDefaultAsync(x => x.Id == input.CheckListId && x.AppTaskId == input.AppTaskId, cancellationToken);

            if (item == null)
                throw new Exception("Checklist item not found.");

            item.IsChecked = input.IsChecked;
            item.CheckedByUserId = input.IsChecked ? currentUserId : null;
            item.CheckedDate = input.IsChecked ? DateTime.Now : null;

            if (!task.StartedDate.HasValue)
                task.StartedDate = DateTime.Now;

            await _db.SaveChangesAsync(cancellationToken);
        }

        private List<AppTaskTimelineItemDto> BuildTimeline(AppTaskDetailsDto model)
        {
            var timeline = new List<AppTaskTimelineItemDto>
            {
                new AppTaskTimelineItemDto
                {
                    EventType = "Created",
                    Title = "Task created",
                    Description = model.Title,
                    EventDate = model.CreatedDate,
                    UserName = model.OwnerUserName,
                    SortOrder = 1
                }
            };

            if (model.StartedDate.HasValue)
            {
                timeline.Add(new AppTaskTimelineItemDto
                {
                    EventType = "Started",
                    Title = "Task started",
                    Description = null,
                    EventDate = model.StartedDate.Value,
                    UserName = null,
                    SortOrder = 2
                });
            }

            foreach (var a in model.Assignments)
            {
                timeline.Add(new AppTaskTimelineItemDto
                {
                    EventType = "Assigned",
                    Title = $"Assigned to {a.AssignedToUserName}",
                    Description = a.Notes,
                    EventDate = a.AssignedDate,
                    UserName = a.AssignedByUserName,
                    SortOrder = 3
                });
            }

            foreach (var c in model.Comments)
            {
                timeline.Add(new AppTaskTimelineItemDto
                {
                    EventType = "Comment",
                    Title = "Comment added",
                    Description = c.CommentText,
                    EventDate = c.CreatedDate,
                    UserName = c.CreatedByUserName,
                    SortOrder = 4
                });
            }

            foreach (var ch in model.CheckLists.Where(x => x.IsChecked && x.CheckedDate.HasValue))
            {
                timeline.Add(new AppTaskTimelineItemDto
                {
                    EventType = "Checklist",
                    Title = $"Checklist checked: {ch.CheckListItemTitle}",
                    Description = null,
                    EventDate = ch.CheckedDate!.Value,
                    UserName = ch.CheckedByUserName,
                    SortOrder = 5
                });
            }

            if (model.CompletedDate.HasValue)
            {
                timeline.Add(new AppTaskTimelineItemDto
                {
                    EventType = "Completed",
                    Title = "Task completed",
                    Description = null,
                    EventDate = model.CompletedDate.Value,
                    UserName = null,
                    SortOrder = 6
                });
            }

            return timeline
                .OrderBy(x => x.EventDate)
                .ThenBy(x => x.SortOrder)
                .ToList();
        }

        private async Task<List<SelectListItem>> GetTaskStatusSelectListAsync(CancellationToken cancellationToken)
        {
            return await GetLookupSelectListAsync("TaskStatus", cancellationToken);
        }

        private async Task<List<SelectListItem>> GetPrioritySelectListAsync(CancellationToken cancellationToken)
        {
            return await GetLookupSelectListAsync("Priority", cancellationToken);
        }

        private async Task<List<SelectListItem>> GetSourceTypeSelectListAsync(CancellationToken cancellationToken)
        {
            return await GetLookupSelectListAsync("SourceType", cancellationToken);
        }

        private async Task<List<SelectListItem>> GetUserSelectListAsync(CancellationToken cancellationToken)
        {
            return await _db.Users.AsNoTracking()
                .Where(x => x.UserTypeId == 1)
                .OrderBy(x => x.FullName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName
                })
                .ToListAsync(cancellationToken);
        }

        private async Task<List<SelectListItem>> GetTaskSelectListAsync(CancellationToken cancellationToken)
        {
            return await _db.Tasks.AsNoTracking()
                .OrderBy(x => x.TaskName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.TaskName
                })
                .ToListAsync(cancellationToken);
        }


        private async Task<List<SelectListItem>> GetLookupSelectListAsync(string category, CancellationToken cancellationToken)
        {
            //return await _db.LookupValues.AsNoTracking()
            //    .Where(x => x.Category == category && x.IsActive)
            //    .OrderBy(x => x.SortOrder)
            //    .ThenBy(x => x.Title)
            //    .Select(x => new SelectListItem
            //    {
            //        Value = x.Id.ToString(),
            //        Text = x.Title
            //    })
            //    .ToListAsync(cancellationToken);
            switch (category)
            {
                case "TaskStatus":
                    return await _db.TaskStatus.AsNoTracking()
                        .OrderBy(x => x.Id)
                        .Select(x => new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.TaskStatusName
                        })
                        .ToListAsync(cancellationToken);
                case "Priority":
                    return await _db.FollowUpPriorities.AsNoTracking()
                        .OrderBy(x => x.Id)
                        .Select(x => new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.FollowUpPriorityName
                        })
                        .ToListAsync(cancellationToken);
                case "SourceType":
                    return await _db.TaskSourceTypes.AsNoTracking()
                        .OrderBy(x => x.Id)
                        .Select(x => new SelectListItem
                        {
                            Value = x.Id.ToString(),
                            Text = x.TaskSourceName
                        })
                        .ToListAsync(cancellationToken);

                default:
                    return null;
            }

        }


        private async Task<Dictionary<int, string>> GetTaskStatusesAsync(CancellationToken cancellationToken)
        {
            return await _db.TaskStatus.AsNoTracking()
                .ToDictionaryAsync(x => x.Id, x => x.TaskStatusName, cancellationToken);
        }
        private async Task<Dictionary<int, string>> GetPrioritiesAsync(CancellationToken cancellationToken)
        {
            return await _db.FollowUpPriorities.AsNoTracking()
                .ToDictionaryAsync(x => x.Id, x => x.FollowUpPriorityName, cancellationToken);
        }
        private async Task<Dictionary<int, string>> GetSourceTypesAsync(CancellationToken cancellationToken)
        {
            return await _db.TaskSourceTypes.AsNoTracking()
                .ToDictionaryAsync(x => x.Id, x => x.TaskSourceName, cancellationToken);
        }
        private async Task<Dictionary<int, string>> GetEntityTypesAsync(CancellationToken cancellationToken)
        {
            return await _db.EntityTypes.AsNoTracking()
                .ToDictionaryAsync(x => x.Id, x => x.EntityTypeName, cancellationToken);
        }


        //private async Task<Dictionary<int, string>> GetLookupMapAsync(string category, CancellationToken cancellationToken)
        //{
        //    //return await _db.LookupValues.AsNoTracking()
        //    //    .Where(x => x.Category == category && x.IsActive)
        //    //    .ToDictionaryAsync(x => x.Id, x => x.Title, cancellationToken);


        //    return await _db.TaskStatus.AsNoTracking()
        //        .ToDictionaryAsync(x => x.Id, x => x.TaskStatusName, cancellationToken);


        //}

        private List<SelectListItem> GetPageSizeSelectList(int selectedPageSize)
        {
            var sizes = new[] { 10, 20, 50, 100 };

            return sizes.Select(x => new SelectListItem
            {
                Value = x.ToString(),
                Text = x.ToString(),
                Selected = x == selectedPageSize
            }).ToList();
        }


        public async System.Threading.Tasks.Task StartAsync(Guid taskId, Guid currentUserId, CancellationToken cancellationToken = default)
        {
            var task = await _db.AppTasks
                .FirstOrDefaultAsync(x => x.Id == taskId && !x.IsDeleted);

            if (task == null)
                throw new InvalidOperationException("Task not found.");

            if (task.TaskStatusId == 3)
                throw new InvalidOperationException("Completed task cannot be started.");

            if (task.TaskStatusId == 2)
                return;

            task.TaskStatusId = 2;
            task.StartedDate = DateTime.Now;
            task.UpdatedAt = DateTime.Now;
            task.EditUserId = currentUserId;
            await _db.SaveChangesAsync();
        }


        public async Task<List<AppTaskDashboardItemDto>> GetDashboardTasksAsync(
            Guid currentUserId,
            string filter,
            string? keyword,
            int take = 5)
        {
            var query = _db.AppTasks
                .AsNoTracking()
                .Include(x => x.Assignments)
                    .ThenInclude(x => x.AssignedToUser)
                .Where(x => !x.IsDeleted && x.TaskStatusId != 3);

            if (filter == "my")
            {
                query = query.Where(x =>
                    x.Assignments.Any(a =>
                        a.IsCurrent &&
                        a.AssignedToUserId == currentUserId) || x.CreateUserId == currentUserId);
            }
            //else if (filter == "assigned-by-me")
            //{
            //    query = query.Where(x => x.CreateUserId == currentUserId);
            //}
            //else if (filter == "overdue")
            //{
            //    var today = DateTime.Today;

            //    query = query.Where(x =>
            //        x.DueDate.HasValue &&
            //        x.DueDate.Value.Date < today &&
            //        x.TaskStatusId != 3);
            //}
            else
            {
                query = query.Where(x => x.TaskStatusId != 3);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x =>
                    x.Title.Contains(keyword) ||
                    x.TaskDesciption.Contains(keyword));
            }

            return await query
                .OrderBy(x => x.DueDate == null)
                .ThenBy(x => x.DueDate)
                .ThenByDescending(x => x.Id)
                .Take(take)
                .Select(x => new AppTaskDashboardItemDto
                {
                    Id = x.Id,
                    TaskTitle = x.Title,
                    TaskDescription = x.TaskDesciption,

                    TaskStatusId = x.TaskStatusId,
                    TaskStatusName = x.TaskStatus.TaskStatusName,

                    PriorityId = x.PriorityId,
                    PriorityName = x.Priority != null ? x.Priority.FollowUpPriorityName : null,

                    AssignedToUserId = x.Assignments
                        .Where(a => a.IsCurrent)
                        .Select(a => (Guid?)a.AssignedToUserId)
                        .FirstOrDefault(),

                    AssignedToUserName = x.Assignments
                        .Where(a => a.IsCurrent)
                        .Select(a => a.AssignedToUser.FullName)
                        .FirstOrDefault(),

                    DueDate = x.DueDate,

                    RelatedEntityTypeId = x.EntityTypeId,
                    RelatedEntityId = x.RelatedEntityId,
                })
                .ToListAsync();
        }
        public async Task<List<SelectListItem>> GetUsersSelectListAsync()
        {
            return await _db.Users
                .AsNoTracking()
                .Where(x => x.IsActive && x.UserTypeId == 1)
                .OrderBy(x => x.FullName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName
                })
                .ToListAsync();
        }
    }
}
