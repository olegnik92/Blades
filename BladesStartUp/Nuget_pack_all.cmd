..\nuget pack ..\Blades.Auth\Blades.Auth.csproj -IncludeReferencedProjects -properties Configuration=Release -OutputDirectory ..\..\LocalNuget
..\nuget pack ..\Blades.Commands\Blades.Commands.csproj -IncludeReferencedProjects -properties Configuration=Release -OutputDirectory ..\..\LocalNuget
..\nuget pack ..\Blades.Core\Blades.Core.csproj -IncludeReferencedProjects -properties Configuration=Release -OutputDirectory ..\..\LocalNuget
..\nuget pack ..\Blades.DataStore\Blades.DataStore.csproj -IncludeReferencedProjects -properties Configuration=Release -OutputDirectory ..\..\LocalNuget
..\nuget pack ..\Blades.Es\Blades.Es.csproj -IncludeReferencedProjects -properties Configuration=Release -OutputDirectory ..\..\LocalNuget
..\nuget pack ..\Blades.Web\Blades.Web.csproj -IncludeReferencedProjects -properties Configuration=Release -OutputDirectory ..\..\LocalNuget
..\nuget pack ..\Blades.WebClient\Blades.WebClient.csproj -OutputDirectory ..\..\LocalNuget
pause