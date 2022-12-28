FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build 
WORKDIR webapi 

EXPOSE 80
EXPOSE 5000
EXPOSE 5001
EXPOSE 5090
EXPOSE 7090

#COPY PROJECT FILES
COPY ./*.csproj ./ 
RUN dotnet restore 

#COPY EVERYTHING ELSE 
COPY . .
RUN dotnet publish -c Release -o out 

#Build image 
FROM mcr.microsoft.com/dotnet/sdk:6.0 
WORKDIR /webapi
COPY --from=build /webapi/out . 
ENTRYPOINT ["dotnet","ValesFise_api.dll"]
