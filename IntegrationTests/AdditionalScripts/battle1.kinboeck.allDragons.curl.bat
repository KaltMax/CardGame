@echo off

REM --------------------------------------------------
REM Monster Trading Cards Game
REM --------------------------------------------------
title Monster Trading Cards Game
echo CURL Testing for Monster Trading Cards Game
echo Syntax: $1 [pause]
echo - pause: optional, if set, the script will pause after each block
echo.

set "pauseFlag=0"
for %%a in (%*) do (
    if /I "%%a"=="pause" (
        set "pauseFlag=1"
    )
)
if %pauseFlag%==1 pause

curl -i -X PUT http://localhost:10001/deck --header "Content-Type: application/json" --header "Authorization: Bearer kienboec-mtcgToken" -d "[\"4a2757d6-b1c3-47ac-b9a3-91deab093531\", \"d04b736a-e874-4137-b191-638e0ff3b4e7\", \"65ff5f23-1e70-4b79-b3bd-f6eb679dd3b5\", \"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\"]"
echo.
echo "Should return HTTP 2xx"
echo.


if %pauseFlag%==1 pause

REM --------------------------------------------------
echo 17) battle
start /b "kienboec battle" curl -i -X POST http://localhost:10001/battles --header "Authorization: Bearer kienboec-mtcgToken"
ping localhost -n 10 >NUL 2>NUL

if %pauseFlag%==1 pause

pause