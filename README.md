# shu

[![NuGet Badge](https://buildstats.info/nuget/shu)](https://www.nuget.org/packages/shu/)

SHell Utilities

<hr/>

## Quickstart

- Requirements: [Download NET Core SDK](https://dotnet.microsoft.com/download)
- Install the tool:

```sh
dotnet tool install -g shu
```

- To update if already installed:

```sh
dotnet tool update -g shu
```

- if `~/.dotnet/tools` dotnet global tool isn't in path it can be added to your `~/.bashrc`

```sh
echo 'export PATH=$PATH:~/.dotnet/tools' >> ~/.bashrc
```

## enable completion

to enable completion edit `/etc/bash_completion.d/shu`

```sh
_fn() {  
        COMPREPLY=($(SHOW_COMPLETIONS=1 shu ${COMP_LINE:2}))
}
complete -F _fn shu
```

## command line

```sh
devel0@main:~$ shu
missing command

Usage: shu COMMAND FLAGS

shell utils

Commands:
  replace-token   replace token from given standard input (not optimized for huge files)

Global flags:
  -h              show usage
```

### replace token

```sh
missing required parameter [token]

Usage: shu replace-token FLAGS token replacement

replace token from given standard input (not optimized for huge files)

Optional flags:
  -csregex      token will treated as csharp regex

Global flags:
  -h            show usage

Parameters
  token         token to search for
  replacement   text to replace where token was found
```

## How this project was built

```sh
mkdir shu
cd shu

dotnet new sln
dotnet new console -n shu

cd shu
dotnet add package netcore-util --version 1.0.32
dotnet add package netcore-cmdline --version 0.2.1
cd ..

dotnet sln shu.sln add shu
dotnet build
./shu/bin/Debug/netcoreapp3.0/shu
```
