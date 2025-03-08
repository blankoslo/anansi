module Api.Resources.ResourceHandler

open Giraffe
open Dapper
open Npgsql

type Resource = {
    id: int
    title: string
    author: string
}

let connectDb () =
    let connectionString = System.Environment.GetEnvironmentVariable("CONNECTION_STRING")
    new NpgsqlConnection(connectionString)

let handleNull r =
    match box r with
    | null -> None
    | _ -> Some r


// GET

let getResources =
    task {
        use conn = connectDb()
        let! result = conn.QueryAsync<Resource>(
            "SELECT *
            FROM resource"
        )

        return result |> Seq.toList
    }

let getResourcesHandler : HttpHandler =
    fun next ctx ->
        task {
            let! maybeResources = getResources
            let handler =
                match maybeResources with
                | [] -> setStatusCode 404 >=> text "No resources"
                | resources -> json resources

            return! handler next ctx
        }

let getResourceById id =
    task {
        use conn = connectDb()
        let! result =
            conn.QuerySingleOrDefaultAsync<Resource>(
                "SELECT *
                FROM resource
                WHERE id = @Id", {| Id = id |}
            )

        return handleNull result
    }

let getResourceByIdHandler id : HttpHandler =
    fun next ctx ->
        task {
            let! maybeResource = getResourceById id
            let handler =
                match maybeResource with
                | Some result -> json result
                | None -> setStatusCode 404 >=> text "ResourceNotFound"

            return! handler next ctx
        }


// POST

let postResource resource =
    task {
        use conn = connectDb()
        let! id =
            conn.QuerySingleAsync<int>(
                "INSERT INTO resource (title, author)
                 VALUES (@Title, @Author)
                 RETURNING id",
                resource
            )
        return id
    }

let postResourceHandler : HttpHandler =
    fun next ctx ->
        task {
            let! resource = ctx.BindJsonAsync<Resource>()
            let! id = postResource resource
            return! json {| id = id |} next ctx
        }