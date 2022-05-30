# StormLibSharp
An C# wrapper for StormLib that works on .NET 5+
Note: MPQ creation functions are currently not implemented. Please use the latest version of StormLib (ideally by compiling it yourself)
StormLib.dll architecture must match, you cannot use 'Any CPU'.

# Features
- Implemented marshal: reading of files, opening of MPQ archives and extracting of files
- Uses byte[] to store read file

# Todo
- Implement marshals of: MPQ creation, importing files, compression