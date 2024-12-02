# Use Ubuntu 22.04 as the base image
FROM ubuntu:22.04

# Install dependencies
RUN apt-get update && apt-get install -y wget curl apt-transport-https && \
    wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y dotnet-sdk-8.0

RUN export TRACER_VERSION=3.4.1 && \
mkdir -p /var/log/datadog && mkdir -p /opt/datadog && \curl -LO https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb && dpkg -i ./datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb

# Set the working directory
WORKDIR /app

# Copy the server project files
COPY . .

# Build the application
RUN dotnet publish -c Release -o out

# Create the Unix socket file for testing purposes
RUN mkdir -p /var/run/datadog && touch /var/run/datadog/apm.socket

# Set the entry point
CMD ["dotnet", "/app/out/Server.dll"]
