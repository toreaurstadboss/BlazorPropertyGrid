# Publishing & Deployment routine of BlazorPropertyGrid to Nuget for the BlazorPropertyGrid


## Quick tip - publishing this package to Nuget.org from CLI

Below are some quick tips I made for myself concerning updating the publishing of this BlazorPropertyGrid to Nuget, so consumers more easily can grab hold of the newest version of the Blazor Property Grid.



First: Edit the BlazorPropertyGridComponents project file and bump the 
version number of the Version element inside. Versions must be unique in Nuget.

E.g. bump to 1.2.0 or higher:

<Version>1.1.0-alpha</Version>


Run dotnet build to build the component package
```bash
dotnet restore
dotnet build --configuration Release
```

Run dotnet pack to create nuget package
```bash
BlazorPropertyGrid\ComponentLibrary>dotnet pack BlazorPropertyGridComponents.csproj --configuration Release                                                                                  
```

Now push the nuget packed file *.pkg file to nuget.org. Adjusting the version number to match up with the version number you set.
BlazorPropertyGrid\ComponentLibrary>dotnet nuget push bin\Release\BlazorPropertyGridComponents.1.0.0.nupkg --api-key TOP_SECRET  --source https://api.nuget.org/v3/index.json

<hr />
