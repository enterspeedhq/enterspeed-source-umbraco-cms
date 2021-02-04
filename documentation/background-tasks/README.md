# Background tasks

Enterspeed is running two background tasks in Umbraco

## HandleEnterspeedJobs Task

This task is handling all jobs that is "Pending".  
This will usually be when all content has been [Seeded](../jobs/README.md).  
This task will run every 60 seconds,
and will handle jobs in batches of 50,
untill there is no more Pending jobs.  
**Note** that there will only be one task running at a time,  
which means that if a task is already handling pending jobs, a new task will not be created.

## InvalidateEnterspeedJobs Task

This task will change the state of jobs that has been "Processing"
for more than 1 hour, to "Failed".  
This is done to cleanup the Jobs queue.  
This task will run every 10 minutes.
