# Shuttle.Extensions.EFCore.ThreadDbContextScope

Used in conjunction with the `Shuttle.Extensions.EFCore` and `Shuttle.Core.Threading` packages to provide a `DbContext` for each thread that is created.

```
PM> Install-Package Shuttle.Extensions.EFCore.ThreadDbContextScope
```

Creates a new `DbContext` for each thread that is created.  This is useful when you have a multi-threaded application that requires a `DbContext` for each thread.

## Configuration

```c#
services.AddThreadDbContextScope();
```
