FROM ubuntu:24.04 AS build

RUN apt update && apt install -y \
  build-essential \
  cmake \
  libgrpc++-dev \
  protobuf-compiler \
  protobuf-compiler-grpc

WORKDIR /app
COPY . .
RUN cmake -B build && cmake --build build

FROM ubuntu:24.04

RUN apt update && apt install -y \
  libgrpc++1 \
  libprotobuf32 \
  ca-certificates \
  && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/build/server /usr/local/bin/server
EXPOSE 50051
CMD ["server"]
