module SqlTokenReplacer.Types


type FileInfo =
    { FileName: string
      Content: string [] }

type Command =
    | All
    | List
    | Single
    | WhereList
    | WhereZip


type CmdInfo =
    { CmdStr: string
      Args: string list
      CmdType: Command
      Variables: FileInfo list }
