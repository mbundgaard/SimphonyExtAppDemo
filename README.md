# Simphony Extension Application DEMO

The application shows basic database connectivity and queries, as well as displaying messages with OpsContext.

The source code has been written purely for educational purpose and should not be used in production without proper adjustment.

Feel free to comment here on github or reach out at mb@muneris.dk

------

Please bring your own Simphony dll's - I cannot commit them here due to Oracle licensing, but you can find them in C:\Micros\Simphony\WebServer\wwwroot\EGateway\Handlers on any workstation running Simphony.

This project has been built with .NET 4.6.1, which means that any dll's from 18.2 and below will work fine - this project references these dlls:
- EGatewayDB.dll
- Ops.dll
- PosCommonClasses.dll
- PosCore.dll
- SimphonyUtilities.dll


***Making an extension application that runs on ALL versions of Simphony (including 19.2) is possible, but not the objective of this demo***


In regards to debugging the Extension App, I prefer to have a few lines in the projects post-build section that moves the output to the correct ExtensionApplication folder. This way we can writeprotect the dll at the same time.

Since the project is a client library it cannot be started on its own, so set the debug start action to external program (C:\Micros\Simphony\WebServer\ServiceHost.exe)

Simphony will automaticly pick up the updated dll and instantiate it and you can use breakpoints and everything else that comes with Visual Studio.

The official "Simphony POS Client Extension API Reference" can be found here: https://docs.oracle.com/cd/E91245_01/api/html/0d377fb8-a70e-4dfe-9a15-b15e4b6787ff.htm
