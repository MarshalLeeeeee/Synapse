# SynapseCommon
SynapseCommon includes common functions shared in both server and client.

## Rules of Naming
The rules of naming are listed below
- If server and client share the whole implementation of class, the class is named normally.
- If server or client has exclusive implementation besides to that of common, the class of named ```[Class]Common``` in SynapseCommon workspace, and both server and client inherit from ```[Class]Common``` with ```[Class]```.
- The script files in SynapseCommon workspace are not necessarily named with ```Common``` suffix.
- The script files with the same functionality are suggested to be named with same relative path. For instance, we have ```SynapseCommon/Common/Const.cs```, ```SynapseServer/Server/Const.cs``` and ```SynapseClient/Client/Const.cs```.
