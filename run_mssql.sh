#!/bin/bash

# Set variables
IMAGE_NAME="mssql-2022-ubuntu"
CONTAINER_NAME="marketplace-db"
SA_PASSWORD="P@ssw0rd!"

# Build the Docker image
echo "Building Docker image..."
sudo docker build -t $IMAGE_NAME .

# Check if the build was successful
if [ $? -ne 0 ]; then
    echo "Docker build failed. Exiting."
    exit 1
fi

# Check if a container with the same name already exists
if [ $(sudo docker ps -aq -f name=$CONTAINER_NAME) ]; then
    echo "Container $CONTAINER_NAME already exists. Removing it..."
    sudo docker rm -f $CONTAINER_NAME
fi

# Run the Docker container
echo "Running Docker container..."
sudo docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=$SA_PASSWORD" \
    -p 1433:1433 --name $CONTAINER_NAME -d $IMAGE_NAME

# Check if the container is running
if [ $(sudo docker ps -q -f name=$CONTAINER_NAME) ]; then
    echo "Container $CONTAINER_NAME is running."
    echo "SQL Server is starting up. It may take a minute or two to be ready."
    echo "You can check the logs with: sudo docker logs $CONTAINER_NAME"
else
    echo "Failed to start container $CONTAINER_NAME."
    exit 1
fi
