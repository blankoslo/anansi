module Database.Database

open DbUp

let runMigrations connectionString =
    try 
        EnsureDatabase.For.PostgresqlDatabase connectionString
    with 
    | ex ->
        printfn "Could not connect to database, retrying: %s" ex.Message
        System.Threading.Thread.Sleep 5000
        EnsureDatabase.For.PostgresqlDatabase connectionString

    let upgrader =
        DeployChanges.To
            .PostgresqlDatabase(connectionString)
            .WithScriptsFromFileSystem("Database/Migrations")
            .WithTransactionPerScript()
            .Build()

    let result = upgrader.PerformUpgrade()
    if result.Successful then
        printfn "✅ Migrations ran successfully"
    else
        failwithf "❌ Migration failed: %s" result.Error.Message