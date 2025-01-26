# WinServiceTemplate
Template For windows service using .Net8
Comm Infra using .Net8, .Net4.8, .Net4.6


Client Examples :
Client SDK (.Net8, .Net4.8, .Net4.6)
Client CLI (.Net8)
Client Exe (.Net4.8)

Service Example :
Implement the  BaseRequestExecuter 
Register the executer in SetupRequestHandlers


# Install as a service 
sc.exe create "MyService" binpath= "C:\Path\To\App.WindowsService.exe"

# Delete the service 
sc.exe delete "MyService"


# start the a service 
sc.exe start "MyService"

# stop the a service 
sc.exe stop "MyService"

