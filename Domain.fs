module Domain

type Status =
    | Todo
    | Doing
    | Done

type TodoList =
    { name: string
      description: string
      status: Status
      percentageDone: decimal }

type ListFetcher = string -> TodoList list
