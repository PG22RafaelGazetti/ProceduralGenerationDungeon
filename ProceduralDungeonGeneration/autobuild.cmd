@echo on
@echo :
@echo : this is just a test
:: Test Comment
:: Not showing up
set SRC=.
set DEST=. \build
set RELEASE=. \release
set LOGFILE=. \autobuild.log

goto :getopts
:usage
@echo Usage:
@echo   c:\> autobuild [--debug] [--help] [--pull]
@echo .
goto :bye


:: Setup the enviroment
:getopts
if /I "%1"=="/?" goto :usage
if /I "%1"=="--help" goto :usage
if /I "%1"=="--debug" set DEBUG=true & shift
if /I "%1"=="--pull" set PULL=true & shift
shift
if NOT "%1"=="" :getopts
@echo : Going
if DEFINED DEBUG (
    @echo Debug: %DEBUG%
    @echo Pull from Git: %PULL%

)

if DEFINED PULL goto :source-control
goto :build-it

:: pull from source control
:source-control
@echo :
@echo : Pulling
git pull

:: Build the project
:build-it
if EXIST %DEST% rmdir /S /Q %DEST% >>%LOGFILE%
mkdir %DEST%  >>%LOGFILE%

:: Create the release
:generate-release

:: Publish the release
:publish

:: Clean up :)
:bye
@echo :
@echo : Exit
set DEBUG=
set PULL=
set SRC=
set DEST=
set RELEASE=