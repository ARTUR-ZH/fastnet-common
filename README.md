# fastnet-common
Libraries containing classes and methods for use across multiple projects. Ideally this library should have no dependencies to anything other than .Net platform stuff.
### `ApplicationSettings`
Use this class to retrieve applications setting keys for the current app (desktop, or web site, etc). Standard .Net types are supported. Addtional types can be registered using TConverter.RegisterTypeConverter.
### `TConverter`
> Visual Studio solution is set up to copy binaries to `C:\devroot\Binaries`.
