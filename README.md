SQLite.Helper
--

####Do you use SQLite with your C# projects?####
If yes then don't worry, SQLite.Helper is a class that aims to help you to manage a SQLite database.

####How to use?####
First, make sure that **[System.Data.SQLite.dll](http://system.data.sqlite.org/index.html/doc/trunk/www/index.wiki)** is on you system.

Add the DLL to your project references and use the code
```C#
using SQLite.Helper;
```
Then call the class to use it
```C#
SQLiteHelper sqlite = new SQLiteHelper("C:\database.db");
```
where C:\database.db is the path to your database.

Then just call the method that you need, like get a string
```C#
string value = sqlite.GetString("SELECT `something` FROM `thattable` WHERE `otherthing` = 10");
```

You can check more information on the wiki.
