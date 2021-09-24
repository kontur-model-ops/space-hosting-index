#!/bin/bash -e
THIS_SCRIPT_DIR=$(cd $(dirname "${BASH_SOURCE[0]}") && pwd)

FAISS_VERSION=1.7.1

docker container run \
    --rm \
    --volume "$THIS_SCRIPT_DIR/lib-faiss-native:/host" \
    "ghcr.io/vektonn/vektonn/faiss-lib:$FAISS_VERSION" \
    bash -c 'cp /lib-faiss-native/* /host/'

docker image build \
    --pull --no-cache \
    --tag vektonn/index-tests:latest \
    --file "$THIS_SCRIPT_DIR/.run-all-tests.dockerfile" \
    "$THIS_SCRIPT_DIR"

# If you are on ububtu, you can just run:
# LD_LIBRARY_PATH=$THIS_SCRIPT_DIR/lib-faiss-native
# dotnet test --configuration Release
