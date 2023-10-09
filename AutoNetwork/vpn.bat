@echo off

REM 检查服务是否正在运行
sc query "autovpn" | findstr /i "RUNNING" > nul
if %errorlevel% == 0 (
    REM 服务正在运行，停止服务
    sc stop "autovpn" > nul
    echo 服务已停止
) else (
    REM 服务未运行，启动服务
    sc start "autovpn" > nul
    echo VPN--------OK
	ping 192.168.3.1 -n 4 | more

)
