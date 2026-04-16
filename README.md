# microservices-guys

## Important: Databases setup (Docker Compose)

Before starting Minikube, you must run Docker Compose to create database containers.

These databases are used as **external services** (outside Kubernetes), and your services will connect to them via connection strings.

```bash
docker compose up
```

## Run project in Minikube

### 1. Check required tools

```bash
docker --version
kubectl version --client
minikube version
```

### If Minikube is not installed:
```bash
brew install minikube
```

### 2. Start Minikube
```bash
minikube start --driver=docker --cpus=3 --memory=4096
```

### 3. Build Docker images
```bash
docker build -t users-service:latest -f src/UsersService/UsersService.Api/Dockerfile .
docker build -t core-service:latest -f src/CoreService/CoreService.Api/Dockerfile .
docker build -t gateway:latest -f src/Gateway/Gateway.Api/Dockerfile .
docker build -t notification-service:latest -f src/NotificationService/NotificationService.Api/Dockerfile .
docker build -t workflow-service:latest -f src/WorkflowService/WorkflowService.Api/Dockerfile .
```

### 4. Load images into Minikube
```bash
minikube image load users-service:latest
minikube image load core-service:latest
minikube image load gateway:latest
minikube image load notification-service:latest
minikube image load workflow-service:latest
```

### 5. Apply Kubernetes manifests and check pods
```bash
kubectl apply -R -f k8s/
kubectl get pods
```

### 6. Get Gateway URL
```bash
minikube service gateway --url
```

