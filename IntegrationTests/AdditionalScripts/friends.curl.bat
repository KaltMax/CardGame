@echo off

REM --------------------------------------------------
REM Monster Trading Cards Game - Integration Test
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

REM --------------------------------------------------
REM Show all friends and requests
echo 8) Show all friends/requests of the user
echo Retrieving friend list for altenhof...
curl -i -X GET http://localhost:10001/friends --header "Authorization: Bearer altenhof-mtcgToken"
echo.
echo "Should return HTTP 200 - and an empty list"
echo.

echo Retrieving friend list for kienboec...
curl -i -X GET http://localhost:10001/friends --header "Authorization: Bearer kienboec-mtcgToken"
echo.
echo "Should return HTTP 200 - and an empty list"
echo.

echo Retrieving friend list with missing token...
curl -i -X GET http://localhost:10001/friends --header "Authorization: Bearer missingToken"
echo.
echo "Should return HTTP 401 - and missing token error"
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
REM Send a friend request
echo 9) Send a friend request
echo Sending friend request from altenhof to kienboec...
curl -i -X POST http://localhost:10001/friends --header "Authorization: Bearer kienboec-mtcgToken" -d "altenhof"
echo.
echo "Should return HTTP 200 - Friend request sent successfully."
echo.

echo Sending friend request from kienboec to altenhof...
curl -i -X POST http://localhost:10001/friends --header "Authorization: Bearer altenhof-mtcgToken" -d "kienboec"
echo.
echo "Should return HTTP 409 - Conflict, friend request already exists."
echo.

echo Sending friend request from kienboec to kienboec...
curl -i -X POST http://localhost:10001/friends --header "Authorization: Bearer kienboec-mtcgToken" -d "kienboec"
echo.
echo "Should return HTTP 403 - Forbidden, can't send friend request to yourself."
echo.

echo Retrieving updated friend list for altenhof...
curl -i -X GET http://localhost:10001/friends --header "Authorization: Bearer altenhof-mtcgToken"
echo.
echo "Should return HTTP 200 - and a list of all friends"
echo.

echo Retrieving updated friend list for kienboec...
curl -i -X GET http://localhost:10001/friends --header "Authorization: Bearer kienboec-mtcgToken"
echo.
echo "Should return HTTP 200 - and a list of all friends"
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
REM Accept a friend request
echo 10) Accept a friend request
echo Accepting friend request from altenhof to kienboec...
curl -i -X POST http://localhost:10001/friends/accept/altenhof --header "Authorization: Bearer kienboec-mtcgToken"
echo.
echo "Should return HTTP 403 - Forbidden, creator of the friends-request is not allowed to accept it."
echo.

echo Accepting friend request from kienboec to altenhof...
curl -i -X POST http://localhost:10001/friends/accept/kienboec --header "Authorization: Bearer altenhof-mtcgToken"
echo.
echo "Should return HTTP 200 - Friend request accepted."
echo.

echo Retrieving updated friend list for altenhof...
curl -i -X GET http://localhost:10001/friends --header "Authorization: Bearer altenhof-mtcgToken"
echo.
echo "Should return HTTP 200 - and a list of all friends"
echo.

echo Retrieving updated friend list for kienboec...
curl -i -X GET http://localhost:10001/friends --header "Authorization: Bearer kienboec-mtcgToken"
echo.
echo "Should return HTTP 200 - and a list of all friends"
echo.

if %pauseFlag%==1 pause

REM --------------------------------------------------
REM Delete a friend
if %pauseFlag%==1 pause
echo 11) Delete a friend
echo Removing friend kienboec from unknown...
curl -i -X DELETE http://localhost:10001/friends/unknown --header "Authorization: Bearer altenhof-mtcgToken"
echo.
echo "Should return HTTP 404 - Friendship not found"
echo.

echo Removing friend kienboec from altenhof...
curl -i -X DELETE http://localhost:10001/friends/kienboec --header "Authorization: Bearer altenhof-mtcgToken"
echo.
echo "Should return HTTP 200 - Friendship removed successfully."
echo.

echo Retrieving updated friend list for altenhof...
curl -i -X GET http://localhost:10001/friends --header "Authorization: Bearer altenhof-mtcgToken"
echo.
echo "Should return HTTP 200 - and an empty list"
echo.

echo Retrieving updated friend list for kienboec...
curl -i -X GET http://localhost:10001/friends --header "Authorization: Bearer kienboec-mtcgToken"
echo.
echo "Should return HTTP 200 - and an empty list"
echo.

echo Accepting friend request from kienboec to altenhof...
curl -i -X POST http://localhost:10001/friends/accept/altenhof --header "Authorization: Bearer kienboec-mtcgToken"
echo.
echo "Should return HTTP 404 - Friendship not found"
echo.

if %pauseFlag%==1 pause

pause
