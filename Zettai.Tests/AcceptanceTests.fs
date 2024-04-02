namespace Zettai.Tests

open Xunit
open Swensen.Unquote
open Microsoft.AspNetCore.Hosting

module AcceptanceTests =
    let buildApp () =
        new Microsoft.AspNetCore.TestHost.TestServer(new WebHostBuilder() |> Zettai.configure)


    [<Fact>]
    let hello_world () =
        let app = buildApp ()
        let client = app.CreateClient()

        let result =
            task {
                let! response = client.GetAsync("\hello")
                let! content = response.Content.ReadAsStringAsync()
                return content
            }
            |> Async.AwaitTask
            |> Async.RunSynchronously

        test <@ result = "Hello world!" @>
