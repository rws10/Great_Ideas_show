Here is a list of items that need to be done to finish the project:
ToDo:
  - timed checking events
    * This is going to be taken care on the server side.
    * Create stored procedures (partly done) on the server side using T-SQL
  - Get an email for Great Ideas from Bobby Sanchez
  - Follow ADA Format
    * Label on name on form
  - Have the attachments that are ready to be uploaded listed by name
  - Add in the time sensitive auto-actions
  - (Call Janet Quinten for secuirty?)

Bugs:
  General:
    - Temp data likes to hang around sometimes.
  Comment:
    - index:
      - When the comment index is called for an idea, the URL is populated with the attributes of the idea. 
          * Any large body of an idea will cause an error.

DEV SERVER TO USE:
  - TLHINTAPPDEV01 (64 bit 2008 R2)

FUTURE UPDATES:
- Allow users to cancel uploading a document. Once a file has been selected using the "browse" 
  button, they are locked into uploading at least one file
- Add a new field of "Project Manager"
  - This will be filled out on the Active Project. It will be required when submitting for Active Project.
  - Send email to PPMO Support about the idea becoming a project.
- Add a configuration page to adjust background settings
  - i.e. length of time that passes before a denied idea is removed.
- Add the ability to search by name
- Add a page to make deleting multiple ideas?

Downloaded NuGet Packages:
- PagedList.Mvc
- log4net