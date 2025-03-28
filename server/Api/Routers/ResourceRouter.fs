module Api.Routers.ResourceRouter

open Giraffe
open Api.Handlers.ResourceHandler

let resourceRouter : HttpHandler =
    choose [
        GET >=> choose [
            route "/resources" >=> getResourcesHandler
            routef "/resources/%i" getResourceByIdHandler
        ]

        DELETE >=> routef "/resources/%i" deleteResourceHandler
        POST >=> route "/resources" >=> postResourceHandler
        PUT >=> routef "/resources/%i" updateResourceHandler
    ]