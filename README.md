# SqlTokenReplacer

This program allows you to replace tokens in SQL files. This is useful when testing the performance of queries under different conditions.

It requires a directory containing the input SQL files, and a series of 'query-variables' - text files which are named according to the 'tokens' embedded into the input SQL. 

For example: 


The input SQL, `get-data.sql`:
```
SELECT * 
FROM TABLE
WHERE data IN (#datalist[List()]<int>#)
```

and the query-variable file, `datalist.txt`:
```
1
2
3
4
5
```

will result in:

```
SELECT * 
FROM TABLE
WHERE data IN (1,2,3,4,5)
```

The token, `#datalist[List()]<int>#` has the syntax `#variables[command(args)<type>]#` and 
tells the program to look for `datalist.txt` in the variable directory and process each new line in that file 
as a comma seperated list. Multiple files can be used to provide query-variables if the command allows for it - 
this are seperated by commas: `#variables1,variables2[command(args)]<type>#`. 
Some commands can also be given arguments in brackats. In some commands, the name of the query-variable file
will be used as part of the token replacement.

#Commands
Various commands are available - the examples assume there are files in the variable directory:

`data1.txt`
```
1
2
3
4
```
`data2.txt`
```
5
6
7
8
```
`data3.txt`
```
9 
10
```

##Single

###Purpose:

Injects the first line of the variable file. 

###Args: 

None

###Example:

```
#data1[Single()]<int># -> 1 

#data1[Single()]<string># -> '1' 
```


##All

###Purpose:

Inject every line of the variable file

###Args: 

None

###Example:

```
#data1[All()]<># -> 1
                    2 
                    3
                    4
```

##List:

###Purpose:

Injects a comma seperated list of all lines in the variable file.

###Args:

None

###Example
```
#data1[List()]<string># -> '1','2','3','4'
                   
```

##WhereList

###Purpose:

Takes a single query-variable file and produces a where clause. Each line of the variable file will be used in conjunction with the variable file name.

###Args:

Can be provided a qualifier to prepend to the inserted variable file name

###Example:


```
#data1[WhereList(arg)]<int># -> arg.data1 in (1,2,3,4)
                   
```

##FlexZip

###Purpose:
Takes two query-variable files and zips their contents together into a 'where' clause. 
If the two query-variable files contain an unequal number of items, then two separate where clauses will
be produced

###Args:
Can be provided a list of qualifier names for each inserted file name

###Example: 

```
#data1,data2[FlexZip(a,b)]<int># -> ((a.data1 = 1 and b.data2 = 5) OR
                                     (a.data1 = 2 and b.data2 = 6) OR
                                     (a.data1 = 3 and b.data2 = 7) OR
                                     (a.data1 = 4 and b.data2 = 8))
                                     
#data1,data3[FlexZip(a,b)]<int># -> a.data1 in (1,2,3,4) AND 
                                    b.data3 in (9,10)
```














