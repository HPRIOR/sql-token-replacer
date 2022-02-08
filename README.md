# SqlTokenReplacer

This program allows you to replace tokens in SQL files. This is useful when testing the performance of queries under different conditions.

It requires a directory containing the input SQL files, and a series of 'query-variables' - text files which are named according to the 'tokens' embedded into the input SQL. 

For example: 

The input SQL, `get-data.sql`:
```
SELECT * 
FROM TABLE
WHERE data IN (#datalist[list()]#)
```

The query-variable file, `datalist.txt`:
```
1
2
3
4
5
```

This will result in:

```
SELECT * 
FROM TABLE
WHERE data IN (1,2,3,4,5)
```

The token, `#datalist[list()]#` has the syntax `#variables[command(args)]` and tells the program to look for `datalist.txt` and process each new line in that file as a comma seperated list. Multiple files can be used to provide query-variables if the command allows for it - this are seperated by commas: `#variables1,variables2[command]#`. Some commands can also be given arguments in brackats. In some commands, the name of the query-variable file will be used as part of the token replacement.

Various commands are available:


`single`:

Purpose:
Injects the first line of the input file. 

Args:
None


`all`:

Purpose:
Inject every line of the input file

Args:
None

'list':

Purpose:
Injects a comma seperated list of all lines in the query-variable file.

Args:
None

`where_list`

Takes a single query-variable file and produces a where clause. Each line of the query-variable file will be used in conjection with the query-varaible file name to produce:

```
file_name in (1,2,3,4,5)
```
where file_name.txt contians:
```
1
2
3
4
5
```

Args:
Can be provided a qualifer to prepend to the inserted `file_name`


`where_zip`

Purpose:
Takes multiple query-variable files and zips their contents together into a 'where' clause. Each corresponding line of the query-variable files will be used in conjuction with the query-variable file name to produce: 

```
((file_name1 = line-1 AND file_name2 = line-1) OR
 (file_name1 = line-2 AND file_name2 = line-2) OR
 (file_name1 = line-3 AND file_name2 = line-3) OR
 ...
 (file_name1 = line-n AND file_name2 = line-n))

```

The output will only be as long as the shortest query-variable file.

Args:
Can be provided a list of qualifier names for each inserted file name

Example: 

`example.sql`
```
SELECT * FROM TABLE
WHERE #data1,data2[where_zip(a,b)]#
```
`data1.txt`
```
1
2
```

`data2.txt`
```
3
4
```

output `example.sql`
```
SELECT * FROM TABLE 
WHERE  ((a.data1 = 1 AND b.data2 = 3) OR
        (a.data1 = 2 AND b.data2 = 4))
```














