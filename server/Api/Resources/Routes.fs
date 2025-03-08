module Api.Resources.Routes

open Giraffe
open Api.Resources.ResourceHandler

let resourceRoutes : HttpHandler =
    choose [
        GET >=> choose [
            route "/resources" >=> getResourcesHandler
            routef "/resources/%i" getResourceByIdHandler
        ]

        DELETE >=> routef "/resources/%i" deleteResourceHandler
        POST >=> route "/resources" >=> postResourceHandler
        PUT >=> routef "/resources/%i" updateResourceHandler
    ]