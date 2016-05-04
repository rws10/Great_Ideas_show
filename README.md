# Team_Zed
Team email:
teamzed@outlook.com
T3@m_Z3d
Here is a list of items that need to be done to finish the project:
ADDITIONS
Russell:
- Force titles to be unique
 * This has been done. I would like the error message to line up with the title and text box displays. 
   Also, the data with the error will still be populated there if you go "back to index" and immediately go back to create new.
   However, if you repeat the same process, it will then be gone.
- Have the ability to delete uploaded files from the edit page
- Add a contact us link on the homepage
- Add Javascript to update page when "FilterBy:" choice is made
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

Since Josh's list is so large, Alex and/or Russell should be able to help take some of the work. 
Josh:
- Make it work for IE
- Make the entire button work for clicking action
- Have the files that are ready to be uploaded listed by name
- Make "create new" button smaller on Homepage
- Make the ideas and comments exist in a paged list. limit perhaps to 10 per page? Adjustable limit?
- Add in the time sensitive auto-actions

Looks:
Overall:
- Squeeze everything into the center of the page more. Will discuss how much
- Whitespace above emblem on the entrance page and Home

Idea:
-index:
  - have the text box around the description fit vertically closer to the text itself.
*Russell can probably do this:
  - have the approval button only appear if the status is set to submitted.

- details:
  - (do this last. not very important) add a link to view the comments next to "Back to list"
*This will probably be taken care of by the first part of this.
  - cut the description text box size to about half the page before it wraps the text.

- edit: 
*Russell can probably do this
  - make the status into a dropdown with only the choices descriped in the model.

Comment:
- index:
  - When the comment index is called for an idea, the URL is populated with the attributes of the idea. 
      Any large body of an idea will cause an error.
- edit:
  - Have the body display like the body of the ideas.

BUGS
- Can submit the same idea more than once. Possible to spam click "create new" 
  and create numerous copies of the idea.
