module SqlTokenReplacer.Types


type SqlFile =
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
      Args: string []
      CmdType: Command
      Variables: Map<string, string []> }
