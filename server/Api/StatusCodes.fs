module Api.StatusCodes

open Giraffe

let withStatusCode statusCode message =
    setStatusCode statusCode >=> text message

// 200
let ok message = withStatusCode 200 message
let created message = withStatusCode 201 message
let accepted message = withStatusCode 202 message
let noContent message = withStatusCode 204 message

// 400
let badRequest message = withStatusCode 400 message
let unauthorized message = withStatusCode 401 message
let forbidden message = withStatusCode 403 message
let notFound message = withStatusCode 404 message
let conflict message = withStatusCode 409 message