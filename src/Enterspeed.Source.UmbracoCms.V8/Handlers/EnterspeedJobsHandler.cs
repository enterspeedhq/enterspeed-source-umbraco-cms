using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V8.Data.Models;
using Enterspeed.Source.UmbracoCms.V8.Data.Repositories;
using Umbraco.Core.Logging;

namespace Enterspeed.Source.UmbracoCms.V9.Handlers
{
    public class EnterspeedJobsHandler : IEnterspeedJobsHandler
    {
        private readonly IEnterspeedJobRepository _enterspeedJobRepository;
        private readonly ILogger _logger;
        private readonly EnterspeedJobHandlerCollection _jobHandlers;

        public EnterspeedJobsHandler(
            IEnterspeedJobRepository enterspeedJobRepository,
            ILogger logger,
            EnterspeedJobHandlerCollection jobHandlers)
        {
            _enterspeedJobRepository = enterspeedJobRepository;
            _logger = logger;
            _jobHandlers = jobHandlers;
        }

        public void HandleJobs(IList<EnterspeedJob> jobs)
        {
            // Process nodes
            var failedJobs = new List<EnterspeedJob>();
            var failedJobsToDelete = new List<EnterspeedJob>();

            // Get a dictionary of contentid and cultures to handle
            var jobsToHandle = jobs
                .Select(x => x.EntityId)
                .Distinct()
                .ToDictionary(x => x, x => jobs.Where(j => j.EntityId == x).Select(j => j.Culture).Distinct().ToList());

            // Fetch all failed jobs for these content ids. We need to do this to delete the failed jobs if they no longer fails
            var failedJobsToHandle =
                _enterspeedJobRepository.GetFailedJobs(jobs.Select(x => x.EntityId).Distinct().ToList());

            foreach (var jobInfo in jobsToHandle)
            {
                foreach (var culture in jobInfo.Value)
                {
                    // List of all jobs with this contentid and culture
                    var jobsToRun = jobs
                        .Where(x => x.EntityId == jobInfo.Key && x.Culture == culture)
                        .OrderBy(x => x.CreatedAt)
                        .ToList();

                    // Get the failed jobs and add it to the batch of jobs that needs to be handled, so we can delete them afterwards
                    failedJobsToDelete.AddRange(
                        failedJobsToHandle.Where(
                            x =>
                                x.EntityId == jobInfo.Key && x.Culture == culture));

                    // We only need to execute the latest jobs instruction.
                    var newestJob = jobsToRun.LastOrDefault();
                    if (newestJob == null)
                    {
                        continue;
                    }

                    var handler = _jobHandlers.FirstOrDefault(f => f.CanHandle(newestJob));
                    if (handler == null)
                    {
                        var message = $"No job handler available for {newestJob.EntityId} {newestJob.EntityType}";
                        failedJobs.Add(GetFailedJob(newestJob, message));
                        _logger.Warn<EnterspeedJobsHandler>(message);
                        continue;
                    }

                    try
                    {
                        handler.Handle(newestJob);
                    }
                    catch (Exception exception)
                    {
                        var message = exception?.Message ?? "Failed to handle the job";
                        failedJobs.Add(GetFailedJob(newestJob, message));
                        _logger.Warn<EnterspeedJobsHandler>(message);
                        continue;
                    }
                }
            }

            // Save all jobs that failed
            _enterspeedJobRepository.Save(failedJobs);

            // Delete all jobs - Note, that it's safe to delete all jobs because failed jobs will be created as a new job
            _enterspeedJobRepository.Delete(
                jobs.Select(x => x.Id).Concat(failedJobsToDelete.Select(x => x.Id)).ToList());

            // Throw exception with a combined exception message for all jobs that failed if any
            if (failedJobs.Any())
            {
                var failedJobExceptions = string.Join(Environment.NewLine, failedJobs.Select(x => x.Exception));
                throw new Exception(failedJobExceptions);
            }
        }

        protected EnterspeedJob GetFailedJob(EnterspeedJob handledJob, string exception)
        {
            return new EnterspeedJob
            {
                EntityId = handledJob.EntityId,
                EntityType = handledJob.EntityType,
                Culture = handledJob.Culture,
                CreatedAt = handledJob.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                JobType = handledJob.JobType,
                State = EnterspeedJobState.Failed,
                Exception = exception
            };
        }
    }
}