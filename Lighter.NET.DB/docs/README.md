Lighter.NET.DB is a Lightweight ORM data access layer utilizing Entity Framework 6.4.4. It aims to provide an easy to use and flexsible db access experience for developer. 

With Lighter.NET.DB, you can Say Goodby to the long-winded Entity Data Model file structures, such as *.edmx, *.Context.tt, *.Designer.cs, and unecessary DbSet for db table mapping.

It features the followings:
1. Full select, update, delete support in ORM approach.
2. Easy data paging for select result set.
3. Automatic logging for update and delete operation.
4. Simpify the transactional db operations that eliminates all the boilerplate code of using tranaction, try...catch, commit when success and rollback when failure.

Release Notes
Ver. 1.3.1
	1. Bug fix for DbServiceBase.BuildRowCountSql() to handle sql syntax which contains Union keyword.

Ver. 1.3.0
	1. Add ConnectionString property to DbServiceConfig
	2. Add a LighterDb class as a general db access and transaction operation service of Lighter.NET.DB
	3. The LighterDb.NewContext() static method provide reusable db context for multiple db service, while the LighterDb.TransactionContext() provide reusable db context for transactional operation.
	4. Add BeginTransaction() to DbContextBase for trasactional db operation
	5. Clarify the concept of "reusable" for DbServiceBase and DbContextBase that when true means the underlining dbcontext owns the dbconnection and will automatically dispose the dbconnection when itself is disposed, on the contrary the false means the dbconnection is not owned by the dbcontext.

Ver. 1.2.0
	1. New DbServiceBase.SaveChangeList() that support batch updating for multiple changed(added, updated, deleted) records using transaction.

Ver. 1.1.0
	1. To save db connection resource, change db connection opening timing to be just before the sqlcommand execution rather than when creating a new db service instance.
	2. Adding a WhereBuilder to simplify creating the where condition sql expression for complicated search context with multiple optional fields.
	3. Make SetPaging() method virtual to be overridable by the generic version of DbService.
	4. Adding a UpdateExcept() method to update a record except specified fields.
	5. Adding DbLogServiceBase for creating DbService dedicated to handling logging.

Ver. 1.0.1
	1. Fix ExecuteCommand method, adding dispose of dbcontext

Ver. 1.0.0
	First Release.
