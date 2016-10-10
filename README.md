Here is a list of items that need to be done to finish the project:
ToDo:
  - Remove all Active Directory references
  - Add in team_zed email
  - Add reference to new database (possibly on GoDaddy?)

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