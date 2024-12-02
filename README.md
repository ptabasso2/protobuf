
# Procedure to get started with the project

## 1. Clone the Repository
To clone the repository, open your terminal and run the following command:

```bash
git clone https://github.com/ptabasso2/protobuf.git
```

This will create a local copy of the project on your machine.

---

## 2. Navigate to the Project Directory
After cloning the repository, navigate into the project directory:

```bash
cd protobuf
```

---

## 3. Install Dependencies
Ensure you have the required dependencies installed:

1. **Install the .NET SDK 8.0**:
   - You can download and install it from the [official .NET website](https://dotnet.microsoft.com/download/dotnet).

2. **Verify the installation**:
   ```bash
   dotnet --version
   ```

---

## 4. Build the Project
The project contains both the **client** and **server** applications. Build them as follows:

### Build the Server
Navigate to the `Server` directory and run:
```bash
cd Server
dotnet restore
dotnet build
dotnet publish -c Release -o out
```

### Build the Client
Navigate to the `Client` directory and run:
```bash
cd Client
dotnet restore
dotnet build
dotnet publish -c Release -o out
```

---

## 5. Instrumentation Explanation
The project uses **Datadog.Trace** to instrument the **client** and **server** applications for distributed tracing and profiling. Key details:

- The **instrumentation configuration** is stored in the `instrument.env` file.
- **Trace Context Propagation**: The **client** propagates trace context to the **server**, allowing Datadog to link operations in a distributed trace.
- **Continuous Profiling**: Profiling is enabled to track CPU and memory usage in real-time. This is configured via environment variables in the `instrument.env` file.

---

## 6. Continuous Profiling
Continuous profiling has been enabled using Datadog's profiler engine, embedded in the **Datadog.Trace** library. This feature provides:

- Real-time visibility into application performance.
- Detailed insights into CPU and memory usage.

This data is sent to Datadog's **Continuous Profiler** for analysis.

---

## 7. Bootstrapping the Project

### Option 1: Using Docker Compose (Recommended)
1. **Install Docker**:
   - Follow the [Docker installation guide](https://docs.docker.com/get-docker/).

2. **Build and Start Services**:
   
   Make sure before this that you have a valid Datadog API Key. 
   From the root project directory (where the `docker-compose.yml` file is located), run:
   ```bash
   DD_API_KEY=<your api key> docker-compose up -d --build
   ```

   This will:
   - Build the **client** and **server** images.
   - Start the `dd-agent-dogfood-jmx` container for Datadog monitoring.
   - Run the **client** and **server** containers.

3. **Verify the Application**:
   - The **client** sends requests to the **server**, propagating trace context.
   - Use Datadog's **APM** and **Continuous Profiler** to analyze the behavior.

### Option 2: Running Locally Without Docker

1. **Install the Datadog .Net tracing library**:
Open a terminal and run the following commands:
```bash
export TRACER_VERSION=3.4.1
mkdir -p /var/log/datadog
mkdir -p /opt/datadog
curl -LO https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb
dpkg -i ./datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb
```

2. **Run the Server**:
   In a first terminal, navigate to the `Server` directory and run:
   ```bash
   . ./instrument.env
   dotnet run
   ```

3. **Run the Client**:
   Open a new terminal, navigate to the `Client` directory, and run:
   ```bash
   . ./instrument.env
   dotnet run
   ```

4. **(Optional) Running the Client multiple times**:
The last command will run the client only one. Once the result is returned to the client, it will then exit.
Therefore if you wish to run several requests, in the same terminal, you can run:
```bash
while true; do dotnet run; sleep 2; done
```

---

## 8. Accessing Datadog
Once the project is running, open the **Datadog dashboard** to view:

- **Distributed Traces**:
  - See how requests propagate between the **client** and **server**.
  - Analyze request timing and performance.

- **Continuous Profiler**:
  - View CPU and memory profiling data for both the **client** and **server**.
  - Identify bottlenecks and optimize performance.
