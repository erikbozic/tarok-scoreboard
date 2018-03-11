# needs rsync installed on client and server
# need dotnet sdk installed on client

# This is only meant for my specific server/use-case.

if uname -a | grep -q 'Microsoft'; then
  dotnet.exe publish -c Release #if detected WSL 
else
  dotnet publish -c Release # if any other distro
fi
rsync -ruP bin/Release/netcoreapp2.1/publish/* erik@172.104.250.202:/srv/tarok-api
