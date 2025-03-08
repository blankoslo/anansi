module Api.Resources.Routes

open Giraffe
open Api.Resources.ResourceHandler

let resourceRoutes : HttpHandler =
    choose [
        GET >=> choose [
            route "/resources" >=> getResourcesHandler
            routef "/resources/%i" getResourceByIdHandler
        ]

        POST >=> route "/resources" >=> postResourceHandler

        // PUT

        // DELETE
    ]

// få til de her fra bruno, så er jeg faktisk fornøyd!
// så kan jeg begynne å se på kompetanse-greiene :))