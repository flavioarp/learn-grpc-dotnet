#include <grpcpp/grpcpp.h>
#include "compute.grpc.pb.h"
#include <armadillo>

using grpc::Server;
using grpc::ServerBuilder;
using grpc::ServerContext;
using grpc::Status;

std::string GetServerAddress() {
  const char* env = std::getenv("SERVER_ADDRESS");
#ifdef _WIN32
  return env ? env : "127.0.0.1:0";
#else
  return env ? env : "unix:/tmp/compute.sock";
#endif
}

class ComputeServiceImpl final : public compute::ComputeService::Service {
public:
  Status SumMatrix(ServerContext* context,
                   const compute::MatrixRequest* request,
                   compute::MatrixReply* reply) override
  {
    arma::mat M(
                const_cast<double*>(request->data().data()),
                request->rows(),
                request->cols(),
                false
                );

    reply->set_result(arma::accu(M));
    return Status::OK;
  }
};

int main() {
  // std::string address = "unix:/tmp/compute.sock";
  std::string address = GetServerAddress();
  ComputeServiceImpl service;
  
  int port = 0;

  ServerBuilder builder;
  builder.AddListeningPort(address, grpc::InsecureServerCredentials(), &port);
  builder.RegisterService(&service);

  std::unique_ptr<Server> server(builder.BuildAndStart());

  if (!server) {
    std::cerr << "Failed to start server!" << std::endl;
    return 1;
  }
  std::cout << "Listening on port: " << port << std::endl;

  server->Wait();
}
