cmake_minimum_required(VERSION 3.20)
project(compute_server LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 17)

find_package(Protobuf CONFIG REQUIRED)
find_package(gRPC CONFIG REQUIRED)

set(PROTO_FILE ${CMAKE_CURRENT_SOURCE_DIR}/proto/compute.proto)

add_custom_command(
    OUTPUT compute.pb.cc compute.pb.h
    COMMAND protobuf::protoc
    ARGS --proto_path=${CMAKE_CURRENT_SOURCE_DIR}/proto
         --cpp_out=${CMAKE_CURRENT_BINARY_DIR}
         ${PROTO_FILE}
    DEPENDS ${PROTO_FILE}
)

add_custom_command(
    OUTPUT compute.grpc.pb.cc compute.grpc.pb.h
    COMMAND protobuf::protoc
    ARGS --proto_path=${CMAKE_CURRENT_SOURCE_DIR}/proto
         --grpc_out=${CMAKE_CURRENT_BINARY_DIR}
         --plugin=protoc-gen-grpc=$<TARGET_FILE:gRPC::grpc_cpp_plugin>
         ${PROTO_FILE}
    DEPENDS ${PROTO_FILE}
)

add_executable(server
    server/server.cpp
    compute.pb.cc
    compute.grpc.pb.cc
)

target_include_directories(server PRIVATE
    ${CMAKE_CURRENT_BINARY_DIR}
)

target_link_libraries(server
    gRPC::grpc++
    protobuf::libprotobuf
)
