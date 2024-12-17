pipeline {
    agent any

    environment {
        DOCKER_COMPOSE_PATH = 'docker-compose.yml'
    }

    stages {
        stage('Checkout Code') {
            steps {
                echo 'Checking out source code...'
                checkout scm
            }
        }

        stage('Deploy with Docker Compose') {
            steps {
<<<<<<< HEAD
                echo 'Deploying services using Docker Compose...'
                bat 'docker-compose -f Jenkins/.jenkins/workspace/MovieTheater/MovieTheaterAPI/docker-compose.yml down' // Dừng các container cũ
                bat 'docker-compose -f Jenkins/.jenkins/workspace/MovieTheater/MovieTheaterAPI/docker-compose.yml up --build -d' // Build lại image và khởi động container
            }
        }

        stage('Verify Deployment') {
=======
                script {
                    // Build image cho cm_sql
                    sh 'docker build -t cm_sql ./cm_sql'  // Thay ./cm_sql bằng đường dẫn đến thư mục chứa Dockerfile của cm_sql
                    
                    // Build image cho cm_api
                    sh 'docker build -t cm_api ./cm_api'  // Thay ./cm_api bằng đường dẫn đến thư mục chứa Dockerfile của cm_api
                }
            }
        }


        stage('Deploy Containers') {
>>>>>>> 0e88f29df3ed8c98aff43eefee264f2217829b8d
            steps {
                echo 'Verifying deployment...'
                bat 'docker ps'
            }
        }
    }

    post {
        success {
            echo 'Deployment completed successfully!'
        }
        failure {
            echo 'Deployment failed. Check logs for details.'
        }
    }
}