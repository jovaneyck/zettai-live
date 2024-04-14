module Zettai.SlowTests

open Xunit
open Swensen.Unquote
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.TestHost
open System.Text.Json

[<Fact>]
let ``App bootstraps correctly`` () =
    let whb =
        new WebHostBuilder()
        |> Zettai.configure (fun _ -> [])

    let server = new TestServer(whb)
    let client = server.CreateClient()

    let response =
        task {
            let! response = client.GetAsync(@"\")
            let! content = response.Content.ReadAsStringAsync()
            return content
        }
        |> Async.AwaitTask
        |> Async.RunSynchronously

    test <@ response = "Hello Zettai user!" @>

[<Fact>]
let ``Can get all lists of a user`` () =
    let lists: Domain.TodoList list =
        [ { name = "books"
            description = "my bookshelf"
            status = Domain.Status.Todo
            percentageDone = 0m } ]

    let db = [ ("jo", lists) ] |> Map.ofList

    let mapFetcher username = db |> Map.find username

    let whb =
        new WebHostBuilder()
        |> Zettai.configure mapFetcher

    let server = new TestServer(whb)
    let client = server.CreateClient()

    let response =
        task {
            let! response = client.GetAsync(@"\jo\lists")
            test <@ response.StatusCode = System.Net.HttpStatusCode.OK @>
            let! stream = response.Content.ReadAsStreamAsync()
            let! content = JsonSerializer.DeserializeAsync<Contract.GetListsResponse>(stream, jsonOptions)
            return content
        }
        |> Async.AwaitTask
        |> Async.RunSynchronously

    test
        <@ response = { lists =
                            [| { name = "books"
                                 description = "my bookshelf"
                                 status = Contract.Status.Todo
                                 percentageDone = 0m } |] } @>
