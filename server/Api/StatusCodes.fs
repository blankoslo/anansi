module Api.StatusCodes

open Giraffe

let withStatusCode statusCode message =
    setStatusCode statusCode >=> text message

let badRequest message = withStatusCode 400 message
let unauthorized message = withStatusCode 401 message
let forbidden message = withStatusCode 403 message
let notFound message = withStatusCode 404 message
