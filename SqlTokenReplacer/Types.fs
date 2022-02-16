module SqlTokenReplacer.Types


type FileInfo =
    { FileName: string
      Content: string [] }

type Command =
    | All
    | List
    | Single
    | WhereList
    | FlexZip


type CmdInfo =
    { CmdStr: string
      Args: string list
      CmdType: Command
      Variables: FileInfo list
      Type: string }

type CommandFunc = FileInfo list -> FileInfo list