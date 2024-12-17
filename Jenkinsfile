pipeline {
    agent any

    environment {
        DOCKER_REGISTRY = "docker.io"  // Địa chỉ Docker registry (nếu có)
    }

    stages {
        stage('Clone Repository') {
            steps {
                git 'https://github.com/Tlordisme/Cinema_ManageMent.git'  // Địa chỉ repo của bạn
            }
        }

        stage('Build Docker Images') {
            steps {
                script {
                    // Build cm_sql image (SQL Server)
                    docker.build('cm_sql', 'docker-compose.yml#cm_sql')

                    // Build cm_api image (API)
                    docker.build('cm_api', 'docker-compose.yml#cm_api')
                }
            }
        }

        stage('Deploy Containers') {
            steps {
                script {
                    // Start up Docker Compose to deploy the services
                    sh 'docker-compose -f docker-compose.yml up -d'
                }
            }
        }

        stage('Run Tests') {
            steps {
                script {
                    // Đây là nơi bạn có thể thêm các bước kiểm tra (nếu có)
                    echo "Running tests..."
                }
            }
        }

        stage('Clean up') {
            steps {
                script {
                    // Dừng và xóa các container sau khi hoàn tất
                    sh 'docker-compose -f docker-compose.yml down'
                }
            }
        }
    }

    post {
        always {
            cleanWs()  // Dọn dẹp workspace sau khi pipeline hoàn thành
        }
    }
}
