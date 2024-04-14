module Zettai

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe
open System.Text.Json.Serialization

let getListsForUserHandler (fetcher: Domain.ListFetcher) user =
    let lists = fetcher user
    let mapped = lists |> List.map Contract.fromDomain
    let response: Contract.GetListsResponse = { lists = mapped |> Array.ofSeq }
    json response

let webApp (fetcher: Domain.ListFetcher) =
    choose [ route "/" >=> text "Hello Zettai user!"
             routef "/%s/lists" (getListsForUserHandler fetcher) ]

let configureApp fetcher (app: IApplicationBuilder) = app.UseGiraffe(webApp fetcher)

let jsonOptions =
    let options = SystemTextJson.Serializer.DefaultOptions
    options.Converters.Add(JsonFSharpConverter(JsonUnionEncoding.FSharpLuLike))
    options

let configureServices (services: IServiceCollection) =

    services
        .AddGiraffe()
        .AddSingleton<Json.ISerializer>(SystemTextJson.Serializer(jsonOptions))
    |> ignore

let configure fetcher (webHostBuilder: IWebHostBuilder) =
    webHostBuilder
        .Configure(configureApp fetcher)
        .ConfigureServices(configureServices)

[<EntryPoint>]
let main _ =
    let fetcher: Domain.ListFetcher =
        (fun _ ->
            [ { name = "books"
                description = "my bookshelf"
                status = Domain.Status.Todo
                percentageDone = 0m } ])

    Host
        .CreateDefaultBuilder()
        .ConfigureWebHostDefaults((configure fetcher) >> ignore)
        .Build()
        .Run()

    0
