# Duplicat

## Prerequisites

* .NET Core 3.0 SDK (`>= 3.0.100-preview6-012264`).

## Getting started with CLI

1. Open your terminal and navigate to the root of this repository.
1. Run `dotnet run --project ./Source/Duplicat.Cli/Duplicat.Cli.csproj --path ./MyFolderWithDuplicates --recurse`

Duplicate files paths will be printed to stdout with a new line separating the groups.

**Example**

```
dotnet run --project ./Source/Duplicat.Cli/Duplicat.Cli.csproj --path "./MyFolderWithDuplicates --recurse"
.\MyFolderWithDuplicates\mew.jpg
.\MyFolderWithDuplicates\random\mew.jpg

.\MyFolderWithDuplicates\2016\incredible.jpg
.\MyFolderWithDuplicates\germany\is this real.jpg

.\MyFolderWithDuplicates\random\cute.jpg
.\MyFolderWithDuplicates\work\cat.jpg
```

## Getting started with PowerShell

1. Open your PowerShell and navigate to the root of this repository.
1. Run `dotnet build`.
1. Run `Import-Module ./Source/Duplicat.PowerShell/bin/Debug/netstandard2.0/Duplicat.PowerShell.dll`
1. Run `Get-Item ./MyFolderWithDuplicates | Find-Duplicates -Recurse`

Duplicate files' paths will be output to the pipeline and you can pipe them into another operator like `ConvertTo-Json`.

**Example**
```powershell
Get-Item "C:\Users\nickd\Downloads\Code Test" | Find-Duplicates -Recurse | ConvertTo-Json
[
    [
        ".\\MyFolderWithDuplicates\\mew.jpg",
        ".\\MyFolderWithDuplicates\\random\\mew.jpg"
    ],
    [
        ".\\MyFolderWithDuplicates\\2016\\incredible.jpg",
        ".\\MyFolderWithDuplicates\\germany\\is this real.jpg"
    ],
    [
        ".\\MyFolderWithDuplicates\\random\\cute.jpg",
        ".\\MyFolderWithDuplicates\\work\\cat.jpg"
    ]
]
```

## Testing

Tests are split into unit and integration tests. You can run the tests with `dotnet test`.

**Duplicat.UnitTests** validates the behaviour of the duplicate-finding algorithm.

**Duplicat.IntegrationTests** validates the behaviour of the algorithm against the real file system.

## TODO
* Implement a non-recursive solution `GetContentDuplicates` 
* Cache stream values per `sizeDuplicate` batch.
* Handle permission denied on individual files (perhaps just skip them).