Sephiroth
=========

This application comes with three users in the database and they all use the same password.

## Users:

* sephiroth@sephiroth.com
* mentor@sephiroth.com
* me - for email testing purposes

 Sephiroth (Admin) has complete control over modifying, approving, or rejecting the requests.<br/>
 There is a (Manager) role (no user assigned with this role, though) that can see all of the requests and can approve or reject them.<br/>
 Mentor (User) can only see and modify their own requests.<br/>

Built with VSCode, .NET Core 2.1, and a SQLite database.

SendGrid (https://sendgrid.com) was used for sending emails.