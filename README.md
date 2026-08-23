# dotnet-dynamic-expressions-demo
Demonstrating using expression trees to build dynamic expressions for database queries.

## Create database and run migration
Create MySQL database with name of your choosing, i.e. `dotnet-dynamic-expr-demo`, then configure the connection string in `user-secrets`:

```
dotnet user-secrets init

dotnet user-secrets set \
"ConnectionStrings:DefaultConnection" \
"Server=localhost;Database=dotnet-dynamic-expr-demo;User Id=user;Password=password"
```

Once the database exists and the connection string has been configured, run the migration:

```
dotnet ef database update
```

## Add entities to database
```
POST /api/users
{
	"name": "John Doe",
	"yearsExperience": 5
}
```

```
POST /api/users/{userId}/skills

{
	"name": "typescript",
	"yearsExperience": 2
}
```

The above requests return the ID of the created entity.

## Example query

```
POST /api/queries

{
	"scope": "user",
	"and": [
		{
			"scope": "user",
			"prop": "yearsExperience",
			"op": "gte",
			"intVal": 5
		},
		{
			"scope": "skill",
			"or": [
				{
					"and": [
						{
							"prop": "name",
							"op": "eq",
							"strVal": "node"
						},
						{
							"prop": "yearsExperience",
							"op": "gte",
							"intVal": 2
						}
					]
				},
				{
					"and": [
						{
							"prop": "name",
							"op": "eq",
							"strVal": "typescript"
						},
						{
							"prop": "yearsExperience",
							"op": "gte",
							"intVal": 2
						}
					]
				}
			]
		}
	]
}
```
