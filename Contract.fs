module Contract

type Status =
    | Todo
    | Doing
    | Done

type ToDoList =
    { name: string
      description: string
      status: Status
      percentageDone: decimal }

type GetListsResponse = { lists: ToDoList [] }

let fromDomain (list: Domain.TodoList) =
    let mapStatus (s: Domain.Status) : Status =
        match s with
        | Domain.Status.Todo -> Todo
        | Domain.Status.Doing -> Doing
        | Domain.Status.Done -> Done

    { name = list.name
      description = list.description
      status = list.status |> mapStatus
      percentageDone = list.percentageDone }
