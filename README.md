# Deprecated in favour of https://github.com/wieslawsoltes/ProDataGrid

# TemplatedDataGrid

[![Build Status](https://dev.azure.com/wieslawsoltes/GitHub/_apis/build/status/wieslawsoltes.TemplatedDataGrid?branchName=main)](https://dev.azure.com/wieslawsoltes/GitHub/_build/latest?definitionId=91&branchName=main)
[![CI](https://github.com/wieslawsoltes/TemplatedDataGrid/actions/workflows/build.yml/badge.svg)](https://github.com/wieslawsoltes/TemplatedDataGrid/actions/workflows/build.yml)

[![NuGet](https://img.shields.io/nuget/v/TemplatedDataGrid.svg)](https://www.nuget.org/packages/TemplatedDataGrid)
[![NuGet](https://img.shields.io/nuget/dt/TemplatedDataGrid.svg)](https://www.nuget.org/packages/TemplatedDataGrid)
[![MyGet](https://img.shields.io/myget/templateddatagrid-nightly/vpre/TemplatedDataGrid.svg?label=myget)](https://www.myget.org/gallery/templateddatagrid-nightly) 

A DataGrid control based on ListBox and Grid panels.

![image](https://user-images.githubusercontent.com/2297442/129415635-15da4974-1b12-42a6-a97a-71f6cf48658b.png)

## Building TemplatedDataGrid

First, clone the repository or download the latest zip.
```
git clone https://github.com/wieslawsoltes/TemplatedDataGrid.git
```

### Build on Windows using script

* [.NET Core](https://www.microsoft.com/net/download?initial-os=windows).

Open up a command-prompt and execute the commands:
```
.\build.ps1
```

### Build on Linux using script

* [.NET Core](https://www.microsoft.com/net/download?initial-os=linux).

Open up a terminal prompt and execute the commands:
```
./build.sh
```

### Build on OSX using script

* [.NET Core](https://www.microsoft.com/net/download?initial-os=macos).

Open up a terminal prompt and execute the commands:
```
./build.sh
```

## NuGet

TemplatedDataGrid is delivered as a NuGet package.

You can find the packages here [NuGet](https://www.nuget.org/packages/TemplatedDataGrid/) and install the package like this:

`Install-Package TemplatedDataGrid`

or by using nightly build feed:
* Add `https://www.myget.org/F/templateddatagrid-nightly/api/v3/index.json` to your package sources
* Alternative nightly build feed `https://pkgs.dev.azure.com/wieslawsoltes/GitHub/_packaging/Nightly/nuget/v3/index.json`
* Update your package using `TemplatedDataGrid` feed

and install the package like this:

`Install-Package TemplatedDataGrid -Pre`

### Package Sources

* https://api.nuget.org/v3/index.json
* https://www.myget.org/F/avalonia-ci/api/v2

## Resources

* [GitHub source code repository.](https://github.com/wieslawsoltes/TemplatedDataGrid)

## License

TemplatedDataGrid is licensed under the [MIT license](LICENSE.TXT).
