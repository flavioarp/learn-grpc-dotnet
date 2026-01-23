# native

```
sudo apt update
sudo apt install -y \
  build-essential \
  cmake \
  pkg-config \
  protobuf-compiler \
  protobuf-compiler-grpc \
  libprotobuf-dev \
  libgrpc++-dev \
  libarmadillo-dev
```

The protocol buffer compiler must have the same major and minor version of the protobuf libraries the server will be linked against. This can be checked with:

```protoc --version```

```dpkg -l | grep libprotobuf```

### Build

First generate the message and gRPC interface classes:

```
protoc \
  --proto_path=proto \
  --cpp_out=. \
  --grpc_out=. \
  --plugin=protoc-gen-grpc=$(which grpc_cpp_plugin) \
  compute.proto
```

Then compile the server

```
g++ server.cpp compute.pb.cc compute.grpc.pb.cc \
  $(pkg-config --cflags --libs grpc++) \
  $(pkg-config --cflags --libs protobuf) \
  -larmadillo \
  -o compute_server
```

## Deploy

If a static binary was generated (```ldd ./server``` returns ```not a dynamic executable```), then only ```ca-certificates``` will be necessary.

On the other hand, a dinamically linked binary will need:
```
sudo apt install -y \
  libgrpc++XX \
  libprotobuf32 \
  libarmadillo11
```
where XX is ```libgrpc++``` version available (check with ```apt search libgrpc++``` on a debian-based distribution).