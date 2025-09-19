# Synapse
A server-client game framework based on C#.

## Environment
Visual Studio C# Console Projects are provided respectively for both server and client. NET 8.0 is used by our projects, however lower version of NET might also be compatible. As far as we know, our implementation does not depent on new feature of NET.

## Structure
The game framework is organized in [SynapseCommon](./SynapseCommon/), [SynapseServer](./SynapseServer/), [SynapseServer](./SynapseClient/). This enhances the reusability of implementation and decreases the cost of project management.
