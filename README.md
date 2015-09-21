edge-sql-o
=======

This is a library forked from edge-sql that  is a SQL compiler for edge.js.

See [edge.js overview](http://tjanczuk.github.com/edge) and [edge.js on GitHub](https://github.com/tjanczuk/egde) for more information. 

##Extended features
### OUTPUT clause
If `id` is generated when `insert` , you get id using by `output` clause in the same way as T-Sql.
```
var create = edge.func('sql-o', function () {
  /*
    INSERT INTO [Users]([email], [name], [passwordHash])
    OUTPUT INSERTED.*
    VALUES(@email, @name, @hash)
  */ 
});
```
### Passable Array value
```
var whereIn = edge.func('sql-o', function(){
  /*
    SELECT [name] FROM [Users]
    WHERE [id] IN ({ids})
  */
});
whereIn({
  ids:[
    "758C75A6-AFB4-4B3B-A0AF-A1B56EF120D9",
    "F2AC6578-4879-420F-ACED-069373D18647",
    "0335B59A-DED0-4703-AAE3-7878A5540B5C"
  ]
}, callback);
```
