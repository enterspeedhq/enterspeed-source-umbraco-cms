# Jobs

A job is an instruction that needs to be executed in order
to syncronize data with Enterspeed.  
A job contains information about what content from Umbraco,  
that needs to be handled, and how it should be handled (ie. Publish or Delete).

Jobs are stored in a [custom table](./../database/README.md).

## Job lifecycle

When a content node in Umbraco is being published, unpublished,
deleted or moved a new job will be added to  
the [EnterspeedJobs table](./../database/README.md)
with the state of "Pending".  
Immediately after the jobs has been created, they will be handled by the
EnterspeedJobHandler, which is changing the state of the jobs to "Processing".  
A job will then either be deleted, if it could be handled without any errors,
or set to "Failed" with the exception(s) that was thrown.

## EnterspeedJobHandler

The EnterspeedJobHandler is responsible for taking a job,  
fetching data from Umbraco, converting it to an IEnterspeedEntity
and sending it to the Enterspeed Ingest API.  
For each job that is being handled all previously failed jobs,
for the same content node will be fetched and deleted,  
so we only have a maximum of 1 failed job per content node
with the recent exception. If a previously failed job is handled with success,  
the failed job will also be deleted, since it's no longer failing.

## Seeding jobs

In the Enterspeed Content dashboard located under Content in Umbraco,
you have the possibility to Seed all your content.  
When the button is clicked, all content will be queued up
for publishing to Enterspeed.  
This means that for each published content node (and variant) in Umbraco, 
there will be created a publishing job.  
This will only create publishing jobs and not deleting jobs.

## Old processing jobs

If a job has been in the state of "Processing" for more than 1 hour,  
the state of that job will automatically change to "Failed".  
This is done to cleanup jobs that, for some reason timed out while processing.  
This is done by the [InvalidateEnterspeedJobs background task](./../background-tasks/README.md)
