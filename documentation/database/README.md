# Umbraco Database

## Tables

### EnterspeedJobs table

This table contains the jobs that needs to be executed,
in order to sync with Enterspeed.  
Read more about jobs [here](./../jobs/README.md)

#### Columns

|Name| Type |Description |
|:----              | :-----                |:-----|
|Id                 | `integer`             | Unique identifier, autoincrements |
|ContentId          | `integer`             | Id of the Umbraco node |
|Culture            | `string`              | Culture of the Umbraco node |
|JobType            | `integer`             | 0 = Publish, 1 = Delete |
|JobState           | `integer`             | 0 = Pending, 1 = Processing, 2 = Failed |
|Exception          | `string`              | Exception message if job failed |
|CreatedAt          | `datetime`            | Job creation date |
|UpdatedAt          | `datetime`            | Job updated date |
