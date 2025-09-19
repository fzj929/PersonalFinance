@echo off
echo 正在启动个人记账系统...
echo.

echo [1/2] 启动后端服务 (端口 8082)
start "后端服务" cmd /k "cd backend && dotnet run"

echo [2/2] 启动前端服务 (端口 8980)
start "前端服务" cmd /k "cd frontend && npm run serve"

echo.
echo 服务启动中，请稍候...
echo 后端API: http://localhost:8082
echo 前端界面: http://localhost:8980
echo.
echo 按任意键退出...
pause >nul