# shu

[![NuGet Badge](https://buildstats.info/nuget/shu/)](https://www.nuget.org/packages/shu/)

SHell Utilities

<hr/>

<!-- TOC -->
- [Quickstart](#quickstart)
- [enable completion](#enable-completion)
- [command line](#command-line)
  - [match regex](#match-regex)
  - [replace token](#replace-token)
- [How this project was built](#how-this-project-was-built)
- [old version w/analyzer tool](#old-version-wanalyzer-tool)
<!-- TOCEND -->

<hr/>

## Quickstart

- Requirements: [Download NET Core SDK](https://dotnet.microsoft.com/download)
- Install the tool:

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

### match regex

```sh
Usage: shu match-regex FLAGS regex fmt

match regex groups

Global flags:
  -h,--help   show usage

Parameters
  regex       c# regex
  fmt         format string ( use \N to print Nth group in place )
```

**example**

```sh
devel0@tuf:/opensource/shu$ acpi -b
Battery 0: Unknown, 97%
devel0@tuf:/opensource/shu$ acpi -b | shu match-regex '[,\s]*(\d+)%' 'battery percent is [\\1]'
battery percent is [97]
```

### replace token

```sh
Usage: shu replace-token FLAGS token replacement

replace token from given standard input (not optimized for huge files)

Optional flags:
  -csregex      token will treated as csharp regex

Global flags:
  -h,--help     show usage

Parameters
  token         token to search for
  replacement   text to replace where token was found
```

[**example**](https://github.com/devel0/security-manager/blob/8ed0f574fa649d5131d2ea2ea8e2dea5338500d2/docker/Dockerfile#L49-L52)

## How this project was built

```sh
mkdir shu
cd shu

dotnet new sln
dotnet new console -n shu

cd shu
dotnet add package netcore-util
dotnet add package netcore-cmdline
cd ..

dotnet sln shu.sln add shu
dotnet build
```

## old version w/analyzer tool

You can find old version 0.6.0 with analyzer tool [here](https://github.com/devel0/shu/tree/0a0d1b9a115c7d9262b947dd6a68469093de5fd1#logic2-freq-graph)

to install that version use

```sh
dotnet tool install --global shu --version 0.6.0
```