# Team_Zed
Team email:
teamzed@outlook.com
T3@m_Z3d
Here is a list of items that need to be done to finish the project:
ADDITIONS
Russell:
- Add a contact us link on the homepage
- Add Meta Scan to check for viruses in docs
- Integrate Active Directory
  * In progress. ITLC papers submitted. Waiting on approval.

Alex:
- Add Filterby choice of archived and project submission
- Make "Search By:" boxes instead of radio buttons
- On the comments edit page:
  * Have the "Back to List" button take the user back to the comments index
- On the comments delete page:
  * Have the "Back to List" button take the user back to the comments index- Get server access
- Add a view for handling unhandled exceptions. 
  * Reference what I did on the IdeaController/create Post for db.Savechanges().
- See about integrating "Logfor Net"
  * This is a log for occurences on the site. Good for when unhandled exceptions occur.
- 

Since Josh's list is so large, Alex and/or Russell should be able to help take some of the work. 
Josh:
- Follow ADA Format
 * Label on name on form
- Make the entire button work for clicking action
- Have the attachments that are ready to be uploaded listed by name
- Make "create new" button smaller on Homepage
- Make the ideas and comments exist in a paged list. limit perhaps to 10 per page? Adjustable limit?
- Add in the time sensitive auto-actions

Looks:

Idea:
-index: 
 - Have the body show the first 5 lines and then have th rest hidden in a "show more" type container.
   Then, accordian the rest of the idea out on clicking "show more"
- details:
  - (do this last. not very important) add a link to view the comments next to "Back to list"

- edit: 
*Russell can probably do this
  - make the status into a dropdown with only the choices described in the model.

- delete:
  - make the button for deleting the idea look like all other buttons and the same for "Back to List"
Comment:
- index:
  - When the comment index is called for an idea, the URL is populated with the attributes of the idea. 
      * Any large body of an idea will cause an error.

BUGS
- Can submit the same idea more than once. Possible to spam click "create new" 
  and create numerous copies of the idea.
