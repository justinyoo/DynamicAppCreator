# DynamicAppCreator


The general idea of the application is to connect to database servers, create databases and tables, and make application modules from these tables.

Logically at run time
you register a server,
you add database to server
you create tables in the database 
By selecting the features you want users to do on these tables, you create a module and make it available 


The abilities that can be achieved in the modules are planned as follows.

**Due to time constraints, not all features could be completed.**
(** Not completed)
Add, delete, update and list records. (deletion and update could not be completed)
Event triggering and automation while performing operations on the record. 
e.g. sending an e-mail when the first record is added,
sending sms,
executing sql command on a different table,
filling the fields you have selected with default values,
Support for automation features where database columns store and decrypt data encrypted. (**)
Automations can be extended with plugin architecture (plugin development requires coding). (**)

Adding special actions to the module by adding command buttons. Example/ Export (excel,pdf etc) (requires plugin development) (**);

Ability to establish real-time communication channels by adding socket connections (SignalR, System.Net.Sockets) (no code, no development required);
Support for connecting to external resources and performing operations (sending, receiving data) (no code, no development required).


Dynamic authorization (permission control down to the lowest level, including module permissions and column permissions of each table.
This means exactly that. While user Z can use columns a,b,c,d of table X, user y can only use columns a and d.

Ability to create n number of modules with the same table, and customize database fields in the created modules


# No security measures and verification procedures have been carried out. Using the code may cause problems.

video : https://www.youtube.com/watch?v=u7aWSqj0bno 
