# Team_Zed
Here are a list of tips and such the project.

Desired (non critical) Functions:
- Have a page that will list all of the ideas with a specific status code. 
  (e.g. all of the ideas with a status of "Submitted")
- Have a page to view own submitted ideas.
- Approval page
  - fix so that the radio buttons actually change the status code.
- Email
- Time sensitive auto-actions
- Admins having the ability to change the status of ideas to archived, active, and project submission

Looks:
Idea:
- index:
  - search bar in the top right corner

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