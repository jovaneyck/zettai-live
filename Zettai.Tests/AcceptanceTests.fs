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

        let result = client.GetAsync("\hello").Result
        let content = (result.Content.ReadAsStringAsync()).Result
        test <@ content = "Hello world!" @>
