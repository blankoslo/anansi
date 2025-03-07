module Api.Resources.Routes

open Giraffe
open Api.Resources.ResourceHandler

let resourceRoutes : HttpHandler =
    choose [
        GET >=> routef "/resources/%i" getResourceByIdHandler
    ]
