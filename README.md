# learn-grpc-dotnet

Sample project on how to expose C++ code via gRPC to a .NET API.

## Dependencies

.NET SDK *>= 10* <br/>
Grpc.Net.Client *>= 3.33.4* <br/>
Google.Protobuf *>= 2.76.0* <br/>
Grpc.Tools *>= 2.76.0* <br/>

### Get .NET SDK

```bash
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 10.0
./dotnet-install.sh --channel 9.0 # optional for VSCode's C# Dev Kit
echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
echo 'export PATH=$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH' >> ~/.bashrc
source ~/.bashrc
```

### Get .NET Packages

```bash
packages=(
  Grpc.Net.Client
  Google.Protobuf
  Grpc.Tools
)

for pkg in "${packages[@]}"; do
  dotnet add package "$pkg"
done
```
