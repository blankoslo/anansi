module Api.Resources.ResourceHandler

open System
open Giraffe
open DbUp
open Dapper
open Npgsql
open System.Threading.Tasks

// Resource type
type Resource = {
    id: int
    title: string
    author: string
}

let getDbConnection () =
    let connectionString = System.Environment.GetEnvironmentVariable("CONNECTION_STRING")
    new NpgsqlConnection(connectionString)

let getResourceById id =
    task {
        use conn = getDbConnection()
        return! conn.QuerySingleOrDefaultAsync<Resource option>(
            "
            SELECT id, title, author
            FROM resource
            WHERE id = @Id
            ", {| Id = id |})
    }

let getResourceByIdHandler id : HttpHandler =
    fun next ctx ->
        task {
            let! maybeResource = getResourceById id
            return!
                match maybeResource with
                | Some resource -> json resource
                | None -> setStatusCode 404 >=> text "Resource not found"
                |> fun handler -> handler next ctx
        }


// // DB function to insert a resource
// let insertResourceToDb (resource: Resource) : Task<int> = task {
//     use conn = getDbConnection()
//     let! id =
//         conn.ExecuteAsync(
//             "INSERT INTO resource (title, author) VALUES (@Title, @Author) RETURNING id",
//             resource)
//     return id
// }


// // HTTP handler to create a new resource
// let createResourceHandler : HttpHandler =
//     fun next ctx -> task {
//         let! resource = ctx.BindJsonAsync<Resource>()
//         let! id = insertResourceToDb resource
//         return! json {| id = id |} next ctx
//     }
