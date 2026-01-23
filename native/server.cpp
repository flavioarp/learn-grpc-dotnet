#include <grpcpp/grpcpp.h>
#include "compute.grpc.pb.h"
#include <armadillo>

using grpc::Server;
using grpc::ServerBuilder;
using grpc::ServerContext;
using grpc::Status;

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
  std::string address = "unix:/tmp/compute.sock";
  ComputeServiceImpl service;

  ServerBuilder builder;
  builder.AddListeningPort(address, grpc::InsecureServerCredentials());
  builder.RegisterService(&service);

  std::unique_ptr<Server> server(builder.BuildAndStart());
  server->Wait();
}
