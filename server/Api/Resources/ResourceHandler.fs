module Api.Resources.ResourceHandler

open Giraffe
open Dapper
open Npgsql

// For the time being, I cannot open Api.StatusCodes, for some reason. So I have these locally:
let withStatusCode statusCode msg = setStatusCode statusCode >=> text msg
let notFound = withStatusCode 404

type Resource = {
    id: int
    title: string
    author: string
}

let connectDb () =
    let connectionString = System.Environment.GetEnvironmentVariable("CONNECTION_STRING")
    new NpgsqlConnection(connectionString)

let handleGetNull r =
    match box r with
    | null -> None
    | _ -> Some r

let handleDeleteNull id =
    match id with
    | 0 -> None
    | _ -> Some id


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
                | [] -> notFound "Could not find any resources."
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
                 WHERE id = @Id",
                {| Id = id |}
            )

        return handleGetNull result
    }

let getResourceByIdHandler id : HttpHandler =
    fun next ctx ->
        task {
            let! maybeResource = getResourceById id
            let handler =
                match maybeResource with
                | Some result -> json result
                | None -> notFound "Could not find any resource with provided id."

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


// DELETE

let deleteResource id =
    task {
        use conn = connectDb()
        let! deletedId = conn.QueryFirstOrDefaultAsync<int>(
            "DELETE FROM resource
            WHERE id = @Id
            RETURNING id",
            {| Id = id |}
        )

        return handleDeleteNull deletedId        
    }

let deleteResourceHandler id : HttpHandler =
    fun next ctx ->
        task {
            let! deletedResourceId = deleteResource id
            let handler =
                match deletedResourceId with
                | Some id -> json {| id = id |}
                | None -> notFound "Could not find resource to delete."

            return! handler next ctx
        }