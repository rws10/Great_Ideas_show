# Team_Zed
Team email:
teamzed@outlook.com
T3@m_Z3d
Here is a list of items that need to be done to finish the project:
ADDITIONS
- Make it work for IE
- Make the entire button work for clicking action
- Force titles to be unique
 * This has been done. I would like the error message to line up with the title and text box displays. 
   Also, the data with the error will still be populated there if you go "back to index" and immediately go back to create new.
   However, if you repeat the same process, it will then be gone.
- Change "approved" to "added"
- Have the files ready to be uploaded listed by name
- Have the ability to delete uploaded files from the edit page
- Make "create new" button smaller on Homepage
- Make the ideas and comments exist in a paged list. limit perhaps to 10 per page? Adjustable limit?
- Edit comment box needs to be bigger
- Add space between the name of the attachment and the download link
- Add a contact us link on the homepage
- Add Filterby choice of archived and project submission
- Add Javascript to update page when "FilterBy:" choice is made
- Make "Search By:" boxes instead of radio buttons
- Whitespace above emblem on the entrance page and Home
- Add in the time sensitive auto-actions
- Squeeze everything into the center of the page more. Will discuss how much
- Integrate Active Directory
- Add Meta to check for viruses
- Get server access
  - In progress. ITLC papers submitted. Waiting on approval.
BUGS
- Can submit the same idea more than once. Possible to spam click "create new" 
  and create numerous copies of the idea.

Looks:
Idea:
  - have the approval button only appear if the status is set to submitted.
  - have the text box around the description fit vertically closer to the text itself.

- details:
  - put the files into a table. get rid of the "Link" title
  - cut the description text box size to about half the page before it wraps the text.
  - have all of the attributes, except denialReason and body, display in a group, then have the denialReason display 
        (only if the idea has been denied) followed by the description.
  - (do this last. not very important) add a link to view the comments next to "Back to list"

- edit: 
  - make the status into a dropdown with only the choices descriped in the model.

Comment:
- index:
  - When the comment index is called for an idea, the URL is populated with the attributes of the idea. 
      Any large body of an idea will cause an error.
  - Have it look something like:
     Author: This Guy
     Creation Date: This time           Body: This is the body of the comment

	 Author: Another Guy
	 Creation Date: Another time        Body: This is the body of another Comment
  - Allow for deleting a comment.
- edit:
  - Have the body display like the body of the ideas.
- create:
  - make it a large text box like in ideas