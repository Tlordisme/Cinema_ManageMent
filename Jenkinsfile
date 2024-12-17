pipeline {
    agent any
    stages {
        stage('Test Docker') {
            steps {
                script {
                    sh 'docker --version'
                }
            }
        }
        
        stage('Clone Repository') {
            steps {
                script {
                    try {
                        echo "Cloning the repository..."
                        git branch: 'main', url: 'https://github.com/Tlordisme/Cinema_ManageMent.git'
                    } catch (Exception e) {
                        echo "Failed to clone repository: ${e.message}"
                        currentBuild.result = 'FAILURE'
                        error "Stopping pipeline because the repository could not be cloned."
                    }
                }
            }
        }

        stage('Build Docker Image with Docker Compose') {
            steps {
                script {
                    try {
                        echo "Building Docker images using Docker Compose..."
                        sh 'docker-compose -f docker-compose.yml build' // Sử dụng docker-compose để build các dịch vụ
                    } catch (Exception e) {
                        echo "Failed to build Docker images using Docker Compose: ${e.message}"
                        currentBuild.result = 'FAILURE'
                        error "Stopping pipeline because the Docker Compose build failed."
                    }
                }
            }
        }

        stage('Start Containers') {
            steps {
                script {
                    try {
                        echo "Starting containers using Docker Compose..."
                        sh 'docker-compose -f docker-compose.yml up ' // Chạy các container với docker-compose
                    } catch (Exception e) {
                        echo "Failed to start containers using Docker Compose: ${e.message}"
                        currentBuild.result = 'FAILURE'
                        error "Stopping pipeline because starting containers failed."
                    }
                }
            }
        }

        stage('Run Database Migrations') {
            steps {
                script {
                    try {
                        echo "Running database migrations..."
                        sh "docker-compose exec -T cm_api bash -c 'dotnet tool install --global dotnet-ef'"
                        sh 'docker-compose exec -T cm_api dotnet ef database update' // Chạy migrations trên container `cm_api`
                    } catch (Exception e) {
                        echo "Failed to run database migrations: ${e.message}"
                        currentBuild.result = 'FAILURE'
                        error "Stopping pipeline because the migrations failed."
                    }
                }
            }
        }

        stage('Test Application') {
            steps {
                script {
                    try {
                        echo "Waiting for the application to be ready..."
                        // Đợi 30 giây để container khởi động hoàn toàn
                        sleep time: 120, unit: 'SECONDS'

        
                        echo "Testing application..."
                        sh 'curl http://localhost:7000'  
                    } catch (Exception e) {
                        echo "Application test failed: ${e.message}"
                        currentBuild.result = 'FAILURE'
                        error "Stopping pipeline because the application test failed."
                    }
                }
            }
        }


        // stage('Push Docker Image to Registry') {
        //     steps {
        //         script {
        //             try {
        //                 echo "Pushing Docker image to registry..."
        //                 sh 'docker login -u <your-username> -p <your-password>'
        //                 sh 'docker tag demo:v1 <your-username>/demo:v1'
        //                 sh 'docker push <your-username>/demo:v1'
        //             } catch (Exception e) {
        //                 echo "Failed to push Docker image: ${e.message}"
        //                 currentBuild.result = 'FAILURE'
        //                 error "Stopping pipeline because the image push failed."
        //             }
        //         }
        //     }
        // }

        // stage('Deploy to Production') {
        //     steps {
        //         script {
        //             try {
        //                 echo "Deploying application to production..."
        //                 sh 'docker-compose -f docker-compose.prod.yml up -d' // Deploy lên môi trường sản xuất (nếu có)
        //             } catch (Exception e) {
        //                 echo "Failed to deploy to production: ${e.message}"
        //                 currentBuild.result = 'FAILURE'
        //                 error "Stopping pipeline because deployment failed."
        //             }
        //         }
        //     }
        // }
    }
}
