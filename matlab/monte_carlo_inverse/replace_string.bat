@echo off 
    setlocal enableextensions disabledelayedexpansion

    set "search=%1"
    set "replace=%2"
    set "textFile=%3"

    for /f "delims=" %%i in ('type "%textFile%" ^& break ^> "%textFile%" ') do (
        set "line=%%i"
        setlocal enabledelayedexpansion
        >>"%textFile%" echo(!line:%search%=%replace%!
        endlocal
    )