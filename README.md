<div align="center">
<h1>Win Service Template</h1>
<p align="center">
    <img src="logo.png" width="1024" height="512">
</p>
<br>

</div>

## 🚀 Quick Start Guide
### WinServiceTemplate
WinService : Template For windows service using .Net8
Service_ExecuterPlugin : Example for WinService Plugin DLL 
AsyncPipeTransport : Comm Infra using .Net8, .Net4.8, .Net4.6


Client Examples :
Client SDK (.Net8, .Net4.8, .Net4.6)
Client CLI (.Net8)
Client Exe (.Net4.8)

Service Example :
Implement the  BaseRequestExecuter 
Register the executer in SetupRequestHandlers

## ⚙️ Setting the Service

### Install as a service 
  ```bash
  sc.exe create "MyService" binpath= "C:\Path\To\App.WindowsService.exe"
  ```
### Delete the service 
  ```bash
  sc.exe delete "MyService"
  ```

### start the a service
  ```bash
  sc.exe start "MyService"
  ```
### stop the a service
  ```bash
  sc.exe stop "MyService"
  ```

